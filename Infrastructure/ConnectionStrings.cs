namespace EventHub_Notification_Service_Demo
{
  public class ConnectionStrings
  {
    public AzureEventHub AzureEventHub { get; set; }
    public AzureBlobStorage AzureBlobStorage { get; set; }
  }

  public class AzureEventHub
  {
    public string EventHubName { get; set; }
    public string ConnectionString { get; set; }
  }

  public class AzureBlobStorage {

    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
  }
}
