using General.Data.Types;
using Microsoft.EntityFrameworkCore;

namespace General.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();

    public DbSet<User> Users => Set<User>();
}