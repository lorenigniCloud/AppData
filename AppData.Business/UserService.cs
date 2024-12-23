using AppData.Business.IService;
using Microsoft.AspNetCore.Identity;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // Esempio di metodo per trovare un utente per email
    public async Task<IdentityUser> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    // Esempio di metodo per creare un nuovo utente
    public async Task<IdentityResult> RegisterUserAsync(IdentityUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }
    
    public async Task<IList<string>> GetRolesAsync(IdentityUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

}