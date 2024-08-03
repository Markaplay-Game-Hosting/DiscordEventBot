using Discord.Webhook;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace DiscordEventBot
{
    public class Polling
    {
        private DiscordWebhookClient _webhookClient = AuthService.LoginWebhook();
        private CalendarService _calendarService = AuthService.SetupService();
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
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy / MM / dd HH: mm:ss")} - Unable to send the message, retrying 1 time");
                            IsSent = DiscordService.SendMessage(builder);
                            if (IsSent != true) {
                                Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Unable to send the message, retrying 2 times");
                                IsSent = DiscordService.SendMessage(builder);
                                if (IsSent != true)
                                {
                                    Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Unable to send the message for the 3rd time, skipping...");
                                } else
                                {
                                    Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Message was sent successfully after 3 tries");
                                }
                            } else
                            {
                                Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Message was sent successfully after 2 tries");
                            }
                            
                        } else
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Message was sent successfully after 1 try");
                        }
                    }
                });
        }
    }
}
