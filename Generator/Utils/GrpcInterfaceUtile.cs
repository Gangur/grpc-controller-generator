using Generator.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Generator.Utils
{
    internal static class GrpcInterfaceUtile
    {
        public static GrpcInterfaceData Convert(ImmutableArray<TypedConstant> typedConstants)
        {
            return new GrpcInterfaceData(
                typedConstants[0].Value.ToString(), 
                typedConstants[1].Value.ToString());
        }
    }
}
