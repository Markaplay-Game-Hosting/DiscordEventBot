using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;

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
                        if(eventInfo.Start.DateTimeDateTimeOffset < DateTimeOffset.Now)
                        {
                            Console.WriteLine($"Event {eventInfo.Summary} is on going, skipping...");
                            return;
                        } else
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Event will start soon, gotta tell everyone!");
                        }
                        Discord.EmbedBuilder builder = DiscordService.Build(eventInfo);
                        var discordService = new DiscordService();
                        discordService.SendMessage(builder);
                        Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Message Send!");
                    }
                });
        }
    }
}
