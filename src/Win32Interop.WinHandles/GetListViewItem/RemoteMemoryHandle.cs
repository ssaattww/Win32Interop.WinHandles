﻿// https://github.com/KOZ60/Samples/blob/master/NET/GetListViewItem/GetListViewItem/RemoteMemoryHandle.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;


namespace Win32Interop.WinHandles.GetListViewItem
{
    internal class RemoteMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private RemoteProcessHandle _processHandle;

        private const int PAGE_NOACCESS = 0x01;
        private const int PAGE_READWRITE = 0x04;

        private const int MEM_COMMIT = 0x1000;
        private const int MEM_RELEASE = 0x8000;
        private const int MEM_FREE = 0x10000;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(RemoteProcessHandle hProcess, IntPtr address, IntPtr size, int allocationType, int protect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFreeEx(RemoteProcessHandle hProcess, IntPtr address, IntPtr size, int freeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(
                RemoteProcessHandle hProcess,
                IntPtr lpBaseAddress,
                IntPtr lpBuffer,
                IntPtr nSize,
                out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
                RemoteProcessHandle hProcess,
                IntPtr lpBaseAddress,
                IntPtr lpBuffer,
                IntPtr dwSize,
                out IntPtr lpNumberOfBytesRead);

        public RemoteMemoryHandle(RemoteProcessHandle hProcess, int cbSize) : base(true)
        {
            _processHandle = hProcess;
            SetHandle(VirtualAllocEx(_processHandle, IntPtr.Zero, new IntPtr((uint)cbSize), MEM_COMMIT, PAGE_READWRITE));
        }

        protected override bool ReleaseHandle()
        {
            return VirtualFreeEx(_processHandle, handle, IntPtr.Zero, MEM_RELEASE);
        }

        public IntPtr Address
        {
            get
            {
                return handle;
            }
        }

        public void WriteTo(IntPtr sourceAddress, int cbSize)
        {
            WriteTo(sourceAddress, 0, cbSize);
        }

        public void WriteTo(IntPtr sourceAddress, int offset, int cbSize)
        {
            IntPtr count;
            WriteProcessMemory(_processHandle, handle + offset, sourceAddress, new IntPtr(cbSize), out count);
        }

        public void ReadFrom(IntPtr destAddress, int cbSize)
        {
            ReadFrom(destAddress, 0, cbSize);
        }

        public void ReadFrom(IntPtr destAddress, int offset, int cbSize)
        {
            IntPtr count;
            ReadProcessMemory(_processHandle, handle + offset, destAddress, new IntPtr(cbSize), out count);
        }

        public void WriteTo<T>(ref T structure) where T : struct
        {
            WriteTo(ref structure, 0);
        }

        public void WriteTo<T>(ref T structure, int offset) where T : struct
        {
            int cbSize = Marshal.SizeOf(structure);
            using (var m = new CoTaskMem(cbSize))
            {
                Marshal.StructureToPtr(structure, m.Address, false);
                WriteTo(m.Address, offset, cbSize);
            }
        }

        public T ReadFrom<T>() where T : struct
        {
            return ReadFrom<T>(0);
        }

        public T ReadFrom<T>(int offset) where T : struct
        {
            int cbSize = Marshal.SizeOf(typeof(T));
            using (var m = new CoTaskMem(cbSize))
            {
                ReadFrom(m.Address, offset, cbSize);
                return (T)Marshal.PtrToStructure(m.Address, typeof(T));
            }
        }

    }
}
