using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32Interop.WinHandles;

namespace WindowHandleSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var windows = TopLevelWindowUtils.FindWindows(w => w.IsValid);
            //var cmds = windows
            //    .Where(w => w.GetWindowText().Contains("テキスト ウィンドウ"))
            //    .FindChildWindows(w => w.GetClassName() == "AfxFrameOrView140u").FirstOrDefault()
            //    .FindChildWindows(w => w.GetWindowText() == "Marin").FirstOrDefault()
            //    .FindChildWindows(w => true)
            //    .SelectMany(s => s)
            //    .Where(w => w.IsValid);
            //var cmdLine = cmds.Where(w => w.GetWindowText() == "Headlands").FirstOrDefault().GetWindowStr();
            //var cmdHist = cmds.Where(w => w.GetWindowText() == "MountTam").FirstOrDefault().GetWindowStr();

            //Console.WriteLine(cmdHist);
            //Console.WriteLine(cmdLine);

            var cmbs = windows.
                Where(w => w.GetWindowText() == "オブジェクト選択フィルタ")
                .FindChildWindows(w => w.GetClassName() == "ComboBox")
                .SelectMany(s => s)
                .ToList();

            var tbxs = windows.Where(w => w.GetWindowText() == "オブジェクト選択フィルタ")
                .FindChildWindows(w => w.GetClassName() == "Edit")
                .SelectMany(s => s)
                .ToList();

            var btns = windows.Where(w => w.GetWindowText() == "オブジェクト選択フィルタ")
                .FindChildWindows(w => w.GetClassName() == "Button")
                .SelectMany(s => s)
                .ToList();
            if(cmbs.Count>=4)
            {
                btns.Where(b => b.GetWindowText() == "リストをクリア(&C)").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("ブロック");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("** AND   開始");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("ブロックの位置");
                cmbs[1].SelectCbItem(">");
                cmbs[2].SelectCbItem(">");
                cmbs[3].SelectCbItem(">");
                tbxs[0].SetText("1.0");
                tbxs[1].SetText("2.0");
                tbxs[2].SetText("3.0");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("ブロックの位置");
                cmbs[1].SelectCbItem("<");
                cmbs[2].SelectCbItem("<");
                cmbs[3].SelectCbItem("<");
                tbxs[0].SetText("3.0");
                tbxs[1].SetText("2.0");
                tbxs[2].SetText("1.0");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();

                cmbs[0].SelectCbItem("** AND   終了");
                btns.Where(b => b.GetWindowText() == "リストに追加(&L):").FirstOrDefault().ClickButton();

            }
            Console.ReadLine();

        }
    }
}
