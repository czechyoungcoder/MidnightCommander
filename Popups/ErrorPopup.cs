using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander.Popups
{
    internal class ErrorPopup : PopupWindow
    {
        private string message;
        public ErrorPopup(string path, string message, Size size) : base(path, size)
        {
            this.message = message;
        }


        public override void HandleKey(ConsoleKeyInfo info)
        {
            Application.PopWindow();
        }

        public override void Draw()
        {
            ChangeColors(ConsoleColor.Black, ConsoleColor.Red);
            SetCursor();
            DrawBorders(header: "ERROR!");
            DrawText(message);
            NewLine();
            BreakLine();
            DrawText("Tap any key to try again...");
        }
    }
}
