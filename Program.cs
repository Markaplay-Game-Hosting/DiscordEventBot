// See https://aka.ms/new-console-template for more information
using Discord;
using Discord.Webhook;
using System.Diagnostics;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Threading;
using Discord.Net;


namespace DiscordEventBot;

class Program
{
    public static async Task Main()
    {
        // The webhook url follows the format https://discord.com/api/webhooks/{id}/{token}
        // Because anyone with the webhook URL can use your webhook
        // you should NOT hard code the URL or ID + token into your application.
        Console.WriteLine("starting Tasks");

        while (true)
        {
            Console.WriteLine("Waiting 5 seconds");
            Thread.Sleep(TimeSpan.FromSeconds(5));
            await Polling.Start();
        }
    }
}