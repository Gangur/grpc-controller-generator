using Infrastructure.Attributes;
using Infrastructure.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Generator
{
    [Generator]
    public class ControllersSourceGenerator : ISourceGenerator
    {
        private const string _namespace = "Controllers";

        private static readonly string _controllerMapping = typeof(GrpcMappingAttribute).FullName;
        private static readonly string _controllerMappingShort = nameof(GrpcMappingAttribute).Substring(0, nameof(GrpcMappingAttribute).Length - nameof(Attribute).Length);

        private static readonly string _methodMapping = typeof(GrpcInterfaceAttribute).FullName;
        private static readonly string _methodMappingShort = nameof(GrpcInterfaceAttribute).Substring(0, nameof(GrpcInterfaceAttribute).Length - nameof(Attribute).Length);

        private static readonly HashSet<string> _technicalFields = new HashSet<string>() { "Parser", "Descriptor" };

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif

            var attributeSymbol = context.Compilation.GetTypeByMetadataName(_controllerMapping);

            var classes = context.Compilation
                .SyntaxTrees.ToArray();

            for (int i = 0; i < classes.Length; i++)
            {
                var semanticModel = context.Compilation.GetSemanticModel(classes[i]);
                var declaredClasses = classes[i]
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .ToArray();

                for (int j = 0; j < declaredClasses.Length; j++)
                {
                    if (declaredClasses[j].AttributeLists
                            .SelectMany(al => al.Attributes)
                                .Any(a => a.Name.GetText().ToString() == _controllerMappingShort))
                    {
                        context.AddSource($"{declaredClasses[j].Identifier.ValueText}.g.cs", buildController(declaredClasses[j], semanticModel));
                    }
                }
            }
        }

        private string buildController(ClassDeclarationSyntax controller, SemanticModel semanticModel)
        {
            var methods = controller
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                            .SelectMany(al => al.Attributes)
                                .Any(a => a.Name.GetText().ToString() == _methodMappingShort))
                .ToArray();

            string controllerName = controller.Identifier.ValueText;

            return
$@"using Grpc.Core;
namespace Server.{_namespace}
{{
    internal partial class {controllerName} 
    {{
        {buildControllerCore(methods, semanticModel, controllerName)}
    }}
}};";
        }
        
        private string buildControllerCore(MethodDeclarationSyntax[] methods, SemanticModel semanticModel, string controllerName)
        {
            var fildsBuilder = new StringBuilder();
            var parametersBuilder = new StringBuilder();
            var assignmentsBuilder = new StringBuilder();
            var methodsBuilder = new StringBuilder();
            var services = new HashSet<string>();

            for (int j = 0; j < methods.Length; j++)
            {
                var method = semanticModel.GetDeclaredSymbol(methods[j]);

                var attribute = method.GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass.Name == nameof(GrpcInterfaceAttribute));

                var members = attribute.ConstructorArguments;
                var serviceType = members[0].Value.ToString();
                var serviceName = serviceType.GetServiceName().ToLower();

                if (!services.Contains(serviceName))
                {
                    fildsBuilder.Append($"private readonly {serviceType} _{serviceName};");
                    parametersBuilder.Append($"{serviceType} {serviceName},");
                    assignmentsBuilder.Append($"_{serviceName} = {serviceName};");

                    services.Add(serviceName);
                }
                
                methodsBuilder.Append(buildMethed(method, serviceName, members[1].Value.ToString()));
            }

            return $@"{fildsBuilder}
        
        public {controllerName}({parametersBuilder.ToString().TrimEnd(',')})
        {{
            {assignmentsBuilder}
        }}
        {methodsBuilder}
            ";
        }

        private string buildMethed(IMethodSymbol method, string serviceName, string serviceMethodName)
        {
            if (string.IsNullOrEmpty(serviceMethodName))
            {
                serviceMethodName = method.MetadataName;
            }

            var requestType = method.Parameters[0].Type;
            var fields = requestType.GetMembers()
                .Where(f => f.Kind == SymbolKind.Property && 
                    f.DeclaredAccessibility == Accessibility.Public &&
                    !_technicalFields.Contains(f.Name))
                .Select(f => "request." + f.Name)
                .ToArray();
            
            string requestTypeName = requestType.Name;
            string responseTypeName = method.ReturnType
                .ToDisplayString()
                .GetResponseGenericTypeName();

            return $@"
        public async override partial Task<{responseTypeName}> {method.MetadataName}(
            {requestTypeName} request,
            ServerCallContext context)
        {{
            var result = await _{serviceName}.{serviceMethodName}(
                {string.Join(", ", fields)},
                context.CancellationToken);
        
            return new {responseTypeName}()
            {{
                Message = result
            }};
        }}";
        }

        public void Initialize(GeneratorInitializationContext context)
        {

        }
    }
}
