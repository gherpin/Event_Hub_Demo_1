using Microsoft.Extensions.Options;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using System;
using System.Text;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Text.Json;
using System.Diagnostics;

namespace EventHub_Notification_Service_Demo.Infrastructure
{
  /// <summary>
  /// This will have to be added to each application that consumes data from the Azure Event Hub
  /// </summary>
  public class EventHubConsumer : IHostedService {

    private readonly string _eventHubConnectionString;
    private readonly string _eventHubName;
    private readonly string _blobStorageConnectionString;
    private readonly string _blobContainerName;

    private readonly BlobContainerClient _blobStorageClient;
    private readonly EventProcessorClient _eventProcessorClient;

    private readonly IEventProcessor _eventProcessor;


    public EventHubConsumer(IOptions<ConnectionStrings> options, IEventProcessor eventProcessor)
    {
      _eventHubConnectionString = options.Value.AzureEventHub.ConnectionString;
      _eventHubName = options.Value.AzureEventHub.EventHubName;
      _blobStorageConnectionString = options.Value.AzureBlobStorage.ConnectionString;
      _blobContainerName = options.Value.AzureBlobStorage.ContainerName;

      _blobStorageClient = new BlobContainerClient(_blobStorageConnectionString, _blobContainerName);

      //Each consumer should use its own consumer group
      string consumerGroup = "$default"; 

      _eventProcessorClient = new EventProcessorClient(
        _blobStorageClient, 
        consumerGroup,
        _eventHubConnectionString,
        _eventHubName);

      _eventProcessor = eventProcessor;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
      _eventProcessorClient.ProcessEventAsync += ProcessEventHandler;
      _eventProcessorClient.ProcessErrorAsync += ProcessErrorHandler;
      await _eventProcessorClient.StartProcessingAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      await _eventProcessorClient.StopProcessingAsync();
    }

    private async Task ProcessEventHandler(ProcessEventArgs arg)
    {
      var data = Encoding.UTF8.GetString(arg.Data.Body.ToArray());      
      var receivedEvent = JsonSerializer.Deserialize<ReceivedEvent>(data);
      var notificationServiceEvent = new NotificationServiceEvent(receivedEvent);
      var random = new Random();
      // Update checkpoint in the blob storage so that the app receives only new events the next time it's run - Prevents message from being executed twice
      await arg.UpdateCheckpointAsync(arg.CancellationToken);
      Debug.WriteLine(data);
      await _eventProcessor.BeginProcessAsync(notificationServiceEvent);
      Thread.Sleep(random.Next(200));
      await _eventProcessor.ProcessAsync(notificationServiceEvent);
      Thread.Sleep(random.Next(200));
      await _eventProcessor.EndProcessAsync(notificationServiceEvent);
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs arg)
    {
      // Write details about the error to the console window
      Console.WriteLine($"\tPartition '{ arg.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
      Console.WriteLine(arg.Exception.Message);
      return Task.CompletedTask;
    }

  }
}
