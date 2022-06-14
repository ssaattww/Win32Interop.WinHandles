using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Win32Interop.WinHandles.Internal;

namespace Win32Interop.WinHandles
{

    /// <summary> Extension methods for <see cref="WindowHandle"/> </summary>
    public static class WindowHandleExtensions
    {

        /// <summary>
        /// SendMessage Constant
        /// </summary>
        private const uint WM_GETTEXT = 0x000D;
        private const uint WM_GETTEXTLENGTH = 0x000E;
        private const uint WM_COPYDATA = 0x004A;
        private const uint WM_NULL = 0x0000;

        /// <summary> Check if the given window handle is currently visible. </summary>
        /// <param name="windowHandle"> The window to act on. </param>
        /// <returns> true if the window is visible, false if not. </returns>
        public static bool IsVisible(this WindowHandle windowHandle)
        {
          return NativeMethods.IsWindowVisible(windowHandle.RawPtr);
        }

        /// <summary> Gets the Win32 class name of the given window. </summary>
        /// <param name="windowHandle"> The window handle to act on. </param>
        /// <returns> The class name of the passed in window. </returns>
        public static string GetClassName(this WindowHandle windowHandle)
        {
            int size = 255;
            int actualSize = 0;
            StringBuilder builder;
            do
            {
                builder = new StringBuilder(size);
                actualSize = NativeMethods.GetClassName(windowHandle.RawPtr, builder, builder.Capacity);
                size *= 2;
            } while (actualSize == size - 1);

            return builder.ToString();
        }

        /// <summary> Gets the text associated with the given window handle. </summary>
        /// <param name="windowHandle"> The window handle to act on. </param>
        /// <returns> The window text. </returns>
        [NotNull]
        public static string GetWindowText(this WindowHandle windowHandle)
        {
            int size = NativeMethods.GetWindowTextLength(windowHandle.RawPtr);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                NativeMethods.GetWindowText(windowHandle.RawPtr, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary>
        /// GetWindowText cannot get textedit`s string. So use this.
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <returns>The text.</returns>
        public static string GetWindowStr(this WindowHandle windowHandle)
        {
            var len = NativeMethods.SendMessage(windowHandle.RawPtr, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
            var msg = new string('\0', len.ToInt32());

            NativeMethods.SendMessage(windowHandle.RawPtr, WM_GETTEXT, len, msg);
            return msg;
        }

        /// <summary>
        /// Put string to the given window
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static bool SendHwndCommand(this WindowHandle windowHandle, string cmd)
        {
            var cds = new COPYDATASTRUCT
            {
                DwData = new IntPtr(1),
                CbData = (uint)((cmd.Length * 2) + 2),
                LpData = cmd,
            };

            var ret = NativeMethods.SendMessage(windowHandle.RawPtr, WM_COPYDATA, IntPtr.Zero, ref cds);
            NativeMethods.PostMessage(windowHandle.RawPtr, WM_NULL, IntPtr.Zero, IntPtr.Zero);

            return ret.ToInt32() == 1;
        }
    }
}