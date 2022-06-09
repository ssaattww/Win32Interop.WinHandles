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
            var windows = TopLevelWindowUtils.FindWindows(w => true);
            var window_texts = windows.
                Where(w => w.IsValid && w.GetWindowText().Contains("Auto"))
                .FindChildWindows(w => true)
                .SelectMany(s =>s)
                .Where(w => w.IsValid)
                .Select(w => (w.GetWindowText(), w.GetClassName()));
            foreach (var window_text in window_texts)
            {
                Console.Write(window_text.Item1 + "\t");
                Console.WriteLine(window_text.Item2);
            }
            Console.ReadLine();

        }
    }
}
