using Discord.Webhook;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Newtonsoft.Json;

namespace DiscordEventBot
{
    public class Singleton
    {
        private static readonly Singleton instance = new Singleton();
        private CalendarService _service;
        private DiscordWebhookClient _client;

        private Singleton() { }

        public static Singleton Instance
        {
            get { return instance; }
        }

        public CalendarService Service
        {
            get { return _service; }
            set { _service = AuthService.SetupService(); }
        }
        public DiscordWebhookClient Client
        { 
            get { return _client; } 
            set { _client = AuthService.LoginWebhook(); }
        }
    }
    internal class AuthService
    {
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
        public static CalendarService SetupService()
        {
            string jsonData = null;
            try
            {
                jsonData = System.IO.File.ReadAllText(@"/config/serviceaccount.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to find the service account file in /config/serviceaccount.json with error: {ex.Message}");
            }
            ServiceAccount? sa = new ServiceAccount();
            try
            {
                sa = JsonConvert.DeserializeObject<ServiceAccount>(jsonData);
            }
            catch
            {
                Console.WriteLine($"Unable to read account key form json file");
            }

            var credential = new ServiceAccountCredential(
                    new ServiceAccountCredential.Initializer(sa.client_email)
                    {
                        Scopes = new[] { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents },
                        User = "map-eventcalendar-bot@markaplay.iam.gserviceaccount.com"
                    }.FromPrivateKey(sa.private_key)
                    );
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MAP Event Checker Discord Bot"
            });
            return service;
        }
    }
}
