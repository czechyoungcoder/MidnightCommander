using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MidnightCommander
{
    internal class Table
    {
        public event Action SwitchTable;
        public event Action<string> EditFile;
        public event Action<string, int> Popup;

        public string Dir { get; set; }

        private List<TableData> rows { get; set; } = new List<TableData>();

        private int offset = 0;

        public int Selected { get; set; } = 0;

        public int Count { get; set; } = 23;

        public bool Active { get; set; } = false;

        private int tableWidth { get; set; }
        private bool showDrives = false;
        private int[] pos;


        public Table(string path, int[] pos)
        {
            this.Dir = path;
            this.pos = pos;
            FillContent();
        }

        public void FillContent()
        {
            this.rows.Add(new TableData("..", "", ""));

            try
            {
                List<FileSystemInfo> items = FileService.GetChildren(Dir);
                foreach (FileSystemInfo item in items) {
                    TableData data = new TableData
                    (
                    item.Name,
                    GetLength(item).ToString(),
                    FormatDate(item.LastWriteTime)
                    );
                    this.rows.Add(data);
                }
            }
            catch (Exception err)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write("ACCESS DENIED!!!");
            }
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Dir + (Dir.Length > 0 ? @"\" : ""), rows[Selected].Name));

            switch (info.Key)
            {
                case ConsoleKey.UpArrow:
                    if (Selected <= 0) return;
                    Selected--;
                    if (Selected == offset - 1) 
                        offset--;
                    break;

                case ConsoleKey.DownArrow:
                    if (Selected >= rows.Count - 1) return;
                    Selected++;
                    if (Selected == offset + Math.Min(Count, this.rows.Count))
                        offset++;
                    break;

                case ConsoleKey.PageUp:
                    Selected = 0;
                    offset = 0;
                    break;

                case ConsoleKey.PageDown:
                    Selected = this.rows.Count >= Count ? this.rows.Count - 1 : this.rows.Count - 1;
                    offset = this.rows.Count > this.Count ? this.rows.Count - this.Count : 0;
                    break;

                case ConsoleKey.Enter:
                    if (!dir.Exists) return;
                    if (dir.FullName.Length == Dir.Length)
                        SetDrives();
                    else
                        ChangeDir(dir.FullName);
                    this.Selected = 0;
                    break;

                case ConsoleKey.Tab:
                    Selected = offset;
                    this.SwitchTable();
                    break;

                case ConsoleKey.F4:
                    if (File.Exists(dir.FullName))
                        this.EditFile(dir.FullName);
                    break;

                case ConsoleKey.F5:
                    this.Popup(dir.FullName, 5);
                    break;

                case ConsoleKey.F6:
                    this.Popup(dir.FullName, 6);
                    break;

                case ConsoleKey.F7:
                    this.Popup(Dir, 7);
                    break;

                case ConsoleKey.F8:
                    this.Popup(dir.FullName, 8);
                    break;
            }
        }

        public void UpdateData()
        {
            this.rows = new List<TableData>();
            this.FillContent();
            Selected = Selected > this.rows.Count-1 ? Selected - 1 : Selected;
        }

        public void ChangeDir(string dir)
        {
            Dir = dir;
            this.showDrives = false;
            this.rows = new List<TableData>();
            offset = 0;
            FillContent();
        }

        public void SetDrives() {
            Dir = "";
            this.showDrives = true;
            this.rows = new List<TableData>();
            DriveInfo[] drives = DriveInfo.GetDrives();
            
            foreach (DriveInfo drive in drives)
            {
                if (!drive.IsReady) continue;
                this.rows.Add(
                    new TableData(
                        drive.Name,
                        (drive.TotalFreeSpace / 1000).ToString()
                    )
                );
            }
        }

        public void Draw()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            this.tableWidth = Console.WindowWidth / 2;
            List<int> widths;

            widths = this.showDrives
                        ? new List<int> { this.tableWidth - 43, 40 }
                        : new List<int> { this.tableWidth - 24, 8, 12 };

            Console.SetCursorPosition(pos[0], pos[1]);

            DrawTop();

            Console.ForegroundColor = ConsoleColor.Yellow;
            DrawData(
                widths,
                    this.showDrives
                        ? new List<string> { "Drive", "Available space" }
                        : new List<string> { "Name", "Size", "Modify time" },
                    this.showDrives
                        ? new List<char> { 'C', 'C' }
                        : new List<char> { 'C', 'C', 'C' }
            );


            for (int i = offset; i < offset + Count; i++)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                if (i == Selected && this.Active)
                {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                if (i > rows.Count - 1)
                {
                    DrawData(
                        widths,
                            this.showDrives
                                ? new List<string> { "", "" }
                                : new List<string> { "", "", "" },
                            this.showDrives
                                ? new List<char> { 'L', 'R' }
                                : new List<char> { 'C', 'C', 'C' }
                    );
                }

                else
                {
                    PropertyInfo[] props = rows[i].GetType().GetProperties();
                    List<string> data = new List<string>();

                    foreach (PropertyInfo prop in props)
                    {
                        string? value = prop.GetValue(rows[i])?.ToString();
                        if (value is not null) data.Add(value);
                    }

                    DrawData(
                        widths, data,
                        this.showDrives 
                            ? new List<char> { 'L', 'R' } 
                            : new List<char> { 'L', 'R', 'C' }
                    );
                }
                Console.ResetColor();
            }
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            DrawBottom();
        }

        private void DrawData(List<int> widths, List<string> data, List<char> centering)
        {
            int i = 0;
            foreach (string item in data)
            {
                string prefixed = i == 0 && data[0].Length > 0 ? (new DirectoryInfo(Dir + @"\" + data[0]).Exists ? "/" : " ") + item : item;
                prefixed = prefixed.Length <= widths[i] ? prefixed : prefixed.Substring(0, widths[i] - 3) + "...";

                Console.Write('│');
                switch (centering[i])
                {
                    case 'L':
                        Console.Write(prefixed.PadRight(widths[i]));
                        break;

                    case 'R':
                        Console.Write(prefixed.PadLeft(widths[i]));
                        break;

                    case 'C':
                        double spacing = (double)widths[i] / 2 - (double)prefixed.Length / 2;
                        int spacingLeft = (int)spacing;
                        int spacingRight = (int)Math.Ceiling(spacing);
                        Console.Write(new String(' ', spacingLeft) + prefixed + new string(' ', spacingRight));
                        break;
                }
                i++;
            }
            Console.Write('│');
            SetCursor();
        }

        private void DrawBottom()
        {
            Console.Write("│" + new string('─', this.tableWidth - 2) + "│");
            SetCursor();
            Console.Write("│" + (this.rows[Selected].Name ?? "").PadRight(this.tableWidth - 2) + "│");
            SetCursor();
            Console.Write("└" + new string('─', this.tableWidth - 2) + "┘");
        }

        private void DrawTop()
        {
            Console.Write("┌──" + Dir.PadRight(tableWidth - 9, '─') + ".[^]>┐");
            SetCursor();
        }

        private void SetCursor()
        {
            Console.SetCursorPosition(pos[0], Console.CursorTop + 1);
        }

        private string FormatDate(DateTime time)
        {
            string[] months = {"Jan", "Feb", "Mar", "Apr", "May",
            "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
            string month = months[time.Month - 1];
            List<string> converted = new[] { time.Day, time.Hour, time.Minute }
                .Select(num => num.ToString())
                .Select(str => str.Length == 1 ? "0" + str : str).ToList();

            return $"{month} {converted[0]} {converted[1]}:{converted[2]}";
        }

        private long GetLength(FileSystemInfo item)
        {
            return item is FileInfo ? ((FileInfo)item).Length / 1000 : 4096;
        }
    }
}
