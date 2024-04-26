using System.Collections.Generic;
using System.Text;

namespace Generator.Builders
{
    internal class ControllerBuilder
    {
        private static string _controllerNamespace = "Controllers";
        private string _controllerName;

        private StringBuilder _dependencyFields;
        private StringBuilder _dependencyParameters;
        private StringBuilder _dependencyAssignments;

        private HashSet<string> _dependenciesSet;

        private List<MethodBuilder> _methods;

        public ControllerBuilder(string controllerName)
        {
            _controllerName = controllerName;

            _dependencyFields = new StringBuilder();
            _dependencyParameters = new StringBuilder();
            _dependencyAssignments = new StringBuilder();
            
            _dependenciesSet = new HashSet<string>();

            _methods = new List<MethodBuilder>();
        }

        public ControllerBuilder AddDependensy(string serviceType, string serviceName)
        {
            _dependencyFields.Append($"private readonly {serviceType} _{serviceName}; \n\r");
            _dependencyParameters.Append($"{serviceType} {serviceName},");
            _dependencyAssignments.Append($"_{serviceName} = {serviceName}; \n\r");

            _dependenciesSet.Add(serviceType);

            return this;
        }

        public ControllerBuilder AddMethod(MethodBuilder method)
        {
            _methods.Add(method);
            return this;
        }

        public bool ContainesDependensy(string dependensy)
            => _dependenciesSet.Contains(dependensy);

        public override string ToString()
        {
            int n = _methods.Count;
            var methodsBuilder = new StringBuilder(n);

            for (int i = 0; i < n; i++)
            {
                methodsBuilder.Append(_methods[i].ToString());
            }

            return
$@"using Grpc.Core;
using Server.Notifications;

namespace Server.{_controllerNamespace}
{{
    internal partial class {_controllerName} 
    {{
        {_dependencyFields.ToString().TrimEnd('\n').TrimEnd('\r')}
        public {_controllerName}({_dependencyParameters.ToString().TrimEnd(',')})
        {{
            {_dependencyAssignments.ToString().TrimEnd('\n').TrimEnd('\r')}
        }}
        {methodsBuilder}
    }}
}};";
        }
    }
}
