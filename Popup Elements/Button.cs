using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander
{
    internal class Button : IElement
    {

        public string Name { get; set; }
        public bool Active { get; set; }
        public event Action Selected;

        public Button(string name)
        {
            this.Name = name;
        }
        public void Draw()
        {
            Console.BackgroundColor = ConsoleColor.White;
            if (Active)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
            }
            Console.Write($"[< {Name} >]");

            Console.BackgroundColor = ConsoleColor.White;
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Enter)
            {
                Selected();
            }
        }
    }
}
