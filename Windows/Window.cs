using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander
{
    internal abstract class Window
    {
        public Application Application { get; set; }

        public abstract void HandleKey(ConsoleKeyInfo info);

        public abstract void Draw();
    }
}
