using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander
{
    internal class Input : IElement
    {
        private int width;
        public string Text = "";
        public bool Active { get; set; } 

        public Input(int width)
        {
            this.width = width;
        }

        public void Draw()
        {
            if (Active)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
            }
            Console.Write(Text.PadRight(width));

            Console.BackgroundColor = ConsoleColor.White;

        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (Text.Length == width) return;

            if (info.Key == ConsoleKey.Backspace)
                Text = Text.Length > 0 ? Text.Substring(0, Text.Length - 1) : Text;
            
            else if (info.Key == ConsoleKey.Spacebar)
            {
                Text += " ";
            }

            else if (info.Key == ConsoleKey.Delete)
            {
                Text = "";
            }

            else if (Char.IsAscii((char)info.KeyChar))
            {
                Text += info.KeyChar;
            }
            
        }
    }
}
