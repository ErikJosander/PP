using System;
using System.Data.Entity;
using ClassLibrary1.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Data
{
    public class Context : DbContext
    {
        public Context()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<Context>());
        }
        public DbSet<Space> Spaces { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<TempVehicle> TempVehicles { get; set; }

    }
}
