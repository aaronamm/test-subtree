using System;
using System.Web.Mvc;

namespace CodeSanook.Common.Web
{
    public static class MvcHelper
    {
        public static string GetControllerName<T>() where T : Controller
            => typeof(T).Name.Replace("Controller", "");

        public static string GetUrlPathRelativeToModule(
            this WebViewPage page,
            string pathInModule)
        {
            var viewPath = page.VirtualPath;
            if (string.IsNullOrEmpty(viewPath))
            {
                return null;
            }

            string basePath = null;
            var viewsPartIndex = viewPath.IndexOf("/Views", StringComparison.OrdinalIgnoreCase);
            if (viewsPartIndex >= 0)
            {
                basePath = page.Combine(viewPath.Substring(0, viewsPartIndex + 1), pathInModule);
            }

            return page.Url.Content(basePath);
        }

        public static string Combine(this WebViewPage page, params string[] paths)
        {
            return Flurl.Url.Combine(paths);
        }
    }
}