using AppData.Infrastructures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Business.IService;

public interface IWordService
{
    public Task<IList<Word>> RetrieveAllAync();
    public Task<Word?> GetByIdAsync(string id);
    public Task<int?> AddAsync(Word Word);
    public Task<bool?> RemoveAsync(string id);
}
