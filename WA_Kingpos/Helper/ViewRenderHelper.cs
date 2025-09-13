using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WA_Kingpos.Helper
{
    public static class ViewRenderHelper
    {
        public static async Task<string> RenderPartialAsync(PageContext pageContext,
                                                        ITempDataProvider tempDataProvider,
                                                        IRazorViewEngine viewEngine,
                                                        string partialName,
                                                        object model)
        {
            var actionContext = new ActionContext(pageContext.HttpContext,
                                                  pageContext.RouteData,
                                                  new ActionDescriptor());

            using var sw = new StringWriter();

            var viewEngineResult = viewEngine.GetView(null, partialName, isMainPage: false);
            if (!viewEngineResult.Success)
                throw new InvalidOperationException($"Partial '{partialName}' not found.");

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), pageContext.ModelState)
            {
                Model = model
            };

            var tempData = new TempDataDictionary(actionContext.HttpContext, tempDataProvider);

            var viewContext = new ViewContext(actionContext,
                                              viewEngineResult.View,
                                              viewDictionary,
                                              tempData,
                                              sw,
                                              new HtmlHelperOptions());

            await viewEngineResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }

}

