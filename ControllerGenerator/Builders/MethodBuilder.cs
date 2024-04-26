namespace Generator.Builders
{
    internal class MethodBuilder
    {
        protected string _methodName;

        protected string _serviceName;
        protected string _serviceMethodName;

        protected string _responseTypeName;
        protected string _requestTypeName;

        protected string _fields;

        public MethodBuilder(string methodName,
            string serviceName, 
            string serviceMethodName,
            string responseTypeName,
            string requestTypeName,
            string[] fields)
        {
            _methodName = methodName;

            _serviceName = serviceName;
            _serviceMethodName = serviceMethodName;

            _responseTypeName = responseTypeName;
            _requestTypeName = requestTypeName;

            _fields = string.Join(", ", fields);
        }

        public override string ToString()
        {
            return $@"
        public async override partial Task<{_responseTypeName}> {_methodName}(
            {_requestTypeName} request,
            ServerCallContext context)
        {{
            var result = await _{_serviceName}.{_serviceMethodName}(
                {string.Join(", ", _fields)},
                context.CancellationToken);
        
            return new {_responseTypeName}()
            {{
                Message = result
            }};
        }}
";
        }
    }
}
