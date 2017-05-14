using Microsoft.EntityFrameworkCore;
using Sinx.UnitTest.Domain.Model;

namespace Sinx.UnitTest.Infrastructure.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) :
            base(dbContextOptions)
        {
        }

        public DbSet<BrainstormSession> BrainstormSessions { get; set; }
    }
}