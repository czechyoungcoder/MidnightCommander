using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander
{
    internal interface IElement
    {
        void Draw();
        void HandleKey(ConsoleKeyInfo info);
        public bool Active { get; set; }
    }
}
