using EventHub_Notification_Service_Demo.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Azure.Messaging.EventHubs;
using System.Text;
using Newtonsoft.Json;

namespace EventHub_Notification_Service_Demo
{
  public class MessageHub : Hub {

    private const string NotificationEventSource = "Notification_Service";
    private const string EnrollmentAPIEventSource = "Enrollment_API";
    private const string AccountManagementAPIEventSource = "AccountManagement_API";
    private const string HouseholdAPIEventSource = "Household_API";
    private const string ShoppingCartAPIEventSource = "ShoppingCart_API";

    private string[] EventSources = {
      NotificationEventSource,
      EnrollmentAPIEventSource,
      AccountManagementAPIEventSource,
      HouseholdAPIEventSource,
      ShoppingCartAPIEventSource
    };

    private string[] eventProcessingOrder = { "StartedProcessingEvent", "ProcessingEvent", "FinishedProcessingEvent" };

    private IEventHubPublisher _eventPublisher;

    public MessageHub(IEventHubPublisher eventPublisher) {
      _eventPublisher = eventPublisher;
    }

    public async Task SimulateEvents(int numberOfEvents) {

      var random = new Random();
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

      var batchEvents = new List<EventData>();
      var batchSize = 10;

      for (int i = 0; i < numberOfEvents; i++)
      {
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

        var serializedSimulatedEvent = JsonConvert.SerializeObject(simulatedEvent);
        var eventData = new EventData(Encoding.UTF8.GetBytes(serializedSimulatedEvent));
        
        batchEvents.Add(eventData);

        if (batchEvents.Count >= batchSize)
        {
          await _eventPublisher.SendBatch(batchEvents);
          batchEvents.Clear();
        }
      }


      await Task.CompletedTask;
      }
    

    public async Task SendEventReceived(NotificationServiceEvent eventReceived) {
       
        if (Clients != null) {
          await Clients.All.SendAsync("eventReceived", eventReceived);
        }
        
      }
    }
}
