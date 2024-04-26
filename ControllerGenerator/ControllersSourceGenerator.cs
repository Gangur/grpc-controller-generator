using Generator.Builders;
using Generator.Utils;
using Infrastructure.Attributes;
using Infrastructure.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Generator
{
    [Generator]
    public class ControllersSourceGenerator : ISourceGenerator
    {
        private static readonly string _controllerMappingAttributeName = nameof(GrpcMappingAttribute).Substring(0, nameof(GrpcMappingAttribute).Length - nameof(Attribute).Length);

        private static readonly string _methodMappingAttributeName = nameof(GrpcInterfaceAttribute).Substring(0, nameof(GrpcInterfaceAttribute).Length - nameof(Attribute).Length);
        private static readonly string _streamMappingAttributeName = nameof(GrpcStreamMappingAttribute).Substring(0, nameof(GrpcStreamMappingAttribute).Length - nameof(Attribute).Length);

        private static readonly HashSet<string> _technicalGRpsFields = new HashSet<string>() { "Parser", "Descriptor" };

        private SyntaxTree[] _syntaxTrees;

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif

            _syntaxTrees = context.Compilation
                .SyntaxTrees.ToArray();

            for (int i = 0; i < _syntaxTrees.Length; i++)
            {
                var semanticModel = context.Compilation.GetSemanticModel(_syntaxTrees[i]);
                var declaredClasses = _syntaxTrees[i]
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .ToArray();

                for (int j = 0; j < declaredClasses.Length; j++)
                {
                    if (declaredClasses[j].AttributeLists
                            .SelectMany(al => al.Attributes)
                                .Any(a => a.Name.GetText().ToString() == _controllerMappingAttributeName))
                    {
                        try
                        {
                            context.AddSource($"{declaredClasses[j].Identifier.ValueText}.g.cs", buildController(declaredClasses[j], semanticModel));
                        }
                        catch (FileNotFoundException) // Если gRPC_Objects еще не сгенерировалась
                        {
                            Execute(context);
                            return;
                        }
                        catch(Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }

        private string buildController(ClassDeclarationSyntax controller, SemanticModel semanticModel)
        {
            var methods = controller
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToArray();

            var controllerBuilder = new ControllerBuilder(controller.Identifier.ValueText);

            for (int j = 0; j < methods.Length; j++)
            {
                var method = semanticModel.GetDeclaredSymbol(methods[j]);

                var methodAtrributes = method.GetAttributes();

                var attribute = methodAtrributes
                    .FirstOrDefault(a => a.AttributeClass.Name == nameof(GrpcInterfaceAttribute));

                if (attribute != default)
                {
                    var streamAtrributes = methodAtrributes
                        .Where(a => a.AttributeClass.Name == nameof(GrpcStreamMappingAttribute))
                        .ToArray();

                    var constructorArguments = attribute.ConstructorArguments;

                    var attributeData = GrpcInterfaceUtile.Convert(constructorArguments);
                    var serviceName = attributeData.ServiceType.GetServiceName().ToLower();

                    if (!controllerBuilder.ContainesDependensy(attributeData.ServiceType))
                    {
                        controllerBuilder.AddDependensy(attributeData.ServiceType, serviceName);
                    }

                    controllerBuilder.AddMethod(
                        buildMethed(method,
                        streamAtrributes,
                        serviceName, attributeData.ServiceMethodName));
                }
            }

            return controllerBuilder.ToString();
        }

        private MethodBuilder buildMethed(
            IMethodSymbol method,
            AttributeData[] streamAtrributes,
            string serviceName, 
            string serviceMethodName)
        {
            if (string.IsNullOrEmpty(serviceMethodName))
            {
                serviceMethodName = method.MetadataName;
            }

            var requestType = method.Parameters[0].Type;
            var fields = requestType.GetMembers()
                .Where(f => f.Kind == SymbolKind.Property &&
                    f.DeclaredAccessibility == Accessibility.Public &&
                    !_technicalGRpsFields.Contains(f.Name))
                .Select(f => "request." + f.Name)
                .ToArray();

            string requestTypeName = requestType.Name;
            

            if (streamAtrributes.Any())
            {
                var responseTypeName = method
                    .Parameters[1].Type
                    .ToDisplayString()
                    .GetResponseGenericTypeName();

                var streamMethodBuilder = new StreamMethodBuilder(method.MetadataName,
                    serviceName,
                    serviceMethodName,
                    responseTypeName,
                    requestTypeName,
                    fields);

                for (int i = 0; i < streamAtrributes.Length; i++)
                {                    
                    var constructorArguments = streamAtrributes[i].ConstructorArguments;
                    var attributeData = GrpcStreamMappingUtile.Convert(constructorArguments);

                    streamMethodBuilder.AddNotificationCase(attributeData.NotificationType, 
                        getConstructorParameters(attributeData.NotificationType),
                        attributeData.MessageType);
                }

                return streamMethodBuilder;
            }
            else
            {
                string responseTypeName = method.ReturnType
                    .ToDisplayString()
                    .GetResponseGenericTypeName();

                return new MethodBuilder(method.MetadataName,
                    serviceName,
                    serviceMethodName,
                    responseTypeName,
                    requestTypeName,
                    fields);
            }
        }

        private string[] getConstructorParameters(string className)
        {
            className = className.GetServiceName();
            var declaredClasses = _syntaxTrees.SelectMany(st => st
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .ToArray())
                .Where(dc => dc.Identifier.ValueText == className)
                .Single();

            var properties = declaredClasses
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .ToArray();

            return properties.Select(p => p.Identifier.ValueText).ToArray();
        }

        public void Initialize(GeneratorInitializationContext context)
        {

        }
    }
}
