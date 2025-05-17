using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model.Enum;
using pwc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<ItemDto?> AddItemAsync(ItemDto item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemDto>> GetAllItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ItemDto?> GetItemByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ItemDto?> GetItemByNameAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemDto>> GetItemsByCategoryAsync(ItemCategory category)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemDto>> GetItemsByCharacterIdAsync(int characterId)
        {
            throw new NotImplementedException();
        }

        public Task<ItemDto?> UpdateItemAsync(ItemDto item)
        {
            throw new NotImplementedException();
        }
    }
}
