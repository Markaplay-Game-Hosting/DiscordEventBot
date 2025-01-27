﻿using Discord.Webhook;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace DiscordEventBot
{
    public class Polling
    {
        
        public static Task Start()
        {
            Singleton.Instance.Service = AuthService.SetupService();
            Singleton.Instance.Client = AuthService.LoginWebhook();
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
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Found {x.Items.Count} events");
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
                        
                        bool IsSent = false;

                        IsSent = DiscordService.SendMessage(builder);
                        if (IsSent != true)
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Message failed to be sent");

                        } else
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Message was sent successfully");
                        }
                    }
                });
        }
    }
}
