using pwc.Domain.DTOs;
using pwc.Domain.Model;
using pwc.Domain.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Interface.Repo
{
    public interface IItemRepository
    {
        Task<Item?> GetByIdAsync(int id);
        Task<IEnumerable<Item>> GetByNameAsync(string name);
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item?> AddAsync(Item item);
        Task<Item?> UpdateAsync(Item item);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Item>> GetByCategoryAsync(ItemCategory category);
        Task<IEnumerable<Item>> GetByCharacterIdAsync(int characterId);
    }
}
