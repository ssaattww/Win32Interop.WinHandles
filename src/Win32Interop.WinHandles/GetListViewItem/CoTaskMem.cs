// https://github.com/KOZ60/Samples/blob/master/NET/GetListViewItem/GetListViewItem/CoTaskMem.cs

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Win32Interop.WinHandles.GetListViewItem
{
    internal class CoTaskMem : SafeHandleZeroOrMinusOneIsInvalid
    {
        public CoTaskMem(int cbSize) : base(true)
        {
            SetHandle(Marshal.AllocCoTaskMem(cbSize));
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeCoTaskMem(handle);
            return true;
        }

        public IntPtr Address
        {
            get
            {
                return handle;
            }
        }
    }
}
