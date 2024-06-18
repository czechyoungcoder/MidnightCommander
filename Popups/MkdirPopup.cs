using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MidnightCommander.Popups;

namespace MidnightCommander
{
    internal class MkdirPopup : PopupWindow
    {
        private Input input;
        public MkdirPopup(string path, Size size) : base(path, size)
        {
            input = new Input(dataWidth) { Active = true};
            Button OKBtn = new Button("OK") { Active = true };
            Button CancelBtn = new Button("Cancel");
            OKBtn.Selected += Button_OK;
            CancelBtn.Selected += Button_Cancel;
            selectables.Add(OKBtn);
            selectables.Add(CancelBtn);
        }

        public void Button_OK()
        {
            try
            {
                string newPath = @$"{path}\{input.Text}";
                DirectoryInfo dir = new DirectoryInfo(newPath);
                dir.Create();
                this.Application.PopWindow();
                UpdateTables();
            }
            catch(Exception e)
            {
                this.Application.PushWindow(new ErrorPopup(path, "You've entered an invalid path!", new Size(50, 8)));
            }


        }
        
        public override void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Tab)
            {
                ChangeActiveElement();
            }
            else if (info.Key == ConsoleKey.Enter)
            {
                selectables[activeElement].HandleKey(info);
            }
            else
            {
                input.HandleKey(info);
            }
        }

        public override void Draw()
        {
            SetCursor();

            DrawBorders(header: "Create a new Directory");
            DrawText("Enter directory name: ");
            input.Draw();
            NewLine();
            NewLine();

            DrawButtons();
        }
    }
}
