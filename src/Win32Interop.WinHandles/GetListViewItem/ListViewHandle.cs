// https://github.com/KOZ60/Samples/blob/master/NET/GetListViewItem/GetListViewItem/Program.cs

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Win32Interop.WinHandles.GetListViewItem
{
    public partial class ListViewHandle
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct LVITEM32
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public uint pszText;
            public int cchTextMax;
            public int iImage;
            public int lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns;
            public int puColumns;
            public int piColFmt;
            public int iGroup;
        }

        // 64bit用
        [StructLayout(LayoutKind.Sequential)]
        public struct LVITEM64
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public int alignment1;
            public ulong pszText;
            public int cchTextMax;
            public int iImage;
            public int alignment2;
            public ulong lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns;
            public int alignment3;
            public ulong puColumns;
            public ulong piColFmt;
            public int iGroup;
            public int alignment4;
        }

        private const int LVIF_TEXT = 0x0001;
        private const int LVM_GETITEMTEXTA = (0x1000 + 45);
        private const int LVM_GETITEMTEXTW = (0x1000 + 115);
        private const int MAX_LVMSTRING = 255;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr Iparam);

        [DllImport("user32.dll")]
        private static extern bool IsWindowUnicode(IntPtr hWnd);

        public static string GetItemText(IntPtr hwnd, int item, int subItem)
        {

            using (var hProcess = new RemoteProcessHandle(hwnd))
            {
                int offset = hProcess.Is32Bit ? Marshal.SizeOf(typeof(LVITEM32)) : Marshal.SizeOf(typeof(LVITEM64));
                int cbSize = (MAX_LVMSTRING + 1) * 2;

                using (var remoteMem = new RemoteMemoryHandle(hProcess, offset + cbSize))
                {
                    if (hProcess.Is32Bit)
                    {
                        var lvitem = new LVITEM32();
                        lvitem.mask = LVIF_TEXT;
                        lvitem.pszText = (uint)(remoteMem.Address + offset);
                        lvitem.cchTextMax = MAX_LVMSTRING;
                        lvitem.iItem = item;
                        lvitem.iSubItem = subItem;

                        remoteMem.WriteTo(ref lvitem);
                    }
                    else
                    {
                        var lvitem = new LVITEM64();
                        lvitem.mask = LVIF_TEXT;
                        lvitem.pszText = (ulong)(remoteMem.Address + offset);
                        lvitem.cchTextMax = MAX_LVMSTRING;
                        lvitem.iItem = item;
                        lvitem.iSubItem = subItem;

                        remoteMem.WriteTo(ref lvitem);
                    }

                    bool isUnicode = IsWindowUnicode(hwnd);
                    int LVM_GETITEMTEXT = isUnicode ? LVM_GETITEMTEXTW : LVM_GETITEMTEXTA;
                    IntPtr result = SendMessage(hwnd, LVM_GETITEMTEXT, new IntPtr(item), remoteMem.Address);
                    if (result == IntPtr.Zero)
                    {
                        throw new Win32Exception();
                    }
                    using (var m = new CoTaskMem(cbSize))
                    {
                        remoteMem.ReadFrom(m.Address, offset, cbSize);
                        if (isUnicode)
                        {
                            return Marshal.PtrToStringUni(m.Address);
                        }
                        else
                        {
                            return Marshal.PtrToStringAnsi(m.Address);
                        }
                    }
                }
            }
        }
    }
}
