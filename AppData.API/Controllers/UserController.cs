
using AppData.API.Models;
using AppData.Business.IService;
using AppData.Infrastructures.Entities;
using AppData.EmailSender.Interfaces;
using AppData.EmailSender.Services;
using Microsoft.AspNetCore.Mvc;


namespace AppData.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly TokenProvider _tokenProvider;
        protected readonly IEmailSenderService _emailSenderService;


        public UserController(IUserService userService, TokenProvider tokenProvider, IEmailSenderService emailSenderService)
        {
            _userService = userService;
            _tokenProvider = tokenProvider;
            _emailSenderService = emailSenderService;
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


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel requestModel)
        {
            // Verifica se il modello di richiesta è valido
            if (!ModelState.IsValid)
            {
                return BadRequest("Dati di login non validi.");
            }

            // Cerca l'utente tramite l'email fornita
            var user = await _userService.FindByEmailAsync(requestModel.Email);
            if (user == null)
            {
                return Unauthorized("Credenziali non valide.");
            }

            // Verifica la password
            var passwordValid = await _userService.CheckPasswordAsync(user, requestModel.Password);
            if (!passwordValid)
            {
                return Unauthorized("Credenziali non valide.");
            }

            // Genera il token JWT
            var token = await _tokenProvider.CreateTokenAsync(user);

            // Restituisce il token in una risposta di successo
            return Ok(new { Token = token });
        }

        [HttpGet("SendEmail")]
        public async Task<IActionResult> SendEmail(string body, string subject, string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dati di login non validi.");
            }

            try
            {
                await _emailSenderService.SendMailAsync(body, subject, email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to send email: {ex.Message}");
            }
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
