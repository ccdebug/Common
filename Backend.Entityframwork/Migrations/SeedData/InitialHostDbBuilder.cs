using Backend.EntityFramework;
using EntityFramework.DynamicFilters;

namespace Backend.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly BackendDbContext _context;

        public InitialHostDbBuilder(BackendDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}
