using System;

namespace Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class GrpcStreamMappingAttribute : Attribute
    {
        public readonly Type Notification;
        public readonly Type Message;
        public GrpcStreamMappingAttribute(Type notification, Type message)
        {
            Notification = notification;
            Message = message;
        }
    }
}
