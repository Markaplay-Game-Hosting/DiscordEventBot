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
            throw new Exception("service not set properly");
        }

        if (CalendarId == null)
        {
            throw new Exception("CalendarId is not set in the environnement variable");
        }

        var request = service.Events.List(CalendarId);

        //request = (EventsResource.ListRequest)SampleHelpers.ApplyOptionalParms(request, optional);
        request.TimeMin = DateTime.Now;
        request.TimeMax = (DateTime.Now).AddSeconds(5);
        //request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        Events events = request.Execute();

        Console.WriteLine(events.Description);

        IList<Event> eventsItems = events.Items;

        return events;
        
    }
    private static CalendarService SetupService()
    {
        string jsonData;
        try
        {
            jsonData = System.IO.File.ReadAllText(@"/config/serviceaccount.json");
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to find the service account file in /config/serviceaccount.json with error: {ex.Message}");
        }
        ServiceAccount? sa;
        try
        {
            sa = JsonConvert.DeserializeObject<ServiceAccount>(jsonData);
        } catch
        {
            throw new Exception($"Unable to read account key form json file");
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


