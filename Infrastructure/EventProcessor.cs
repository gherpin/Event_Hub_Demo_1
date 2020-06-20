using System.Threading.Tasks;
using System;

namespace FoodTruckWebsite_Notification_Service.Infrastructure
{
  public interface IEventProcessor
  {
    Task BeginProcessAsync(NotificationServiceEvent receivedEvent);
    Task ProcessAsync(NotificationServiceEvent receivedEvent);
    Task EndProcessAsync(NotificationServiceEvent receivedEvent);

  }
  public class EventProcessor : IEventProcessor {
    //TODO: Holds list of events that service knows how to handle

    public async Task BeginProcessAsync(NotificationServiceEvent receivedEvent) {

      receivedEvent.ServiceEvents.Add("StartedProcessingEvent", DateTimeOffset.UtcNow.ToString());
      //I can time stamp the moment the service consumed the event
      Console.WriteLine("\tRecevied event: {0}", receivedEvent.ReceivedEvent.EventName);

      await Task.CompletedTask;
    }

    public async Task ProcessAsync(NotificationServiceEvent receivedEvent)
    {
      receivedEvent.ServiceEvents.Add("ProcessingEvent", DateTimeOffset.UtcNow.ToString());
      await Task.CompletedTask;
    }

    public async Task EndProcessAsync(NotificationServiceEvent receivedEvent) {

      receivedEvent.ServiceEvents.Add("FinishedProcessingEvent", DateTimeOffset.UtcNow.ToString());
      //TODO: Send Message to UI that event has been processed
      await Task.CompletedTask;
    }

  }
}
