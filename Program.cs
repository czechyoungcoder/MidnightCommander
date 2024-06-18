namespace MidnightCommander
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Console.CursorVisible = false;
            
            Application app = new Application();

            while (true)
            {
                app.Draw();

                ConsoleKeyInfo info = Console.ReadKey();
                app.HandleKey(info);
            }
        }
    }
}