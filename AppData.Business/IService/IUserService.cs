using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Business.IService
{
    public interface IUserService
    {
        public Task<IdentityUser> FindByEmailAsync(string email);
        public Task<IdentityResult> RegisterUserAsync(IdentityUser user, string password);
        public Task<bool> CheckPasswordAsync(IdentityUser user, string password);
        public Task<IList<string>> GetRolesAsync(IdentityUser user);


    }
}
