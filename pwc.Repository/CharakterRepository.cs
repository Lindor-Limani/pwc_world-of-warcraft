using Microsoft.EntityFrameworkCore;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model;
using pwc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Repository
{
    public class CharakterRepository : ICharakterRepository
    {
        private readonly AppDbContext _context;
        public CharakterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Charakter> AddAsync(Charakter charakter)
        {
            var entry = await _context.Characters.AddAsync(charakter);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var charakter = await _context.Characters.FindAsync(id);
            if (charakter == null)
                return false;

            _context.Characters.Remove(charakter);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Charakter> EquipItemToCharakter(int charakterId, Item item)
        {
            var charakter = await _context.Characters
                .Include(c => c.CharakterItems)
                .FirstOrDefaultAsync(c => c.Id == charakterId);

            if (charakter == null)
                throw new KeyNotFoundException($"Charakter with ID {charakterId} not found.");

            var itemEntity = await _context.Items.FindAsync(item.Id);
            if (itemEntity == null)
                throw new KeyNotFoundException($"Item with ID {item.Id} not found.");

            bool alreadyEquipped = charakter.CharakterItems.Any(ci => ci.ItemId == item.Id);
            bool categoryAlreadyEquipped = charakter.CharakterItems.Any(ci => ci.Item.Category == item.Category);

            if (alreadyEquipped)
                throw new Exception($"Item with ID {item.Id} is already equipped.");

            if (categoryAlreadyEquipped)
                throw new Exception($"An item of category {item.Category} is already equipped.");

            var charakterItem = new CharakterItem
            {
                CharakterId = charakterId,
                ItemId = item.Id
            };
            charakter.CharakterItems.Add(charakterItem);
            await _context.SaveChangesAsync();

            return charakter;
        }

        public async Task<List<Charakter>> GetAllAsync()
        {
            return await _context.Characters
                .Include(c => c.CharakterItems)
                .ThenInclude(ci => ci.Item)
                .ToListAsync();
        }

        public async Task<Charakter?> GetByIdAsync(int id)
        {
            return await _context.Characters
                .Include(c => c.CharakterItems)
                .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Charakter?> GetByNameAsync(string name)
        {
            return await _context.Characters
                .Include(c => c.CharakterItems)
                .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<List<CharakterItem>> GetCharakterItemsAsync(int charakterId)
        {
            return await _context.CharacterItems
                .Include(ci => ci.Item)
                .Where(ci => ci.CharakterId == charakterId)
                .ToListAsync();
        }

        public async Task<List<Item>> GetEquippedItemsByCharakterIdAsync(int charakterId)
        {
            return await _context.CharacterItems
                .Where(ci => ci.CharakterId == charakterId)
                .Include(ci => ci.Item)
                .Select(ci => ci.Item)
                .ToListAsync();
        }

        public async Task<Charakter?> UpdateAsync(Charakter charakter)
        {
            var existing = await _context.Characters.FindAsync(charakter.Id);
            if (existing == null)
                return null;

            existing.Name = charakter.Name;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
