using Microsoft.EntityFrameworkCore;

namespace ArchiToolkit.CppInteropGenerator.Data;

public class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=appconfig.db");
    }
}