using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Backend.Core;
using Backend.Core.Authorization.Users;
using Backend.Core.MultiTenancy;
using Microsoft.AspNet.Identity;

namespace Backend.Application
{
    public class BackendAppServiceBase : ApplicationService
    {
        public UserManager UserManager { get; set; }

        public TenantManager TenantManager { get; set; }

        protected BackendAppServiceBase()
        {
            LocalizationSourceName = BackendConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId());

            if (user == null)
            {
                throw new ApplicationException("当前用户不存在");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.FindByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}