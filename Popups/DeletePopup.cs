using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MidnightCommander.Popups;

namespace MidnightCommander
{
    internal class DeletePopup : PopupWindow
    {

        public DeletePopup(string path, Size size) : base(path, size)
        {
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
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
                else
                    File.Delete(path);

                this.Application.PopWindow();
                UpdateTables();
            }
           
            catch (Exception e)
            {
                this.Application.PushWindow(new ErrorPopup(path, "You can't delete this file!", new Size(50, 8)));
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
        }

        public override void Draw()
        {
            string content = Directory.Exists(path) ? "directory" : "file";

            SetCursor();
            DrawBorders(header: "Delete " + content );
            DrawText($"Delete {content} \"{new DirectoryInfo(path).Name}\"?");
            BreakLine();
            DrawButtons();
        }
    }
}
