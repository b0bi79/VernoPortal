using System.Reflection;
using Castle.Facilities.TypedFactory;
using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    public class ReportGeneratorTypedFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            if (method.Name == "Create" && arguments.Length == 1 && arguments[0] is ReportOutFormat)
            {
                var format = (ReportOutFormat)arguments[0];
                return format.OutFormat.GenerateUtil.ToLower();
            }
            return base.GetComponentName(method, arguments);
        }
    }
}