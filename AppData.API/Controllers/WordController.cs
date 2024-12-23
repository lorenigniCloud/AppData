using AppData.Business.IService;
using AppData.Infrastructures.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppData.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordController : ControllerBase
    {
        private readonly IWordService _wordService;

        public WordController(IWordService injectedService)
        {
            _wordService = injectedService;
        }

        // Accessibile sia dagli utenti che dagli amministratori
        [HttpGet("GetAllWords")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<IEnumerable<Word>>> GetAllWords()
        {
            var words = await _wordService.RetrieveAllAsync();
            return Ok(words);
        }

        // Accessibile solo dagli amministratori
        [HttpPost("CreateWord")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateWordAsync([FromBody] Word word)
        {
            if (word == null)
            {
                return BadRequest("Il parametro 'word' non può essere null.");
            }

            var isAddedWord = await _wordService.AddAsync(word);

            if (isAddedWord == null)
            {
                return StatusCode(500, "Errore durante la creazione della parola.");
            }

            return CreatedAtAction(nameof(GetWordByIdAsync), new { id = word.Id }, word);
        }

        // Accessibile solo dagli amministratori
        [HttpDelete("DeleteWord")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteWordAsync([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("L'ID non può essere null o vuoto.");
            }

            var deletedWordDB = await _wordService.RemoveAsync(id);

            if (deletedWordDB.HasValue && deletedWordDB.Value)
            {
                return NoContent();
            }
            else
            {
                return NotFound($"Parola con ID '{id}' non trovata.");
            }
        }

       
        [HttpGet("GetWordById")]
        public async Task<IActionResult> GetWordByIdAsync([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("L'ID non può essere null o vuoto.");
            }

            var foundWordDB = await _wordService.GetByIdAsync(id);

            if (foundWordDB == null)
            {
                return NotFound($"Parola con ID '{id}' non trovata.");
            }

            return Ok(foundWordDB);
        }


    
    }
}
