using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using auth_service.Models;

namespace auth_service.Interfaces
{
    public interface IJwtTokenGenerator
    {
        /// <summary>Generates a token for a given app user and a list of roles.</summary>
        /// <param name="AppUser">Representing the user for whom the token is being
        /// generated.</param>
        /// <param name="roles">Representing the roles assigned to the user.</param>
        string GenerateToken(AppUser appUser, IEnumerable<string> roles);
    }
}