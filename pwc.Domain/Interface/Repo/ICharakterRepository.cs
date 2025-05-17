using pwc.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Domain.Interface.Repo
{
    public interface ICharakterRepository
    {
        Task<List<CharakterDto>> GetAllCharakter();
        Task<List<EquipItemDTO>> GetEquipedItemsOfCharakterById(int id);
        Task<CharakterDto> GetCharakterById(int id);
        Task<CharakterDto> GetCharakterByName(string name);
        Task<CharakterDto> CreateCharakter(CharakterDto charakter);
        Task<CharakterDto> UpdateCharakter(CharakterDto charakter);
        Task<bool> DeleteCharakter(int id);
    }
}
