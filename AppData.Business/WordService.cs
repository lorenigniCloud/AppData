using AppData.Business.IService;
using AppData.Infrastructures.Models;
using AppData.Infrastructures.Models.IStorage;


namespace AppData.Business;

public class WordService : IWordService
{
    private readonly IWordStorage _wordStorage;
    public WordService(IWordStorage wordStorage)
    {
        _wordStorage = wordStorage;
    }

    public async Task<IList<Word>?> RetrieveAllAsync()
    {
        return await _wordStorage.RetrieveAllAync();
    }
    public async Task<Word?> GetByIdAsync(string id)
    {
        return await _wordStorage.GetByIdAsync(id);
    }
    public async Task<int?> AddAsync(Word word)
    {
        return await _wordStorage.AddAsync(word);
    }
    public async Task<bool?> RemoveAsync(string id)
    {
        return await _wordStorage.RemoveAsync(id);
    }

}
