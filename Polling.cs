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
                    Events events;
                    do
                    {
                        events = EventChecker.GetEvents();
                    } while (events == null || events.Items.Count == 0);
                    return events;
                }).
                ContinueWith((events) =>
                {
                    var x = events.Result;
                    foreach (Event eventInfo in x.Items)
                    {
                        if(eventInfo.Start.DateTimeDateTimeOffset < DateTimeOffset.Now)
                        {
                            Console.WriteLine($"Event {eventInfo.Summary} is on going");
                            return;
                        }
                        Discord.EmbedBuilder builder = DiscordService.Build(eventInfo);
                        var discordService = new DiscordService();
                        discordService.SendMessage(builder);
                    }
                });
        }
    }
}
