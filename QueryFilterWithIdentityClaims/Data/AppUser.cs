using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace QueryFilterWithIdentityClaims.Data
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
    }
}
