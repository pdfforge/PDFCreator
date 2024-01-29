using System;
using System.Runtime.InteropServices;

namespace pdfforge.PDFCreator.Conversion.Actions.AttachToOutlookItem
{
    public static class DisposableCom
    {
        /// <summary>
        ///     Create a ComDisposable for the supplied type object
        /// </summary>
        /// <typeparam name="T">Type for the com object</typeparam>
        /// <param name="comObject">the com object itself</param>
        /// <returns>IDisposableCom of type T</returns>
        public static IDisposableCom<T> Create<T>(T comObject)
        {
            if (Equals(comObject, default(T)))
            {
                return null;
            }

            return new DisposableComImplementation<T>(comObject);
        }

        /// <summary>
        ///     A simple com wrapper which helps with "using"
        /// </summary>
        /// <typeparam name="T">Type to wrap</typeparam>
        public interface IDisposableCom<out T> : IDisposable
        {
            /// <summary>
            ///     The actual com object
            /// </summary>
            T ComObject { get; }
        }

        /// <summary>
        ///     Implementation of the IDisposableCom, this is internal to prevent other code to use it directly
        /// </summary>
        /// <typeparam name="T">Type of the com object</typeparam>
        private class DisposableComImplementation<T> : IDisposableCom<T>
        {
            public DisposableComImplementation(T obj)
            {
                ComObject = obj;
            }

            public T ComObject { get; private set; }

            /// <summary>
            ///     Cleans up the COM object.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            ///     Release the COM reference
            /// </summary>
            /// <param name="disposing"><see langword="true" /> if this was called from the<see cref="IDisposable" /> interface.</param>
            private void Dispose(bool disposing)
            {
                if (!disposing)
                {
                    return;
                }

                // Do not catch an exception from this.
                // You may want to remove these guards depending on
                // what you think the semantics should be.
                if (!Equals(ComObject, default(T)) && Marshal.IsComObject(ComObject))
                {
                    Marshal.ReleaseComObject(ComObject);
                }

                ComObject = default;
            }
        }
    }
}
