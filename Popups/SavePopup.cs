using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander.Popups
{
    internal class SavePopup : PopupWindow
    {
        private List<string> lines;
        private string path;
        public SavePopup(string path, List<string> lines, Size size) : base(path, size)
        {
            this.lines = lines;
            this.path = path;
            Button OKBtn = new Button("Yes") { Active = true };
            Button NoBtn = new Button("No");
            Button CancelBtn = new Button("Cancel");
            OKBtn.Selected += Button_OK;
            NoBtn.Selected += Button_No;
            CancelBtn.Selected += Button_Cancel;
            selectables.Add(OKBtn);
            selectables.Add(NoBtn);
            selectables.Add(CancelBtn);
        }

        public void Button_OK()
        {
            FileService.SaveFile(path, lines);
            Application.PopWindow();
            Application.PopWindow();
        }

        public void Button_No()
        {
            Application.PopWindow();
            Application.PopWindow();
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
        }

        public override void Draw()
        {
            SetCursor();
            DrawBorders(header: "File modified");
            DrawText("Do you want to save changes?");
            BreakLine();
            DrawButtons();
        }
    }
}
