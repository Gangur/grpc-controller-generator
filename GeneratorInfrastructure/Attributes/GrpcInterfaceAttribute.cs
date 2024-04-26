using System;

namespace Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GrpcInterfaceAttribute : Attribute
    {
        public readonly Type InterfaceType;
        public readonly string InterfaceMethod;

        public GrpcInterfaceAttribute(Type interfaceType, string interfaceMethod = default)
        {
            InterfaceType = interfaceType;
            InterfaceMethod = interfaceMethod;
        }
    }
}
