using System.Reflection;
using Castle.Facilities.TypedFactory;
using Verno.Reports.Models;

namespace Verno.Reports.DataSource
{
    public class DataSourceTypedFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            if (method.Name == "Create" && arguments.Length == 1 && arguments[0] is Report)
            {
                var report = (Report)arguments[0];
                return report.SqlFile != null ? "sqlfile" : "sqlproc";
            }
            return base.GetComponentName(method, arguments);
        }
    }
}