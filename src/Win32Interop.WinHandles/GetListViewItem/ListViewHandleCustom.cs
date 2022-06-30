using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Win32Interop.WinHandles.GetListViewItem
{
    public partial class ListViewHandle
    {
        private const int LVM_GETITEMCOUNT = (0x1000 + 4);
        private const int LVM_SETITEMSTATE = (0x1000 + 43);
        private const int LVM_GETITEMSTATE = (0x1000 + 44);
        private const int LVM_GETSELECTEDCOUNT = (0x1000 + 50);

        private const int LVIS_FOCUSED = 0x0001;
        private const int LVIS_SELECTED = 0x0002;

        public static int Count(IntPtr intPtr) => SendMessage(intPtr, LVM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero).ToInt32();

        /// <summary>
        /// 0始まり
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="rowNo"></param>
        /// <exception cref="Win32Exception"></exception>
        public static void SelectRow(IntPtr hwnd, int rowNo)
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
                        lvitem.stateMask = LVIS_SELECTED;
                        lvitem.state = LVIS_SELECTED;
                        remoteMem.WriteTo(ref lvitem);
                    }
                    else
                    {
                        var lvitem = new LVITEM64();
                        lvitem.stateMask = LVIS_SELECTED;
                        lvitem.state = LVIS_SELECTED;

                        remoteMem.WriteTo(ref lvitem);
                    }

                    IntPtr result = SendMessage(hwnd, LVM_SETITEMSTATE, new IntPtr(rowNo), remoteMem.Address);
                    if (result == IntPtr.Zero)
                    {
                        throw new Win32Exception();
                    }
                }
            }
        }

        public static bool IsSelectedRow(IntPtr hwnd, int rowNo)
        {
            return SendMessage(hwnd, LVM_GETITEMSTATE, new IntPtr(rowNo), new IntPtr(LVIS_SELECTED)) != IntPtr.Zero;
        }
    }
}
