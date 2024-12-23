using AppData.Business.IService;
using AppData.Infrastructures.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppData.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class WordController : ControllerBase
{
    private readonly IWordService _wordService;

    public WordController(IWordService injectetedService)
    {
        _wordService = injectetedService;
    }


    [HttpGet("GetAllWords")]
    public async Task<IEnumerable<Word>> GetAllWords()
    {
        return await _wordService.RetrieveAllAync();
    }

    [HttpPost("CreateWord")]
    public async Task<IActionResult> CreateWordAsync([FromBody] Word word)
    {
        if (word is null) return BadRequest();

        int? isAddedWord = await _wordService.AddAsync(word);

        if (isAddedWord is null) return BadRequest();

        else return Created();
    }

    [HttpDelete("DeleteWord")]
    public async Task<IActionResult> DeleteWordAsync([FromQuery] string id)
    {
        bool? deletedWordDB = await _wordService.RemoveAsync(id);

        if (deletedWordDB.HasValue && deletedWordDB.Value) return NoContent();
        else return NotFound();
    }

    [HttpGet("GetWordById")]
    public async Task<IActionResult> GetWordByIdAsync([FromQuery] string id)
    {
        Word? foundWordDB = await _wordService.GetByIdAsync(id);

        if (foundWordDB == null) return NotFound();

        else return Ok(foundWordDB);
    }

}

