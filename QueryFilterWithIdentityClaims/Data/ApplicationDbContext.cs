using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

// You can add navigation properties without adding a new migration:
// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.1#add-navigation-properties


namespace QueryFilterWithIdentityClaims.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        private readonly bool _isAdmin;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            var requestPath = httpContextAccessor.HttpContext?.Request.Path ?? PathString.Empty;
            _isAdmin = requestPath.StartsWithSegments(new PathString("/admin"), StringComparison.OrdinalIgnoreCase);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                b.HasQueryFilter(u =>
                    _isAdmin
                        ? u.Claims.Any(c => c.ClaimType == "admin")
                        : u.Claims.Any(c => c.ClaimType != "admin") || !u.Claims.Any());
            });
        }
    }
}
