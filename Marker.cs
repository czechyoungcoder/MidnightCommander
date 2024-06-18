using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander
{
    internal class Marker
    {
        public bool active { get; set; } = false;
        public bool selected { get; set; } = false;
        public Position? start;
        public Position? end;
        private Position cursor;
        private Position offset;
        public List<string> lines;
        public List<string> data = new List<string>();

        public Marker(List<string> lines, Position cursor, Position offset) {
            this.lines = lines;
            this.cursor = cursor;
            this.offset = offset;
        }

        public void FillData()
        {
            data = new List<string>();
            bool backwards = start!.Y > end!.Y;

            if (start!.Y == end!.Y)
                data.Add(lines[start!.Y].Substring(Math.Min(start!.X, end!.X), Math.Abs(start!.X - end!.X)));

            else
            {
                for (int i = Math.Min(start!.Y, end!.Y); i <= Math.Max(start!.Y, end!.Y); i++)
                {
                    if (i == start!.Y)
                    {
                        if (backwards)
                        {
                            data.Add(lines[start!.Y].Substring(0, start!.X));
                        }
                        else
                        {
                            data.Add(lines[start!.Y].Substring(start!.X));
                        }
                    }

                    else if (i == end!.Y)
                    {
                        if (backwards)
                        {
                            data.Add(lines[end!.Y].Substring(end!.X));
                        }
                        else
                        {
                            data.Add(lines[end!.Y].Substring(0, end!.X));
                        }
                    }

                    else
                    {
                        data.Add(lines[i]);
                    }
                }
            }
        }

        public void CopyData()
        {
            if (!HasMarked()) return;
            FillData();
            int x = cursor.X + offset.X;
            int y = cursor.Y + offset.Y;
            if (data.Count == 1)
           {
                lines[y] = lines[y].Insert(x, data[0]);
            }
           else
            {
                string left = lines[y].Substring(0, x);
                string right = lines[y].Substring(x);
                for (int i = 0; i < data.Count; i++)
                {
                    if (i == 0)
                        lines[y] = left + data[i];
                    else if (i == data.Count - 1)
                    {
                        lines.Insert(y + i, data[i]+right);
                    }
                    else
                    {
                        lines.Insert(y + i, data[i]);
                    }
                }
            }
        }

        public void DeleteData()
        {
            if (!HasMarked()) return;
            FillData();
            string left = lines[Math.Min(start!.Y, end!.Y)].Substring(0, start!.Y < end!.Y ? start!.X : end!.X);
            string right = lines[Math.Max(start!.Y, end!.Y)].Substring(data[data.Count-1].Length);
            Debug.WriteLine(left);
            Debug.WriteLine(right);
            if (data.Count == 1)
            {
                lines[start!.Y] = lines[start!.Y].Remove(Math.Min(start!.X, end!.X), data[0].Length);
            }
            else
            {
                lines[Math.Min(start!.Y, end!.Y)] = left + right;

                for (int i = 0; i < data.Count; i++)
                {
                    lines.RemoveAt(Math.Min(start!.Y, end!.Y) + 1);
                }
            }
        }

        public void MoveData()
        {
            if (!HasMarked()) return;
            FillData();
            CopyData();
            DeleteData();
        }

        public void Reset()
        {
            start = null;
            end = null;
            data = new List<string>();
        }

        public bool HasMarked()
        {
            return start is not null && end is not null;
        }
    }
}
