using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidnightCommander.Popups;

namespace MidnightCommander
{
    internal class MovePopup : PopupWindow
    {
        private string sourcePath;
        private Input sourceInput;
        private Input destInput;

        public MovePopup(string sourcePath, string destPath, Size size) : base(sourcePath, size)
        {
            this.sourcePath = sourcePath;
            sourceInput = new Input(dataWidth) { Text = sourcePath };
            destInput = new Input(dataWidth) { Text = Path.Combine(destPath, new DirectoryInfo(sourcePath).Name), Active = true };
            Button OKBtn = new Button("OK") { Active = true };
            Button CancelBtn = new Button("Cancel");
            OKBtn.Selected += Button_OK;
            CancelBtn.Selected += Button_Cancel;
            selectables.Add(OKBtn);
            selectables.Add(CancelBtn);
        }

        public void Button_OK()
        {
            string source = sourceInput.Text;
            string dest = destInput.Text;
            try
            {
                if (Directory.Exists(source))
                {
                    FileService.CopyFolder(source, dest);
                    Directory.Delete(source, true);
                }
                else
                {
                    File.Copy(source, dest);
                    File.Delete(source);
                }

                this.Application.PopWindow();
                UpdateTables();
            }
            catch (Exception e)
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
                destInput.HandleKey(info);
            }
        }

        public override void Draw()
        {
            string content = Directory.Exists(sourcePath) ? "directory" : "file";

            SetCursor();
            DrawBorders(header: "Move " + content);
            DrawText($"Move {content} \"{new DirectoryInfo(sourcePath).Name}\"");
            DrawText("From");
            sourceInput.Draw();
            NewLine();
            DrawText("To");
            destInput.Draw();
            NewLine();
            NewLine();
            DrawButtons();
        }
    }
}
