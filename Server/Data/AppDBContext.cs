using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Server.Data;
public class AppDBContext : IdentityDbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }
}
