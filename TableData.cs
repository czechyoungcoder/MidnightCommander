using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander
{
    internal class TableData
    {
        public string Name { get; set; }
        public string? Size { get; set; }
        public string? LastChange { get; set; }
        public string? FreeSpace { get; set; }

        public TableData(string name, string size, string lastChange)
        {
            Name = name;
            Size = size;
            LastChange = lastChange;
        }

        public TableData(string name, string freeSpace)
        {
            Name = name;
            FreeSpace = freeSpace;
        }

        public List<string> ToList() => new List<string>() { Name};
    }
}
