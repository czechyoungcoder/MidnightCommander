using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander
{
    internal abstract class PopupWindow : Window
    {
        protected Size size;
        protected string? path;
        protected ConsoleColor background { get; set; } = ConsoleColor.White;
        protected ConsoleColor foreground { get; set; } = ConsoleColor.Black;
        protected int dataWidth { get; set; }
        protected int padLeft { get; set; }
        protected List<IElement> selectables = new List<IElement>();
        protected int activeElement = 0;
        public event Action? Confirmed;

        public PopupWindow(string path, Size size)
        {
            this.path = path;
            this.size = size;
            dataWidth = size.width - 6;
        }

        protected void UpdateTables()
        {
            Confirmed!.Invoke();
        }

        public void Button_Cancel()
        {
            Application.PopWindow();
        }

        public void ChangeColors(ConsoleColor bg, ConsoleColor fg)
        {
            background = bg;
            foreground = fg;
        }

        protected void ChangeActiveElement()
        {
            activeElement = ++activeElement >= selectables.Count ? 0 : activeElement;
            for (int btn = 0; btn < selectables.Count; btn++)
            {
                selectables[btn].Active = btn == activeElement;
            }
        }

        protected void DrawText(string data)
        {
            Console.Write(data);
            NewLine();
        }

        protected void DrawBorders(string header)
        {
            DrawTop(header);
            for (int i = 0; i < size.height - 4; i++)
            {
                Console.Write(" │".PadRight(size.width - 2) + "│ ");
                NewLine();
            }
            DrawBottom();
            padLeft = padLeft + 3;
            Console.SetCursorPosition(padLeft, (Console.WindowHeight / 2 - size.height / 2) + 2);
        }

        protected void DrawTop(string header)
        {
            Console.Write(" ".PadRight(size.width));
            NewLine();
            Console.Write(" ┌".PadRight(size.width - 2, '─') + "┐ ");
            Console.SetCursorPosition(Console.WindowWidth / 2 - header.Length / 2, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(" " + header + " ");
            Console.ForegroundColor = foreground;
            NewLine();
        }
        protected void DrawBottom()
        {
            Console.Write(" └".PadRight(size.width - 2, '─') + "┘ ");
            NewLine();
            Console.Write(" ".PadRight(size.width));
        }

        protected void NewLine()
        {
            Console.SetCursorPosition(padLeft, Console.CursorTop + 1);
        }

        protected void DrawButtons()
        {
            List<IElement> buttons = selectables.Where(el => el is Button).ToList();
            int sizeToPad = Console.WindowWidth / 2 - ((buttons.Count-1) * 2);
            foreach (Button button in buttons)
            {
                sizeToPad -= (button.Name.Length + 6) / 2; 
            }
            Console.SetCursorPosition(sizeToPad, Console.CursorTop);
            foreach (Button button in buttons)
            {
                button.Draw();
                Console.Write("   ");
            }
        }

        protected void BreakLine()
        {
            Console.SetCursorPosition(padLeft - 2, Console.CursorTop);
            Console.Write("├".PadRight(dataWidth + 3, '─') + "┤");
            NewLine();
        }
        
        protected void SetCursor()
        {
            this.padLeft = Console.WindowWidth / 2 - size.width / 2;
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.SetCursorPosition(padLeft, Console.WindowHeight / 2 - size.height / 2);
        }

        public abstract override void HandleKey(ConsoleKeyInfo info);

        public abstract override void Draw();
       
    }
}
