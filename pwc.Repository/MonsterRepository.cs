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
    public class MonsterRepository : IMonsterRepository
    {
        private readonly AppDbContext _context;
        public MonsterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Monster> AddAsync(Monster monster)
        {
            var entry = await _context.Monsters.AddAsync(monster);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var monster = await _context.Monsters.FindAsync(id);
            if (monster == null)
                return false;

            _context.Monsters.Remove(monster);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Monster>> GetAllAsync()
        {
            return await _context.Monsters
                .Include(m => m.Drops)
                .Include(m => m.MonsterItemDrops)
                .ToListAsync();
        }

        public async Task<Monster?> GetByIdAsync(int id)
        {
            return await _context.Monsters
                .Include(m => m.Drops)
                .Include(m => m.MonsterItemDrops)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Monster?> GetByNameAsync(string name)
        {
            return await _context.Monsters
                .Include(m => m.Drops)
                .Include(m => m.MonsterItemDrops)
                .FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<Monster?> UpdateAsync(Monster monster)
        {
            var existing = await _context.Monsters.FindAsync(monster.Id);
            if (existing == null)
                return null;

            existing.Name = monster.Name;
            existing.Health = monster.Health;
            existing.Damage = monster.Damage;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
