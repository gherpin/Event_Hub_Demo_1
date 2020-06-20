using FoodTruckWebsite_Notification_Service.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FoodTruckWebsite_Notification_Service
{
  public class MessageHub : Hub {

    private const string NotificationEventSource = "Notification_Service";
    private const string FoodTruckManagementAPIEventSource = "FoodTruckManagement_API";
    private const string AccountManagementAPIEventSource = "AccountManagement_API";
    private const string MenuManagementAPIEventSource = "MenuManagement_API";
    private const string PaymentManagementAPIEventSource = "PaymentManagement_API";

    private string[] EventSources = {
      NotificationEventSource,
      FoodTruckManagementAPIEventSource,
      AccountManagementAPIEventSource,
      MenuManagementAPIEventSource,
      PaymentManagementAPIEventSource
    };

    private string[] eventProcessingOrder = { "StartedProcessingEvent", "ProcessingEvent", "FinishedProcessingEvent" };

      public async Task SimulateEvents() {

      var random = new Random();
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

      for (int i = 0; i < 10000; i++)
      {
        await Task.Delay(100);
        var simulatedEvent = new ReceivedEvent
        {
          EventSource = EventSources[random.Next(0, 5)],
          EventName = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
          OccurredAt = DateTimeOffset.UtcNow,
          EventData = new Dictionary<string, object>() {
            { "Key 1", "Value 1" },
            { "Key 2", "Value 2" },
            { "Key 3", "Value 3" },
            { "Key 4", "Value 4" }
          }
        };

        var testNotificationServiceEvent = new NotificationServiceEvent(simulatedEvent);

        //Loop Through each Processing Event and send event
        var eventStart = DateTimeOffset.UtcNow;
        foreach (var step in eventProcessingOrder)
        {
          testNotificationServiceEvent.ServiceEvents.Add(step, DateTimeOffset.UtcNow.ToString());


          await SendEventReceived(testNotificationServiceEvent);
        }
      }


      await Task.CompletedTask;
      }
    

    public async Task SendEventReceived(NotificationServiceEvent eventReceived) {
        await Clients.All.SendAsync("eventReceived", eventReceived);
      }
    }
}
