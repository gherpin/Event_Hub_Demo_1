using System.Threading.Tasks;
using System;

namespace EventHub_Notification_Service_Demo.Infrastructure
{
  public interface IEventProcessor
  {
    Task BeginProcessAsync(NotificationServiceEvent receivedEvent);
    Task ProcessAsync(NotificationServiceEvent receivedEvent);
    Task EndProcessAsync(NotificationServiceEvent receivedEvent);

  }
  public class EventProcessor : IEventProcessor {
    private MessageHub _messageHub;

    //TODO: Holds list of events that service knows how to handle

    public EventProcessor(MessageHub messageHub) {
      _messageHub = messageHub;
    }

    public async Task BeginProcessAsync(NotificationServiceEvent receivedEvent) {

      receivedEvent.ServiceEvents.Add("StartedProcessingEvent", DateTimeOffset.UtcNow.ToString("o"));

      await _messageHub.SendEventReceived(receivedEvent);
      //I can time stamp the moment the service consumed the event
      Console.WriteLine("\tRecevied event: {0}", receivedEvent.ReceivedEvent.EventName);

      await Task.CompletedTask;
    }

    public async Task ProcessAsync(NotificationServiceEvent receivedEvent)
    {
      receivedEvent.ServiceEvents.Add("ProcessingEvent", DateTimeOffset.UtcNow.ToString("o"));
      await _messageHub.SendEventReceived(receivedEvent);
      await Task.CompletedTask;
    }

    public async Task EndProcessAsync(NotificationServiceEvent receivedEvent) {

      receivedEvent.ServiceEvents.Add("FinishedProcessingEvent", DateTimeOffset.UtcNow.ToString("o"));
      await _messageHub.SendEventReceived(receivedEvent);
      //TODO: Send Message to UI that event has been processed
      await Task.CompletedTask;
    }

  }
}
