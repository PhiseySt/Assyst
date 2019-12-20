using Microsoft.EntityFrameworkCore;

namespace Assyst.Models
{
    public class MobileContext : DbContext
    {
        public DbSet<EventItem> Events { get; set; }
        public MobileContext(DbContextOptions<MobileContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
