using Google.Apis.Calendar.v3.Data;
using System.Collections.Generic;

namespace DiscordEventBot
{
    public class Polling
    {
        public static Task Start()
        {
            HttpClientHandler handler = new HttpClientHandler();
            return Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Checking if an event is starting...");
                    Events events;
                    do
                    {
                        events = EventChecker.GetEvents();
                    } while (events == null || events.Items.Count == 0);
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - An Event is starting soon!");
                    return events;
                }).
                ContinueWith((events) =>
                {
                    var x = events.Result;
                    foreach (Event eventInfo in x.Items)
                    {
                        if (eventInfo.Start.DateTimeDateTimeOffset < DateTimeOffset.Now)
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Event {eventInfo.Summary} is on going, skipping...");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Event will start soon, gotta tell everyone!");
                        }
                        Discord.EmbedBuilder builder = DiscordService.Build(eventInfo);
                        
                        var discordService = new DiscordService();
                        Task<bool> IsSent;
                        int Counter = 0;
                        do {
                            IsSent = discordService.SendMessage(builder);
                            Counter++;
                        } while (IsSent.IsCompleted == false && IsSent.Result == false || Counter <= 3);
                        
                        if (IsSent.Result == true)
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Message Send Successfully!");
                        } else
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Message Send Failed!");
                        }
                    }
                });
        }
    }
}
