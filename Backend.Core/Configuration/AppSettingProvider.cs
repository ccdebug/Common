using System.Collections.Generic;
using Abp.Configuration;

namespace Backend.Core.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.DefaultLanguageName, "zh-CN", scopes: SettingScopes.Tenant)
            };
        }
    }
}