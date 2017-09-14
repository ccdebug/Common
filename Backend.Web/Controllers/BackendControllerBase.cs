using Abp.UI;
using Abp.Web.Mvc.Controllers;
using Backend.Core;
using Microsoft.AspNet.Identity;

namespace Backend.Web.Controllers
{
    public abstract class BackendControllerBase : AbpController
    {
        protected BackendControllerBase()
        {
            LocalizationSourceName = BackendConsts.LocalizationSourceName;
        }

        protected virtual void CheckModelState()
        {
            if (!ModelState.IsValid)
            {
                throw new UserFriendlyException(L("FormIsNotValidMessage"));
            }
        }
    }
}