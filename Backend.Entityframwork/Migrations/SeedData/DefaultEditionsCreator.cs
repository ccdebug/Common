using Abp.Application.Editions;
using Backend.Core.Editions;
using Backend.EntityFramework;
using System.Linq;

namespace Backend.Migrations.SeedData
{
    public class DefaultEditionsCreator
    {
        private readonly BackendDbContext _context;

        public DefaultEditionsCreator(BackendDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateEditions();
        }

        private void CreateEditions()
        {
            var defaultEdition = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
            if (defaultEdition == null)
            {
                defaultEdition = new Edition { Name = EditionManager.DefaultEditionName, DisplayName = EditionManager.DefaultEditionName };
                _context.Editions.Add(defaultEdition);
                _context.SaveChanges();

                //TODO: Add desired features to the standard edition, if wanted!
            }   
        }
    }
}