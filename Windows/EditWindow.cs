using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidnightCommander.Popups;

namespace MidnightCommander.Windows
{
    internal class EditWindow : Window
    {
        
        private string path;
        private List<string> lines;
        private int width = Console.WindowWidth;
        private int height = Console.WindowHeight-1;
        private Position cursor = new Position(0, 0);
        private Position offset = new Position(0, 0);
        private bool modified = false;
        private Marker marker;

        public EditWindow(string path)
        {
            this.path = path;
            lines = FileService.ReadLines(path);
            marker = new Marker(lines, cursor, offset);

            Console.Clear();
            Console.CursorVisible = true;
        }

        public void DrawHeader()
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("".PadRight(Console.WindowWidth));
            Console.SetCursorPosition(0, 0);
            Console.Write(new FileInfo(path).Name.PadRight(30));
            Console.Write($"[-{(modified ? 'M' : '-')}--]  ");
            Console.Write(cursor.X+1 + offset.X);
            Console.Write($" L:[{cursor.Y+1}+{offset.Y} {cursor.Y+1 + offset.Y}/{lines.Count}]");
            Console.ResetColor();
        }

        public void DrawLines()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            int lastLine = offset.Y + height - 2;

            Console.SetCursorPosition(0, 1);
            for (int line = offset.Y; line <= lastLine; line++)
            {
                string text = line > lines.Count - 1 ? "" : lines[line];
                if (text.Length == 0 || text.Length <= offset.X)
                    Console.Write("".PadRight(width));
                else
                    Console.Write(text.Substring(offset.X, (text.Length - offset.X) >= width ? width : text.Length - offset.X).PadRight(width));
            }
            Console.SetCursorPosition(0, 0);

        }

        public override void Draw()
        {
            DrawHeader();
            DrawLines();
            DrawMarked();
            Application.DrawMenu(new List<string>() { "Help", "Save", "Mark", "Replace", "Copy", "Move", "Search", "Delete", "PullDn", "Quit" });

            Console.SetCursorPosition(cursor.X, cursor.Y + 1);
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            Debug.WriteLine(cursor.Y + " " + offset.Y);
            switch (info.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (CursorOnStart())
                        break;
                    if (cursor.X == 0 && offset.X > 0)
                    {
                        offset.X--;
                        break;
                    }
                    if (cursor.X > 0)
                    {
                        cursor.X--;
                        break;
                    }

                    if (cursor.Y == 0) offset.Y--;
                    else cursor.Y--;
                    CursorToEnd();
                    break;

                case ConsoleKey.RightArrow:
                    if (cursor.X + offset.X == GetLineLength())
                    {
                        if ((cursor.Y + offset.Y) == lines.Count - 1) break;
                        if (cursor.Y == height - 2) offset.Y++;
                        else cursor.Y++;
                        cursor.X = 0;
                        offset.X = 0;
                        break;
                    }
                    if (cursor.X == width - 1) offset.X++;
                    else cursor.X++;
                    break;

                case ConsoleKey.DownArrow:
                    if (cursor.Y + offset.Y == lines.Count-1) break;
                    if (cursor.Y == height - 2)
                    {
                        offset.Y++;
                        break;
                    }
                    cursor.Y++;
                    if ((cursor.X + offset.X) > GetLineLength())
                    {
                        CursorToEnd();
                    };
                    break;

                case ConsoleKey.UpArrow:
                    if (cursor.Y + offset.Y == 0) break;
                    if (cursor.Y == 0)
                    {
                        offset.Y--;
                        break;
                    }
                    cursor.Y--;
                    if ((cursor.X + offset.X) > GetLineLength())
                    {
                        CursorToEnd();
                    };
                    break;

                case ConsoleKey.Home:
                    cursor.X = 0;
                    offset.X = 0;
                    break;

                case ConsoleKey.End:
                    CursorToEnd();
                    break;

                case ConsoleKey.PageDown:
                    if (lines.Count < height-1)
                    {
                        cursor.Y = lines.Count - 2;
                    }
                    else if (lines.Count - height - 1 - offset.Y < height - 1)
                        offset.Y += lines.Count - height - offset.Y +1;
                    else 
                        offset.Y += height - 2;
                    CursorToEnd();
                    break;


                case ConsoleKey.PageUp:
                    if (lines.Count < height - 1)
                    {
                        cursor.Y = 0;
                    }
                    else if (offset.Y < height-1)
                        offset.Y -= offset.Y;
                    else
                        offset.Y -= height - 2;
                    if (offset.Y < 0) offset.Y = 0;
                    CursorToEnd();
                    break;

                case ConsoleKey.Enter:
                    string left = GetLine().Substring(0, cursor.X + offset.X);
                    string right = GetLine().Substring(cursor.X + offset.X);
                    lines[cursor.Y + offset.Y ] = left;
                    lines.Insert(cursor.Y + offset.Y + 1, right);
                    if (cursor.Y == height - 2)
                        offset.Y++;
                    else
                        cursor.Y++;
                    cursor.X = 0;
                    offset.X = 0;
                    modified = true;
                    break;

                case ConsoleKey.Delete:
                    if (GetLineLength() == 0) {
                        lines.RemoveAt(cursor.Y + offset.Y);
                        if (offset.Y > 0) offset.Y--;
                    }
                    else if (cursor.X + offset.X == GetLineLength())
                    {
                        if (cursor.Y + offset.Y == lines.Count - 1) break;
                        lines[cursor.Y + offset.Y] += lines[cursor.Y + offset.Y + 1];
                        lines.RemoveAt(cursor.Y + offset.Y + 1);
                        if (offset.Y > 0) offset.Y--;
                    }
                    else
                        lines[cursor.Y + offset.Y] = lines[cursor.Y + offset.Y].Remove(cursor.X + offset.X, 1);
                    modified = true;
                    break;

                case ConsoleKey.Backspace:
                    if (CursorOnStart()) break;
                    if (GetLineLength() == 0)
                    {
                        lines.RemoveAt(cursor.Y + offset.Y);
                        if (offset.Y > 0)
                            offset.Y--;
                        else
                            cursor.Y--;
                        CursorToEnd();
                    }
                    else if (cursor.X == 0 && offset.X == 0)
                    {
                        int length = Math.Min(GetLineLength(), width - 1);
                        lines[cursor.Y + offset.Y - 1] += lines[cursor.Y + offset.Y];
                        lines.RemoveAt(cursor.Y + offset.Y);
                        if (offset.Y > 0) {
                            offset.Y--; }
                        else
                            cursor.Y--;
                        cursor.X = Math.Min(GetLineLength(), width - 1) - length;
                        offset.X = Math.Max(GetLineLength(), width - 1) - (width - 1);


                    }
                    else
                    {
                        Debug.Write(cursor.X);
                        lines[cursor.Y + offset.Y] = lines[cursor.Y + offset.Y].Remove(cursor.X + offset.X-1, 1);
                        if (offset.X > 0) 
                            offset.X--;
                        if (cursor.X > 0 && offset.X == 0)
                            cursor.X--;
                    }
                    modified = true;
                    break;

                case ConsoleKey.F2:
                    FileService.SaveFile(path, lines);
                    modified = false;
                    break;

                case ConsoleKey.F3:
                    if (marker.active)
                    {
                        marker.active = false;
                        marker.FillData(); 
                    }
                    else
                    {
                        marker.start = new Position(cursor.X + offset.X, cursor.Y + offset.Y);
                        marker.active = true;
                        
                    }
                    break;

                case ConsoleKey.F5:
                    marker.CopyData();
                    marker.Reset();
                    break;

                case ConsoleKey.F6:
                    marker.MoveData();
                    marker.Reset();
                    break;

                case ConsoleKey.F8:
                    marker.DeleteData();
                    marker.Reset();
                    break;

                case ConsoleKey.F10:
                    if (modified)
                        Application.PushWindow(new SavePopup(path, lines, new Size(50, 7)));
                    else
                        Application.PopWindow();
                    break;


                default:
                    if (Char.IsAscii(info.KeyChar))
                    {
                        lines[cursor.Y + offset.Y] = GetLine().Insert(cursor.X + offset.X, info.KeyChar.ToString());
                        if (cursor.X == width - 1) offset.X++;
                        else cursor.X++;
                    }
                    modified = true;
                    break;
            }

            if (marker.active) marker.end = new Position(cursor.X + offset.X, cursor.Y + offset.Y);
        }

        private void DrawMarked()
        {
            if (marker.start is null || marker.end is null) return;
            if (offset.Y > marker.start.Y) return;

            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            bool backwards = marker.start.Y > marker.end.Y;

            if (marker.start.Y == marker.end.Y)
            {
                if (marker.start.X > marker.end.X)
                {
                    Console.SetCursorPosition(marker.end.X - (offset.X > 0 ? marker.start.X - offset.X : 0), marker.start.Y + 1 - offset.Y);
                    Console.WriteLine(lines[marker.start.Y].Substring(marker.end.X, marker.start.X - marker.end.X));
                }
                else
                {
                    Console.SetCursorPosition(marker.start.X - offset.X > 0 ? marker.start.X - offset.X : 0, marker.start.Y + 1 - offset.Y);
                    Console.WriteLine(lines[marker.start.Y].Substring(marker.start.X, Math.Min(marker.end.X - marker.start.X, width)));
                    
                }
            }
                
            else
            {
                for (int i = Math.Min(marker.start.Y, marker.end.Y); i <= Math.Max(marker.start.Y, marker.end.Y); i++)
                {
                    if (i == marker.start.Y)
                    {
                        if (backwards)
                        {
                            Console.SetCursorPosition(0, i + 1);
                            Console.Write(lines[i].Substring(0, marker.start.X));
                        }
                        else
                        {
                            Console.SetCursorPosition(marker.start.X, i + 1);
                            Console.Write(lines[i].Substring(marker.start.X, Math.Min(width, lines[i].Length) - marker.start.X));
                        }

                    }
                    else if (i == marker.end.Y)
                    {
                        if (backwards)
                        {
                            Console.SetCursorPosition(marker.end.X, i + 1);
                            Console.Write(lines[i].Substring(marker.end.X, Math.Min(width, lines[i].Length) - marker.end.X));
                        }
                        else
                        {
                            Console.SetCursorPosition(0, i + 1);
                            Console.Write(lines[i].Substring(0, marker.end.X));
                        }

                    }
                    else
                    {
                        if (backwards)
                        {
                            Console.SetCursorPosition(0, i + 1);
                            Console.Write(lines[i].Substring(0, Math.Min(width, lines[i].Length)));
                        }
                        else
                        {
                            Console.SetCursorPosition(0, i + 1);
                            Console.Write(lines[i].Substring(0, Math.Min(width, lines[i].Length)));
                        }

                    }
                }
            }

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            
        }


        private void CursorToEnd()
        {
            cursor.X = Math.Min(GetLineLength(), width - 1);
            offset.X = Math.Max(GetLineLength(), width - 1) - (width - 1);
        }

        private bool CursorOnStart()
        {
            return cursor.X == 0 && offset.X == 0 && cursor.Y == 0 && offset.Y == 0;
        }

        private bool EmptyLine()
        {
            return lines[cursor.Y + offset.Y].Length == 0;
        }

        private int GetLineLength()
        {
            return lines[cursor.Y + offset.Y].Length;
        }

        private string GetLine()
        {
            return lines[cursor.Y + offset.Y];
        }

        private int GetLineIndex()
        {
            return cursor.Y + offset.Y;
        }
    }
}