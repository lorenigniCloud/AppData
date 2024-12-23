using AppData.Infrastructures.Models;
using AppData.Infrastructures.Models.IStorage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Infrastructures.Storage;

public class WordStorage : IWordStorage
{

    private AppDataSqlServerContext _context;

    public WordStorage(AppDataSqlServerContext injectedContext)
    {
        _context = injectedContext;
    }

    public async Task<IList<Word>> RetrieveAllAync()
    {
        return await _context.Words.ToListAsync();
    }

    public async Task<Word?> GetByIdAsync(string id)
    {
        return await _context.Words.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int?> AddAsync(Word Word)
    {
        _context.Words.Add(Word);
        int affected = await _context.SaveChangesAsync();

        if (affected == 1) return affected;
        else return 0;
    }

    public async Task<bool?> RemoveAsync(string id)
    {
        Word? wordDB = await _context.Words.FirstOrDefaultAsync(x => x.Id == id);

        if (wordDB is null) return null;

        _context.Words.Remove(wordDB);
        int affected = await _context.SaveChangesAsync();

        if (affected == 1) return true;
        else return null;
    }

  
}
