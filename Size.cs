using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander
{
    internal class Size
    {
        public int width { get; set; }
        public int height { get; set; }

        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
