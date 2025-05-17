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
    public class CharakterRepository : ICharakterRepository
    {
        private readonly AppDbContext _context;
        public CharakterRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task<CharakterDto> CreateCharakter(CharakterDto charakter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCharakter(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CharakterDto>> GetAllCharakter()
        {
            throw new NotImplementedException();
        }

        public Task<CharakterDto> GetCharakterById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CharakterDto> GetCharakterByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<EquipItemDTO>> GetEquipedItemsOfCharakterById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CharakterDto> UpdateCharakter(CharakterDto charakter)
        {
            throw new NotImplementedException();
        }
    }
}
