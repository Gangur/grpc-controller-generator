namespace Generator.Models
{
    internal class GrpcInterfaceData
    {
        public string ServiceType { get; private set; }
        public string ServiceMethodName { get; private set; }

        public GrpcInterfaceData(string serviceType, string serviceMethodName)
        {
            ServiceType = serviceType;
            ServiceMethodName = serviceMethodName;
        }
    }
}
