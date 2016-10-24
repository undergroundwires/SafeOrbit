using System;
using System.Collections.Concurrent;
using SafeOrbit.Common.Reflection;
using SafeOrbit.Exceptions;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Stampers;
using SafeOrbit.Memory.SafeContainerServices;

namespace SafeOrbit.Memory.Injection
{
    /// <summary>
    ///     <p>Injection protector is a thread-safe class with a state.</p>
    ///     <p>
    ///         Each time you call <see cref="NotifyChanges" />, it saves the information related to object its inner
    ///         dictionary.
    ///     </p>
    ///     <p>You can call <see cref="AlertUnnotifiedChanges" /> to throw if the objects last saved state has been changed.</p>
    /// </summary>
    /// <seealso cref="IInjectionDetector" />
    /// <seealso cref="IDisposable" />
    /// <seealso cref="SafeObject{TObject}" />
    public class InjectionDetector : IInjectionDetector
    {
        private static readonly ConcurrentDictionary<string, IStamp<int>> CodeStampsDictionary =
            new ConcurrentDictionary<string, IStamp<int>>(); //static for caching as types always must be the same.

        private readonly IInjectionAlerter _alerter;

        private readonly IStamper<Type> _codeStamper;
        private readonly IObjectIdGenerator _objectIdGenerator;
        private readonly IStamper<object> _stateStamper;


        private readonly ITypeKeyGenerator _typeKeyGenerator;

        private ConcurrentDictionary<long, IStamp<int>> _stateStampsDictionary =
            new ConcurrentDictionary<long, IStamp<int>>();

        public InjectionDetector(bool justCode = true, bool justState = true, InjectionAlertChannel alertChannel = Defaults.AlertChannel) : this
        (
            InjectionAlerter.StaticInstance,
            InstanceIdGenerator.StaticInstance,
            TypeKeyGenerator.StaticInstance,
            StateStamper.StaticInstance, IlCodeStamper.StaticInstance, justCode, justState, alertChannel)
        {
        }

        internal InjectionDetector(
            IInjectionAlerter alerter,
            IObjectIdGenerator objectIdGenerator,
            ITypeKeyGenerator typeKeyGenerator,
            IStamper<object> stateStamper,
            IStamper<Type> codeStamper,
            bool scanCode, bool scanState,
            InjectionAlertChannel alertChannel)
        {
            if (alerter == null) throw new ArgumentNullException(nameof(alerter));
            if (objectIdGenerator == null) throw new ArgumentNullException(nameof(objectIdGenerator));
            if (stateStamper == null) throw new ArgumentNullException(nameof(stateStamper));
            if (codeStamper == null) throw new ArgumentNullException(nameof(codeStamper));
            _alerter = alerter;
            _objectIdGenerator = objectIdGenerator;
            _typeKeyGenerator = typeKeyGenerator;
            _stateStamper = stateStamper;
            _codeStamper = codeStamper;
            _alertChannel = alertChannel;
            ScanCode = scanCode;
            ScanState = scanState;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="InjectionDetector"/> will keep track of the state of the objects.
        /// </summary>
        public bool ScanState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="InjectionDetector"/> will keep track of the code of the objects.
        /// </summary>
        public bool ScanCode { get; set; }

        /// <summary>
        ///     Saves the state and/or the code  of the object.
        ///     Use <see cref="AlertUnnotifiedChanges" /> method to check if the state has been injected.
        /// </summary>
        public void NotifyChanges(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (this.ScanState)
            {
                SaveStateStampFor(obj);
            }
            if (this.ScanCode)
            {
                SaveCodeStampFor(obj.GetType());
            }
        }

        /// <summary>
        ///     Alerts when any unnotified changes are detected any <see cref="CanAlert"/> is true.
        /// </summary>
        /// <seealso cref="IAlerts"/>
        public void AlertUnnotifiedChanges(object obj)
        {
            //get validation results
            var isStateValid = ScanState ? IsStateValid(obj) : true;
            var isCodeValid = ScanCode ? IsCodeValid(obj) : true;
            //alert
            if (isStateValid && isCodeValid) return;
            if (!this.CanAlert) return;
            var message = new InjectionMessage(!isStateValid, !isCodeValid, obj);
            _alerter.Alert(message, this.AlertChannel);
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
            var lastStamp = GetLastStateStampFor(obj);
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

        private void SaveStateStampFor(object obj)
        {
            var stateId = GetStateId(obj);
            var stateStamp = _stateStamper.GetStamp(obj);
            _stateStampsDictionary.AddOrUpdate(stateId, stateStamp, (key, existingVal) => stateStamp);
        }

        /// <exception cref="ArgumentException">
        ///     Please validate the object using <see cref="NotifyChanges" /> method before
        ///     requesting a code stamp.
        /// </exception>
        private IStamp<int> GetLastCodeStampFor(Type type)
        {
            var id = GetCodeId(type);
            IStamp<int> stamp;
            var valueExists = CodeStampsDictionary.TryGetValue(id, out stamp);
            if (!valueExists)
                throw new ArgumentException(
                    $"Please validate the object using {nameof(NotifyChanges)} method before requesting a code stamp.");
            return stamp;
        }

        /// <exception cref="ArgumentException">
        ///     Please validate the object using <see cref="NotifyChanges" /> method before
        ///     requesting a state stamp.
        /// </exception>
        private IStamp<int> GetLastStateStampFor(object obj)
        {
            var id = GetStateId(obj);
            IStamp<int> stamp;
            var valueExists = _stateStampsDictionary.TryGetValue(id, out stamp);
            if (!valueExists)
                throw new ArgumentException(
                    $"Please validate the object using {nameof(NotifyChanges)} method before requesting a state stamp");
            return stamp;
        }

        private long GetStateId(object obj) => _objectIdGenerator.GetStateId(obj);
        private string GetCodeId(Type type) => _typeKeyGenerator.Generate(type);

        #region [IAlerts]

        private InjectionAlertChannel _alertChannel;

        /// <summary>
        ///     Gets or sets the alert channel.
        /// </summary>
        /// <seealso cref="IAlerts" />
        /// <seealso cref="IInjectionDetector" />
        /// <seealso cref="CanAlert" />
        /// <value>The alert channel.</value>
        public virtual InjectionAlertChannel AlertChannel
        {
            get { return _alertChannel; }
            set { _alertChannel = value; }
        }

        /// <summary>
        ///     Returns whether this <see cref="InjectionDetector" /> instance tracks objects (see: <see cref="ScanCode" />,
        ///     <see cref="ScanState" />)
        /// </summary>
        /// <seealso cref="IAlerts" />
        /// <seealso cref="IInjectionDetector" />
        /// <seealso cref="AlertChannel" />
        /// <value>If this instance is allowed to alert.</value>
        public bool CanAlert => ScanCode || ScanState;

        #endregion

        #region [IDisposable]

        private bool _isDisposed; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
                _stateStampsDictionary.Clear();
            //unmanaged resources
            _stateStampsDictionary = null;
            _isDisposed = true;
        }

        ~InjectionDetector()
        {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}