using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Backend.Core.Authorization.Users;
using Backend.Core.Editions;

namespace Backend.Core.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore
        )
            : base(
                tenantRepository,
                tenantFeatureRepository,
                editionManager,
                featureValueStore
            )
        {
        }
    }
}