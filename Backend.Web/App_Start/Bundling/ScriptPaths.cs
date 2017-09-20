using System.IO;
using System.Threading;
using System.Web;
using Abp.Extensions;

namespace Backend.Web.Bundling
{
    public class ScriptPaths
    {
        public static string JQuery_Validation_Localization
        {
            get
            {
                return GetLocalizationFileForjQueryValidationOrNull(Thread.CurrentThread.CurrentUICulture.Name.ToLower().Replace("-", "_"))
                       ?? GetLocalizationFileForjQueryValidationOrNull(Thread.CurrentThread.CurrentUICulture.Name.Left(2).ToLower())
                       ?? "~/lib/jquery-validation/js/localization/_messages_empty.js";
            }
        }

        private static string GetLocalizationFileForjQueryValidationOrNull(string cultureCode)
        {
            try
            {
                var relativeFilePath = "~/lib/jquery-validation/js/localization/messages_" + cultureCode + ".min.js";
                var physicalFilePath = HttpContext.Current.Server.MapPath(relativeFilePath);
                if (File.Exists(physicalFilePath))
                {
                    return relativeFilePath;
                }
            }
            catch { }

            return null;
        }
    }
}