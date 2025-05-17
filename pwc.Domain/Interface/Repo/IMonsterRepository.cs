using pwc.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Interface.Repo
{
    public interface IMonsterRepository
    {
        Task<List<MonsterDto>> GetAllMonsters();
        Task<MonsterDto> GetMonsterById(int id);
        Task<MonsterDto> GetMonsterByName(int id);
        Task<MonsterDto> CreateMonster(MonsterDto monster);
        Task<MonsterDto> UpdateMonster(MonsterDto monster);
        Task<bool> DeleteMonster(int id);
    }
}
