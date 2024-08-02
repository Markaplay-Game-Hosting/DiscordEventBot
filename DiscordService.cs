using Discord;
using Discord.Webhook;
using System;
using System.Collections.Generic;
using Google.Apis.Calendar.v3.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DiscordEventBot
{
    public class DiscordService
    {
        static DiscordWebhookClient client = LoginWebhook();

        public async void SendMessage(EmbedBuilder message)
        {
            if (client == null)
            {
                throw new Exception("Client is not initiated!");
            }
            string? role = Environment.GetEnvironmentVariable("SendTo");
            if (role == null) 
            {
                role = "@everyone";
            }
            await client.SendMessageAsync(text: role, embeds: new[] { message.Build() });
               
        }
        public static DiscordWebhookClient LoginWebhook()
        {
            try
            {
                string? token = Environment.GetEnvironmentVariable("DiscordUrlToken");
                if (token == null)
                {
                    throw new Exception("Webhook url was not found in the environnement variable");
                }
                var client = new DiscordWebhookClient(token);
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing DiscordWebhookClient: {ex.Message}");
                throw new Exception($"Error initializing the Webhook bot from url: {ex.Message}");
            }
        }
        public static EmbedBuilder Build(Event eventInfo)
        {
            DateTimeOffset? startTime = eventInfo.Start.DateTimeDateTimeOffset;
            Console.WriteLine($"Event Name: {eventInfo.Summary}");
            Console.WriteLine($"Event Description: {eventInfo.Description}");
            Console.WriteLine($"Event Starts at: {startTime}");

            var embed = new EmbedBuilder
            {
                Title = $"{eventInfo.Summary}",
                Timestamp = startTime,
                Color = Color.Orange,
            };
            embed.AddField("Event Info", eventInfo.Description, inline: true);

            return embed;
        }
    }
}
