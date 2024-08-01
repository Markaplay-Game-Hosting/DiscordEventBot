# Discord Event Bot

This is a simple implementation of a Discord Webhook Event notification bot written in .NET Core 8

# Requirements

- Google Cloud Platform (GCP) Service Account
- Google Calendar
	- **The Service Account must have read access**
- Discord Webhook link

# Configuration

## Environnement variables

`DiscordUrlToken` : the Webhook url that discord generate when creating a webhook
`GoogleCalendarId` : The ID of your google calendar

## Files

the GCP Serivce Account token file must be store in the same directory as the app and the file name of `serviceaccount.json`


# Docker
`docker-compose.yaml`:

```yaml
services:
  DiscordEventBot:
	container_name: EventBot
	image: vincdelta/map-discordeventbot:latest
	restart: unless-stopped
	env:
		- "DiscordUrlToken=https://discord/bla/bla/token"
		- "GoogleCalendarId=9227723950618562939d...@group.calendar.google.com"
	volumes:
		- "/etc/DiscordEventBot/serviceaccount.json:/config/serviceaccount.json"
```

