using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
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

        public Task<MonsterDto> CreateMonster(MonsterDto monster)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMonster(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<MonsterDto>> GetAllMonsters()
        {
            throw new NotImplementedException();
        }

        public Task<MonsterDto> GetMonsterById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<MonsterDto> GetMonsterByName(int id)
        {
            throw new NotImplementedException();
        }

        public Task<MonsterDto> UpdateMonster(MonsterDto monster)
        {
            throw new NotImplementedException();
        }
    }
}
