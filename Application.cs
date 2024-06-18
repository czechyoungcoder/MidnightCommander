using MidnightCommander.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander
{
    internal class Application
    {
        private List<Window> windows = new List<Window>();

        public Application()
        {
            this.PushWindow(new TableWindow(null));
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            GetLastWindow().HandleKey(info);
        }

        public void Draw()
        {
            //foreach (Window window in windows)
            //{
            //    window.Draw();
            //}
            GetLastWindow().Draw();
        }

        public void PushWindow(Window window)
        {
            this.windows.Add(window);
            window.Application = this;
        }

        public void PopWindow()
        {
            this.windows.RemoveAt(this.windows.Count - 1);
        }

        public Window GetLastWindow()
        {
            return this.windows[windows.Count - 1];
        }

        public void DrawMenu(List<string> items)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(1, Console.WindowHeight - 1);
            for (int i = 0; i < items.Count; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(i + 1);
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(items[i].PadRight(7));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                if (i < items.Count - 1)
                    Console.Write("    ");
            }
        }
    }


}
