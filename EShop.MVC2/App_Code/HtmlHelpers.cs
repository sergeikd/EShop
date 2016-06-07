using EShop.ServiceLayer;
using System;
using System.IO;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace EShop.MVC2
{
    public static class HtmlHelpers
    {

        public static string ShortString (this HtmlHelper html, string text, int maxLength)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return text.Length <= maxLength ? text : string.Format("{0}...", text.Substring(0, maxLength));
            }
            return string.Empty;
        }
        public static MvcHtmlString LogoUrl(this HtmlHelper html, int? imageId)
        {
            if (!imageId.HasValue)
            {
                imageId = 10000000;
            }
            var cacheKey = string.Format("logo_{0}", imageId);
            var cache = html.ViewContext.HttpContext.Cache;
            string imgUrl;
            if (cache[cacheKey] == null)
            {
                var uh = new UrlHelper(html.ViewContext.RequestContext);
                var logoFolder = new DirectoryInfo(html.ViewContext.HttpContext.Server.MapPath("~/Content/Images"));
                if (logoFolder.Exists)
                {
                    var files = logoFolder.GetFiles(string.Format("ico_{0}.jpg", imageId));
                    if (files.Length > 0)
                    {
                        imgUrl = uh.Content(string.Concat("~/Content/Images/", files[0].Name));
                        cache.Insert(cacheKey, imgUrl, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 10));
                    }
                }
            }
            imgUrl = cache[cacheKey] as string;
            var sb = new StringBuilder();
            using (var htmlWriter = new HtmlTextWriter(new StringWriter(sb)))
            {
                var img = new HtmlImage()
                    {
                        Src = imgUrl,
                        Align = "middle",
                        Alt = "no image"
                    };
                img.RenderControl(htmlWriter);
            }
            return new MvcHtmlString(sb.ToString());
        }
        public static MvcHtmlString ImgUrl(this HtmlHelper html, int? imageId)
        {
            if (!imageId.HasValue)
            {
                imageId = 10000000;
            }
            var cacheKey = string.Format("img_{0}", imageId);
            var cache = html.ViewContext.HttpContext.Cache;
            string imgUrl;
            if (cache[cacheKey] == null)
            {
                var uh = new UrlHelper(html.ViewContext.RequestContext);
                var logoFolder = new DirectoryInfo(html.ViewContext.HttpContext.Server.MapPath("~/Content/Images"));
                if (logoFolder.Exists)
                {
                    var files = logoFolder.GetFiles(string.Format("img_{0}.jpg", imageId));
                    if (files.Length > 0)
                    {
                        imgUrl = uh.Content(string.Concat("~/Content/Images/", files[0].Name));
                        cache.Insert(cacheKey, imgUrl, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 10));
                    }
                }
            }
            imgUrl = cache[cacheKey] as string;
            var sb = new StringBuilder();
            using (var htmlWriter = new HtmlTextWriter(new StringWriter(sb)))
            {
                var img = new HtmlImage()
                {
                    Src = imgUrl,
                    Align = "middle",
                    Alt = "no image"
                };
                img.RenderControl(htmlWriter);
            }
            return new MvcHtmlString(sb.ToString());
        }
    }
}