using System;
using System.Runtime.InteropServices;
using Dapplo.Windows.Common.Enums;
using Dapplo.Windows.Common.Extensions;

namespace pdfforge.PDFCreator.Conversion.Actions.AttachToOutlookItem
{
    internal class OleAuth32Api
    {
        /// <summary>
        /// This provides an API for OLE32
        /// </summary>
        public static class Ole32Api
        {
            /// <summary>
            /// This converts a ProgID (program ID) into a Guid with the clsId
            /// </summary>
            /// <param name="programId">string with the program ID</param>
            /// <returns>Guid with the clsId</returns>
            public static Guid ClassIdFromProgId(string programId)
            {
                if (CLSIDFromProgID(programId, out Guid clsId).Succeeded())
                {
                    return clsId;
                }

                return clsId;
            }

            /// <summary>
            /// See more <a href="https://docs.microsoft.com/en-us/windows/desktop/api/combaseapi/nf-combaseapi-clsidfromprogid">here</a>
            /// </summary>
            /// <param name="progId">string with the progId</param>
            /// <param name="clsId">Guid</param>
            /// <returns>HResult</returns>
            [DllImport("ole32.dll", ExactSpelling = true)]
            private static extern HResult CLSIDFromProgID([In][MarshalAs(UnmanagedType.LPWStr)] string progId, [Out] out Guid clsId);
        }


        /// <summary>
        /// API for OLEAUT32
        /// </summary>
        public static class OleAut32Api
        {
            /// <summary>
            /// Get the active instance of the com object with the specified GUID
            /// </summary>
            /// <typeparam name="T">Type for the instance</typeparam>
            /// <param name="clsId">Guid</param>
            /// <returns>IDisposableCom of T</returns>
            public static DisposableCom.IDisposableCom<T> GetActiveObject<T>(ref Guid clsId)
            {
                if (GetActiveObject(ref clsId, IntPtr.Zero, out object comObject).Succeeded())
                {
                    return DisposableCom.Create((T)comObject);
                }

                return null;
            }

            /// <summary>
            /// Get the active instance of the com object with the specified progId
            /// </summary>
            /// <typeparam name="T">Type for the instance</typeparam>
            /// <param name="progId">string</param>
            /// <returns>IDisposableCom of T</returns>
            public static DisposableCom.IDisposableCom<T> GetActiveObject<T>(string progId)
            {
                var clsId = Ole32Api.ClassIdFromProgId(progId);
                return GetActiveObject<T>(ref clsId);
            }

            /// <summary>
            /// For more details read <a href="https://docs.microsoft.com/en-gb/windows/desktop/api/oleauto/nf-oleauto-getactiveobject">this</a>
            /// </summary>
            /// <param name="rclsId">The class identifier (CLSID) of the active object from the OLE registration database.</param>
            /// <param name="pvReserved">Reserved for future use. Must be null.</param>
            /// <param name="ppunk">The requested active object.</param>
            /// <returns></returns>
            [DllImport("oleaut32.dll")]
            private static extern HResult GetActiveObject(ref Guid rclsId, IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);
        }
    }
}
