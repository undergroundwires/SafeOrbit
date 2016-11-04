/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Concurrent;
using SafeOrbit.Common.Reflection;
using SafeOrbit.Memory.InjectionServices;
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
    public class InjectionDetector
    {
        private static readonly ConcurrentDictionary<string, IStamp<int>> CodeStampsDictionary =
            new ConcurrentDictionary<string, IStamp<int>>(); //static for caching as types always must be the same.

        private readonly IInjectionAlerter _alerter;

        private readonly IStamper<Type> _codeStamper;
        private readonly IStamper<object> _stateStamper;


        private readonly ITypeIdGenerator _typeIdGenerator;

        private IStamp<int> _lastStateStamp;

        public InjectionDetector(bool justCode = true, bool justState = true,
            InjectionAlertChannel alertChannel = Defaults.AlertChannel) : this
        (
            InjectionAlerter.StaticInstance,
            TypeIdGenerator.StaticInstance,
            StateStamper.StaticInstance, IlCodeStamper.StaticInstance, justCode, justState, alertChannel)
        {
        }

        internal InjectionDetector(
            IInjectionAlerter alerter,
            ITypeIdGenerator typeIdGenerator,
            IStamper<object> stateStamper,
            IStamper<Type> codeStamper,
            bool scanCode, bool scanState,
            InjectionAlertChannel alertChannel)
        {
            if (alerter == null) throw new ArgumentNullException(nameof(alerter));
            if (stateStamper == null) throw new ArgumentNullException(nameof(stateStamper));
            if (codeStamper == null) throw new ArgumentNullException(nameof(codeStamper));
            _alerter = alerter;
            _typeIdGenerator = typeIdGenerator;
            _stateStamper = stateStamper;
            _codeStamper = codeStamper;
            _alertChannel = alertChannel;
            ScanCode = scanCode;
            ScanState = scanState;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether <see cref="InjectionDetector" /> will keep track of the state of the
        ///     objects.
        /// </summary>
        public bool ScanState { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether <see cref="InjectionDetector" /> will keep track of the code of the
        ///     objects.
        /// </summary>
        public bool ScanCode { get; set; }

        /// <summary>
        ///     Saves the state and/or the code  of the object.
        ///     Use <see cref="AlertUnnotifiedChanges" /> method to check if the state has been injected.
        /// </summary>
        public void NotifyChanges(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (ScanState)
                SaveStateStamp(obj);
            if (ScanCode)
                SaveCodeStampFor(obj.GetType());
        }

        /// <summary>
        ///     Alerts when any unnotified changes are detected any <see cref="CanAlert" /> is true.
        /// </summary>
        /// <seealso cref="IAlerts" />
        public void AlertUnnotifiedChanges(object obj)
        {
            //get validation results
            var isStateValid = ScanState ? IsStateValid(obj) : true;
            var isCodeValid = ScanCode ? IsCodeValid(obj) : true;
            //alert
            if (isStateValid && isCodeValid) return;
            if (!CanAlert) return;
            var message = new InjectionMessage(!isStateValid, !isCodeValid, obj);
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
            IStamp<int> stamp;
            var valueExists = CodeStampsDictionary.TryGetValue(id, out stamp);
            if (!valueExists)
                throw new ArgumentException(
                    $"Please validate the object using {nameof(NotifyChanges)} method before requesting a code stamp.");
            return stamp;
        }


        private string GetCodeId(Type type) => _typeIdGenerator.Generate(type);

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
    }
}