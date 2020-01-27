using System;
using System.Collections.Concurrent;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Reflection;
using SafeOrbit.Memory.InjectionServices.Stampers;

namespace SafeOrbit.Memory.Injection
{
    /// <summary>
    ///     <p>Injection protector is a thread-safe class with a state.</p>
    ///     <p>The instance should only be used for single object.</p>
    ///     <p>
    ///         Each time you call <see cref="NotifyChanges" />, it saves the information related to the object.
    ///     </p>
    ///     <p>You can call <see cref="AlertUnnotifiedChanges" /> to throw if the objects last saved state has been changed.</p>
    /// </summary>
    /// <seealso cref="IInjectionDetector" />
    /// <seealso cref="IDisposable" />
    /// <seealso cref="ISafeObject{TObject}" />
    public class InjectionDetector : IInjectionDetector
    {
        private static readonly ConcurrentDictionary<string, IStamp<int>> CodeStampsDictionary =
            new ConcurrentDictionary<string, IStamp<int>>(); //static for caching as types always must be the same.

        private readonly IInjectionAlerter _alerter;

        private readonly IStamper<Type> _codeStamper;
        private readonly IStamper<object> _stateStamper;


        private readonly ITypeIdGenerator _typeIdGenerator;

        private IStamp<int> _lastStateStamp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InjectionDetector" /> class.
        /// </summary>
        /// <param name="scanCode">if set to <c>true</c> it'll scan and validate the code.</param>
        /// <param name="scanState">if set to <c>true</c>  it'll scan and validate the state.</param>
        /// <param name="alertChannel">The alert channel.</param>
        /// <seealso cref="ScanCode" />
        /// <seealso cref="ScanState" />
        public InjectionDetector(
            bool scanCode = true, bool scanState = true,
            InjectionAlertChannel alertChannel = Defaults.AlertChannel) : this
        (
            InjectionAlerter.StaticInstance,
            TypeIdGenerator.StaticInstance,
            StateStamper.StaticInstance,
            IlCodeStamper.StaticInstance,
            scanCode, scanState, alertChannel)
        {
        }

        /// <summary>
        ///     Internal constructor with all dependencies.
        /// </summary>
        internal InjectionDetector(
            IInjectionAlerter alerter,
            ITypeIdGenerator typeIdGenerator,
            IStamper<object> stateStamper,
            IStamper<Type> codeStamper,
            bool scanCode, bool scanState,
            InjectionAlertChannel alertChannel)
        {
            _alerter = alerter ?? throw new ArgumentNullException(nameof(alerter));
            _typeIdGenerator = typeIdGenerator;
            _stateStamper = stateStamper ?? throw new ArgumentNullException(nameof(stateStamper));
            _codeStamper = codeStamper ?? throw new ArgumentNullException(nameof(codeStamper));
            _alertChannel = alertChannel;
            ScanCode = scanCode;
            ScanState = scanState;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether <see cref="InjectionDetector" /> will keep track of the
        ///     state of the object.
        /// </summary>
        public bool ScanState { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether <see cref="InjectionDetector" /> will keep track of the
        ///     code of the object.
        /// </summary>
        public bool ScanCode { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Saves the state and/or the code  of the object.
        ///     Use <see cref="M:SafeOrbit.Memory.Injection.InjectionDetector.AlertUnnotifiedChanges(System.Object)" /> method to
        ///     check if the state has been injected.
        /// </summary>
        /// <param name="object">Object that this instance scans/tracks.</param>
        public void NotifyChanges(object @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            if (ScanState)
                SaveStateStamp(@object);
            if (ScanCode)
                SaveCodeStampFor(@object.GetType());
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="object" /> is <see langword="NULL" /></exception>
        /// <seealso cref="IAlerts" />
        public void AlertUnnotifiedChanges(object @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            //get validation results
            var isStateValid = !ScanState || IsStateValid(@object);
            var isCodeValid = !ScanCode || IsCodeValid(@object);
            //alert
            if (isStateValid && isCodeValid) return;
            if (!CanAlert) return;
            var message = new InjectionMessage(!isStateValid, !isCodeValid, @object);
            _alerter.Alert(message, AlertChannel);
        }


        private bool IsCodeValid(object obj)
        {
            var lastStamp = GetLastCodeStampFor(obj.GetType());
            var currentStamp = _codeStamper.GetStamp(obj.GetType());
            var result = currentStamp.Equals(lastStamp);
            return result;
        }

        private bool IsStateValid(object obj)
        {
            var lastStamp = _lastStateStamp;
            var currentStamp = _stateStamper.GetStamp(obj);
            var result = currentStamp.Equals(lastStamp);
            return result;
        }

        private void SaveCodeStampFor(Type type)
        {
            var codeId = GetCodeId(type);
            if (CodeStampsDictionary.ContainsKey(codeId))
                return; //code always must be the same, skip saving for performance
            var codeStamp = _codeStamper.GetStamp(type);
            CodeStampsDictionary.AddOrUpdate(codeId, codeStamp, (key, existingVal) => codeStamp);
        }

        private void SaveStateStamp(object obj)
        {
            _lastStateStamp = _stateStamper.GetStamp(obj);
        }

        /// <exception cref="ArgumentException">
        ///     Please validate the object using <see cref="NotifyChanges" /> method before
        ///     requesting a code stamp.
        /// </exception>
        private IStamp<int> GetLastCodeStampFor(Type type)
        {
            var id = GetCodeId(type);
            var valueExists = CodeStampsDictionary.TryGetValue(id, out var stamp);
            if (!valueExists)
                throw new ArgumentException(
                    $"Please validate the object using {nameof(NotifyChanges)} method before requesting a code stamp.");
            return stamp;
        }


        private string GetCodeId(Type type) => _typeIdGenerator.Generate(type);

        #region [IAlerts]

        private InjectionAlertChannel _alertChannel;

        /// <inheritdoc />
        public virtual InjectionAlertChannel AlertChannel
        {
            get => _alertChannel;
            set => _alertChannel = value;
        }

        /// <inheritdoc />
        public bool CanAlert => ScanCode || ScanState;

        #endregion

        #region [IDisposable]

        private bool _isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            _lastStateStamp = null;
            _isDisposed = true;
        }

        ~InjectionDetector() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}