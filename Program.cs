// See https://aka.ms/new-console-template for more information

namespace DiscordEventBot;

class Program
{
    public static async Task Main()
    {
        Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - starting Tasks");

        while (true)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Waiting 5 seconds");
            Thread.Sleep(TimeSpan.FromSeconds(5));
            await Polling.Start();
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Task just ended");
        }
    }
}