using System.Linq;
using System.Text;
using Infrastructure.Extensions;

namespace Generator.Builders
{
    internal class StreamMethodBuilder : MethodBuilder
    {
        private StringBuilder _notifications;
        public StreamMethodBuilder(string methodName,
            string serviceName,
            string serviceMethodName,
            string responseTypeName,
            string requestTypeName,
            string[] fields) : base(methodName, serviceName, serviceMethodName, responseTypeName, requestTypeName, fields)
        {
            _notifications = new StringBuilder();
        }

        public StreamMethodBuilder AddNotificationCase(
            string notificationType,
            string[] notificationParametrs,
            string messageType)
        {
            notificationType = notificationType.GetServiceName();
            var notification = notificationType.ToLower();

            notificationParametrs = notificationParametrs
                .Select(p => $"{notification}.{p}")
                .ToArray();

            _notifications.Append($@"
                            case {notificationType} {notification}:
                                await response.WriteAsync(new {messageType}(
                                    {string.Join(", ", notificationParametrs)}
                                ));
                            break;");

            return this;
        }

        public override string ToString()
        {
            return $@"
       public async override partial Task {_methodName}(
            {_requestTypeName} request, 
            IServerStreamWriter<{_responseTypeName}> response, 
            ServerCallContext context)
        {{
            await foreach(var notification in _{_serviceName}.{_serviceMethodName}(
                    {string.Join(", ", _fields)},
                    context.CancellationToken))
                    {{
                        switch(notification)
                        {{{_notifications}
                            default:
                                throw new InvalidCastException($""Unexpected notification type {{notification.GetType().FullName}}"");
                        }}
                    }}
        }}
";
        }
    }
}
