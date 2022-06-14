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
            var cmds = windows
                .Where(w => w.GetWindowText().Contains("テキスト ウィンドウ"))
                .FindChildWindows(w => w.GetClassName() == "AfxFrameOrView140u").FirstOrDefault()
                .FindChildWindows(w => w.GetWindowText() == "Marin").FirstOrDefault()
                .FindChildWindows(w => true)
                .SelectMany(s => s)
                .Where(w => w.IsValid);
            var cmdLine = cmds.Where(w => w.GetWindowText() == "Headlands").FirstOrDefault().GetWindowStr();
            var cmdHist = cmds.Where(w => w.GetWindowText() == "MountTam").FirstOrDefault().GetWindowStr();
            
            Console.WriteLine(cmdHist);
            Console.WriteLine(cmdLine);

            Console.ReadLine();

        }
    }
}
