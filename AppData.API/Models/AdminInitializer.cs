using Microsoft.AspNetCore.Identity;

namespace AppData.API.Models
{
    public static class AdminInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
          
            // Creare un utente amministratore
            var adminUser = new IdentityUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com"
            };

            string adminPassword = "Admin@123";

            var user = await userManager.FindByEmailAsync(adminUser.Email);

            if (user == null)
            {
                var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdminUser.Succeeded)
                {
                    // Assegnare il ruolo "Admin" all'utente
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

    }
}
