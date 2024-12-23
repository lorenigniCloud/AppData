using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Infrastructures.Entities
{
    public class RegisterModel
    {
        public IdentityUser User { get; set; }
        public string Password { get; set; }
    }

}
