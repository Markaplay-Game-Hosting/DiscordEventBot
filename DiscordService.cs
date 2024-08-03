using Discord;
using Discord.Webhook;
using Google.Apis.Calendar.v3.Data;

namespace DiscordEventBot
{
    public class DiscordService(DiscordWebhookClient client)
    {
        private DiscordWebhookClient _client = client;
        public static bool SendMessage(EmbedBuilder message)
        {
            DiscordWebhookClient client = Singleton.Instance.Client;
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
                client.SendMessageAsync(text: role, embeds: new[] { message.Build() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - Unable to send the message to discord: {ex.Message}");
                return false;
            }

            return true;

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
