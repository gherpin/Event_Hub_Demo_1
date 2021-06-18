using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHub_Notification_Service_Demo.Infrastructure
{
  public interface IEventHubPublisher {
    Task SendBatch(IEnumerable<EventData> eventDataList);
  }

  public class EventHubPublisher :  IEventHubPublisher
  {
    private readonly string _eventHubConnectionString;
    private readonly string _eventHubName;
    private EventHubProducerClient _producerClient;


    public EventHubPublisher(IOptions<ConnectionStrings> options) {

      _eventHubConnectionString = options.Value.AzureEventHub.ConnectionString;
      _eventHubName = options.Value.AzureEventHub.EventHubName;
     
      
    }

   public async Task StopAsync(CancellationToken cancellationToken)
    {
      await _producerClient.CloseAsync(cancellationToken);
    }

    public async Task SendBatch(IEnumerable<EventData> eventDataList) {

      try {

        _producerClient = new EventHubProducerClient(_eventHubConnectionString, _eventHubName);

        using (var eventBatch = await _producerClient.CreateBatchAsync())
        {

          foreach (var e in eventDataList)
          {
            eventBatch.TryAdd(e);
          }
          await _producerClient.SendAsync(eventBatch);
        }
      }
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
      }
      finally {
        await _producerClient.DisposeAsync();
      }
     
    }

  }
}
