using System;

namespace SafeOrbit.Common
{
    /// <summary>
    /// Abstract base class to implement IDisposable interface.
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;


        /// <remarks>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </remarks>
        ~DisposableBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        /// <remarks>
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </remarks>
        public void Dispose()
        {
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <exception cref="ObjectDisposedException">Throws if the instance is disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if(IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <remarks>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </remarks>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!IsDisposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    DisposeManagedResources();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
                DisposeUnmanagedResources();
            }
            IsDisposed = true;
        }
        /// <remarks>
        ///  Can be overridden by derived classes to dispose managed resources.
        /// </remarks>
        protected virtual void DisposeManagedResources()
        {
        }

        /// <remarks>
        /// Can be overridden by derived classes to dispose unmanaged resources.
        /// </remarks>
        protected virtual void DisposeUnmanagedResources()
        {
        }
    }
}