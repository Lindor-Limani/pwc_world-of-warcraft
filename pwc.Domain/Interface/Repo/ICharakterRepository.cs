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
        Task<List<Charakter>> GetAllAsync();
        Task<Charakter?> GetByIdAsync(int id);
        Task<Charakter?> GetByNameAsync(string name);
        Task<Charakter> AddAsync(Charakter charakter);
        Task<Charakter?> UpdateAsync(Charakter charakter);
        Task<bool> DeleteAsync(int id);
        Task<Charakter> EquipItemToCharakter(int id, Item item);

        Task<List<Item>> GetEquippedItemsByCharakterIdAsync(int charakterId);
        Task<List<CharakterItem>> GetCharakterItemsAsync(int charakterId);
    }
}
