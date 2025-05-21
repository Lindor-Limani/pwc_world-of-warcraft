using pwc.Domain.DTOs;
using pwc.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Interface.Repo
{
    public interface ICharakterRepository
    {
        Task<IEnumerable<Charakter>> GetAllAsync();
        Task<Charakter?> GetByIdAsync(int id);
        Task<IEnumerable<Charakter>> GetByNameAsync(string name);
        Task<Charakter> AddAsync(Charakter charakter);
        Task<Charakter?> UpdateAsync(Charakter charakter);
        Task<bool> DeleteAsync(int id);
        Task<Charakter> EquipItemToCharakter(int charakterId, int itemId);

        Task<List<Item>> GetEquippedItemsByCharakterIdAsync(int charakterId);
        Task<List<CharakterItem>> GetCharakterItemsAsync(int charakterId);
    }
}
