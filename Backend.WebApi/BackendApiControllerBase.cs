using Abp.WebApi.Controllers;
using Backend.Core;

namespace Backend.WebApi
{
    public class BackendApiControllerBase : AbpApiController
    {
        public BackendApiControllerBase()
        {
            LocalizationSourceName = BackendConsts.LocalizationSourceName;
        }
    }
}