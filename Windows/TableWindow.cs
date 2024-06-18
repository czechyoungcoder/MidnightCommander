using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MidnightCommander.Windows;

namespace MidnightCommander
{
    internal class TableWindow : Window
    {
        private Table[] tables = new Table[2];
        private int active { get; set; } = 0;
        private int tableWidth { get; set; }

        public TableWindow(string[]? paths)
        {
            this.tableWidth = Console.WindowWidth / 2;
            CreateTables(paths);
        }

        private void CreateTables(string[]? paths)
        {
            for (int i = 0; i < 2; i++)
            {
                string path = paths is null ? @"C:\" : paths[i];
                Table table = new Table(path, new int[2] { i * tableWidth, 1 });
                table.SwitchTable += Table_SwitchTable;
                table.Popup += Table_Popup;
                table.EditFile += Table_EditFile;
                tables[i] = table;
            }
            tables[0].Active = true;
        }


        private void Table_EditFile(string path)
        {
            this.Application.PushWindow(new EditWindow(path));
        }

        private void Popup_UpdateTables()
        {
            foreach (Table table in tables)
                table.UpdateData();
        }

        private void Table_Popup(string path, int popup)
        {
            string destPath = this.tables.Where(t => !t.Active).ToArray()[0].Dir;
            PopupWindow popupWindow;

            switch (popup)
            {
                case 5:
                    popupWindow = new CopyPopup(path, destPath, new Size( 80, 11));
                    break;
                case 6:
                    popupWindow = new MovePopup(path, destPath, new Size( 80, 11 ));
                    break;
                case 7:
                    popupWindow = new MkdirPopup(path, new Size(50, 8 ));
                    break;
                case 8:
                    popupWindow = new DeletePopup(path, new Size(50, 7));
                    break;
                default:
                    popupWindow = new DeletePopup(path, new Size(50, 7));
                    break;
            }

            popupWindow.Confirmed += Popup_UpdateTables;
            this.Application.PushWindow(popupWindow);
        }

        private void Table_SwitchTable()
        {
            active = ++active >= 2 ? 0 : active;
            for (int i = 0; i < 2; i++)
            {
                this.tables[i].Active = active == i;
            }
        }

        public override void Draw()
        {
            DrawHeader();
            foreach (Table table in this.tables)
            {
                table.Draw();
            }
            Application.DrawMenu(new List<string>() { "Help", "Menu", "View", "Edit", "Copy", "RenMov", "Mkdir", "Delete", "PullDn", "Quit" });
        }

        public void DrawHeader()
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(" Left    File    Command    Options    Right".PadRight(Console.WindowWidth - 1));
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            this.tables[active].HandleKey(info);
        }
    }
}
