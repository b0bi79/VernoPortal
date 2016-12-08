using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Verno.Filters
{
    public class DeleteFileAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.OnCompleted(() => Task.Run(() =>
            {
                var filePathResult = filterContext.Result as PhysicalFileResult;
                if (filePathResult != null)
                {
                    File.Delete(filePathResult.FileName);
                }
            }));
        }
    }
}