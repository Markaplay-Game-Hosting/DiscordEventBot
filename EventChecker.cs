using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System.Globalization;
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
        CalendarService service = SetupService();

        if (service == null)
        {
            Console.WriteLine("service not set properly");
        }

        if (CalendarId == null)
        {
            Console.WriteLine("CalendarId is not set in the environnement variable");
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
    private static CalendarService SetupService()
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
        } catch
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


