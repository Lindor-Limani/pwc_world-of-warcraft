using pwc.Domain.DTOs;
using pwc.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Interface.Repo
{
    public interface IMonsterRepository
    {
        Task<IEnumerable<Monster>> GetAllAsync();
        Task<Monster?> GetByIdAsync(int id);
        Task<IEnumerable<Monster>> GetByNameAsync(string name);
        Task<Monster> AddAsync(Monster monster);
        Task<Monster?> UpdateAsync(Monster monster);
        Task<bool> DeleteAsync(int id);
        Task<Monster> AddItemDropAsync(int monsterId, int itemId, double dropChance);

    }
}
