namespace Generator.Models
{
    internal class GrpcStreamMappingData
    {
        public string NotificationType { get; private set; }
        public string MessageType { get; private set; }

        public GrpcStreamMappingData(string notificationType, string messageType)
        {
            NotificationType = notificationType;
            MessageType = messageType;
        }
    }
}
