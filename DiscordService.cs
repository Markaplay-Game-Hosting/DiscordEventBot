using Discord;
using Discord.Webhook;
using Google.Apis.Calendar.v3.Data;

namespace DiscordEventBot
{
    public class DiscordService
    {
        static DiscordWebhookClient client = LoginWebhook();

        public async Task<bool> SendMessage(EmbedBuilder message)
        {
            if (client == null)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Client is not initiated!");
                return false;
            }
            string? role = Environment.GetEnvironmentVariable("SendTo");
            if (role == null)
            {
                role = "@everyone";
            }
            try{
                await client.SendMessageAsync(text: role, embeds: new[] { message.Build() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Unable to send the message to discord: {ex.Message}");
                return false;
            }

            return true;

        }
        public static DiscordWebhookClient LoginWebhook()
        {
            try
            {
                string? token = Environment.GetEnvironmentVariable("DiscordUrlToken");
                if (token == null)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Webhook url was not found in the environnement variable");
                }
                var client = new DiscordWebhookClient(token);
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Error initializing DiscordWebhookClient: {ex.Message}");
                //throw new Exception($"Error initializing the Webhook bot from url: {ex.Message}");
            }
            return null;
        }
        public static EmbedBuilder Build(Event eventInfo)
        {
            DateTimeOffset? startTime = eventInfo.Start.DateTimeDateTimeOffset;
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Event Name: {eventInfo.Summary}");
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Event Description: {eventInfo.Description}");
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Event Starts at: {startTime}");

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
