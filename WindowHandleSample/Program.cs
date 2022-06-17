using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Win32Interop.WinHandles;
using Win32Interop.WinHandles.GetListViewItem;

namespace WindowHandleSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            method3();
            Console.ReadLine();
        }

        static void method1()
        {
            var windows = TopLevelWindowUtils.FindWindows(w => w.IsValid);
            var cmds = windows
                .Where(w => w.GetWindowText().Contains("テキスト ウィンドウ"))
                .FindChildWindows(w => w.GetClassName() == "AfxFrameOrView140u").FirstOrDefault()
                .FindChildWindows(w => w.GetWindowText() == "Marin").FirstOrDefault()
                .FindChildWindows(w => true)
                .SelectMany(s => s)
                .Where(w => w.IsValid);
            var cmdLine = cmds.Where(w => w.GetWindowText() == "Headlands").FirstOrDefault();
            var cmdHist = cmds.Where(w => w.GetWindowText() == "MountTam").FirstOrDefault().GetWindowStr();

            //Console.WriteLine(cmdHist);
            Console.WriteLine(cmdLine.GetWindowStr());
            //  var cmdLine = cmds.Where(w => w.GetWindowText() == "Headlands").FirstOrDefault();

            var mainWindow = windows
                .Where(w => w.GetWindowText().Contains("Autodesk"))
                .Where(w => !w.GetWindowText().Contains("テキスト ウィンドウ"))
                .FirstOrDefault();
            mainWindow.SendHwndCommand("select ");
            mainWindow.SendHwndCommand("all  ");
            mainWindow.SendHwndCommand("filter ");
            windows = TopLevelWindowUtils.FindWindows(w => w.IsValid);
            Thread.Sleep(100);
            var cmbs = windows.
                Where(w => w.GetWindowText() == "オブジェクト選択フィルタ")
                .FindChildWindows(w => w.GetClassName() == "ComboBox")
                .SelectMany(s => s)
                .ToList();

            while (cmbs.Count == 0)
            {
                Thread.Sleep(10);
                windows = TopLevelWindowUtils.FindWindows(w => w.IsValid);
                cmbs = windows.
                    Where(w => w.GetWindowText() == "オブジェクト選択フィルタ")
                    .FindChildWindows(w => w.GetClassName() == "ComboBox")
                    .SelectMany(s => s)
                    .ToList();
            }

            var tbxs = windows.Where(w => w.GetWindowText() == "オブジェクト選択フィルタ")
                .FindChildWindows(w => w.GetClassName() == "Edit")
                .SelectMany(s => s)
                .ToList();

            var btns = windows.Where(w => w.GetWindowText() == "オブジェクト選択フィルタ")
                .FindChildWindows(w => w.GetClassName() == "Button")
                .SelectMany(s => s)
                .ToList();
            if (cmbs.Count >= 4)
            {
                btns.Where(b => b.GetWindowText() == "リストをクリア(&C)").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("ブロック");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("** AND   開始");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("ブロックの位置");
                cmbs[1].SelectCbItem(">");
                cmbs[2].SelectCbItem(">");
                cmbs[3].SelectCbItem("=");
                tbxs[0].SetText("3896");
                tbxs[1].SetText("2219");
                tbxs[2].SetText("0");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("ブロックの位置");
                cmbs[1].SelectCbItem("<");
                cmbs[2].SelectCbItem("<");
                cmbs[3].SelectCbItem("=");
                tbxs[0].SetText("3897");
                tbxs[1].SetText("2220");
                tbxs[2].SetText("0");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("** AND   終了");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();
                btns.Where(b => b.GetWindowText() == "適用(&A)").FirstOrDefault().ClickButton();
            }
            
        }
        
        static void method2()
        {

            var windows = TopLevelWindowUtils.FindWindows(w => w.IsValid);
            var mainWindow = windows
                .Where(w => w.GetWindowText().Contains("Autodesk"))
                .Where(w => !w.GetWindowText().Contains("テキスト ウィンドウ"))
                .FirstOrDefault();
            mainWindow.SendHwndCommand("eattedit ");

            var attrListView = windows
                .Where(w => w.GetWindowText().Contains("拡張属性編集")).FirstOrDefault()
                .FindChildWindows(w => w.IsValid)
                .Where(w => w.GetClassName().Contains("#"))
                .FindChildWindows(w => w.GetClassName() == "SysListView32")
                .SelectMany(s=>s)
                .Where(w => w.IsValid).FirstOrDefault();
            while (attrListView.IsValid != true)
            {
                windows = TopLevelWindowUtils.FindWindows(w => w.IsValid);
                attrListView = windows
                    .Where(w => w.GetWindowText().Contains("拡張属性編集")).FirstOrDefault()
                    .FindChildWindows(w => w.IsValid)
                    .Where(w => w.GetClassName().Contains("#"))
                    .FindChildWindows(w => w.GetClassName() == "SysListView32")
                    .SelectMany(s => s)
                    .Where(w => w.IsValid).FirstOrDefault();
                Thread.Sleep(50);
            }
            var txt = ListViewHandle.GetItemText(attrListView.RawPtr, 0, 0);
            var num = ListViewHandle.Count(attrListView.RawPtr);
            ListViewHandle.SelectRow(attrListView.RawPtr, 5);
            Console.WriteLine($"text{txt}num{num}");

            Console.WriteLine($"select nameplate");
            attrListView.SelectListViewItem("NAME_PLATE");
        }
        static void method3()
        {
            var attrListView = TopLevelWindowUtils
                .FindWindows(w => w.IsValid)
                .Where(w => w.GetWindowText().Contains("拡張属性編集")).FirstOrDefault()
                .FindChildWindows(w => w.IsValid)
                .Where(w => w.GetClassName().Contains("#"))
                .FindChildWindows(w => w.GetClassName() == "SysListView32")
                .SelectMany(s => s)
                .Where(w => w.IsValid).FirstOrDefault();
            attrListView.SelectListViewItem("ITEM_NO.");
        }
    }
}
