namespace TestUI
{
    using System.Diagnostics.CodeAnalysis;
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
            Console.WriteLine("Hello, World!");
            //Console.ReadKey();
            for (var i = 5; i > 0; i--)
            {
                Console.WriteLine($"{i}. saniye kaldı");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Uygulama kapanıyor.");
            Environment.Exit(0);
        }
        private static async Task MainAsync(string[] args)
        {
            await Task.CompletedTask;
        }
    }
}