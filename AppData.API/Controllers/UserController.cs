
using AppData.API.Models;
using AppData.Business.IService;
using AppData.Infrastructures.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppData.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly TokenProvider _tokenProvider;

        public UserController(IUserService userService, TokenProvider tokenProvider)
        {
            _userService = userService;
            _tokenProvider = tokenProvider;
        }

        // Endpoint per registrare un nuovo utente
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUserAsync(model.User, model.Password);

            if (result.Succeeded)
            {
                return Ok("Registrazione avvenuta con successo.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }


        [HttpGet("FindByEmail")]
        public async Task<string> FindByEmailAsync([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "L'email non può essere vuota.";
            }

            var user = await _userService.FindByEmailAsync(email);

            if (user == null)
            {
                return "Nessun utente trovato con l'email '{email}'.";
            }
         

            string result = await _tokenProvider.Create(user);

            return result;
            
        }


        //[HttpPost("Handle")]
        //public async Task<IActionResult> Handle([FromBody] RequestModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var result = await _tokenProvider.Create(model.Email, model.Password)

        //    if (result.Succeeded)
        //    {
        //        return Ok("Registrazione avvenuta con successo.");
        //    }

        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }

        //    return BadRequest(ModelState);
        //}

    }

}
