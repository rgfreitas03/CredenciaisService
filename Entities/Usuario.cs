using System;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace CredenciaisService.Entidades
{
    public class Usuario : IdentityUser
    {
        public ClaimsIdentity Identidade()
            => new ClaimsIdentity(new GenericIdentity(this.Id));
    }
}