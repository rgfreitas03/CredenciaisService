using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CredenciaisService.Context
{
    public class AplicacaoContext : IdentityDbContext
    {
        public AplicacaoContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
        }
    }
}