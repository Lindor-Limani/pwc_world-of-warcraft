using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model;
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

        public ItemRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
        }

        public async Task<Item?> AddAsync(Item item)
        {
            var entityEntry = await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return false;
            }
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetByCategoryAsync(ItemCategory category)
        {
            return await _context.Items
                .Where(i => i.Category == category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Item>> GetByCharacterIdAsync(int characterId)
        {
            return await _context.Items
                .Where(item => item.CharakterItems.Any(ci => ci.CharakterId == characterId))
                .ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            return await _context.Items.FindAsync(id);
        }

        public async Task<Item?> GetByNameAsync(string name)
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.Name == name);
        }

        public async Task<Item?> UpdateAsync(Item item)
        {
            var itemToUpdate = await _context.Items.FindAsync(item.Id);
            if (itemToUpdate == null)
            {
                return null;
            }

            // Update scalar properties
            itemToUpdate.Name = item.Name;
            itemToUpdate.Geschicklichkeit = item.Geschicklichkeit;
            itemToUpdate.Staerke = item.Staerke;
            itemToUpdate.Ausdauer = item.Ausdauer;
            itemToUpdate.Category = item.Category;

            // Save changes
            await _context.SaveChangesAsync();
            return itemToUpdate;
        }
    }
}
