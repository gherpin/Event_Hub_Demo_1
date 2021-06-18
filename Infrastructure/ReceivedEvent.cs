using System;
using System.Collections.Generic;

namespace EventHub_Notification_Service_Demo.Infrastructure
{
  public class ReceivedEvent {

    public string EventSource { get; set; }
    public string EventName { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public IReadOnlyDictionary<string, object> EventData { get; set; }
  }

  public class NotificationServiceEvent {
    public NotificationServiceEvent(ReceivedEvent receivedEvent)
    {
      Id = Guid.NewGuid();
      ReceivedEvent = receivedEvent;
      ServiceEvents = new Dictionary<string, string>();
    }

    /// <summary>
    /// Id to track the event through the notification service.
    /// </summary>
    public Guid Id { get; private set; }
    /// <summary>
    /// The event as received by the notifciation service.
    /// </summary>
    public ReceivedEvent ReceivedEvent { get; }

    /// <summary>
    /// Events performed by the notification service for the event
    /// </summary>
    public Dictionary<string, string> ServiceEvents { get; }
  }
}
