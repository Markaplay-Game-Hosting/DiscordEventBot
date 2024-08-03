using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Newtonsoft.Json;
namespace DiscordEventBot;

public class ServiceAccount
{
    public string type { get; set; }
    public string project_id { get; set; }
    public string private_key_id { get; set; }
    public string private_key { get; set; }
    public string client_email { get; set; } = null!;
    public string client_id { get; set; }
    public string auth_uri { get; set; }
    public string token_uri { get; set; }
    public string auth_provider_x509_cert_url { get; set; }
    public string client_x509_cert_url { get; set; }
    public string universe_domain { get; set; }
}

public class EventChecker
{
    static string? CalendarId = Environment.GetEnvironmentVariable("GoogleCalendarId");
    public static Events GetEvents()
    {
        CalendarService service = Singleton.Instance.Service;
        if (service == null)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - service not set properly");
        }

        if (CalendarId == null)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - CalendarId is not set in the environnement variable");
        }

        var request = service.Events.List(CalendarId);

        DateTimeOffset startTime = DateTime.Now;
        DateTimeOffset endTime = DateTime.Now.AddSeconds(5);

        request.TimeMinDateTimeOffset = startTime;
        request.TimeMaxDateTimeOffset = endTime;

        Events events = new Events();
        try
        {
            events = request.Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cannot get events list from calendar: {ex.Message}");
        }
        return events;

    }
}


