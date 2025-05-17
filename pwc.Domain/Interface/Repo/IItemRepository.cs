using pwc.Domain.DTOs;
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
        Task<ItemDto?> GetItemByIdAsync(int id);
        Task<ItemDto?> GetItemByNameAsync(int id);
        Task<IEnumerable<ItemDto>> GetAllItemsAsync();
        Task<ItemDto?> AddItemAsync(ItemDto item);
        Task<ItemDto?> UpdateItemAsync(ItemDto item);
        Task<bool> DeleteItemAsync(int id);
        Task<IEnumerable<ItemDto>> GetItemsByCategoryAsync(ItemCategory category);
        Task<IEnumerable<ItemDto>> GetItemsByCharacterIdAsync(int characterId);
    }
}
