using Microsoft.EntityFrameworkCore;
using SmartMovieApp.Models;

namespace SmartMovieApp.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}
