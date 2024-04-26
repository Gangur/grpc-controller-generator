using Generator.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Generator.Utils
{
    internal static class GrpcStreamMappingUtile
    {
        public static GrpcStreamMappingData Convert(ImmutableArray<TypedConstant> typedConstants)
        {
            return new GrpcStreamMappingData(
                typedConstants[0].Value.ToString(),
                typedConstants[1].Value.ToString());
        }
    }
}
