using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Exceptions;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Commands.Charakter
{
    public record class EquipItemToCharakterCommand(
        int CharakterId,
        int ItemId) : IRequest<CharakterDto>
    {
    }
    public class EquipItemToCharakterCommandHandler : IRequestHandler<EquipItemToCharakterCommand, CharakterDto>
    {
        private readonly ICharakterRepository _charakterRepository;
        private readonly IMapper _mapper;

        public EquipItemToCharakterCommandHandler(
            ICharakterRepository charakterRepository,
            IMapper mapper)
        {
            _charakterRepository = charakterRepository;
            _mapper = mapper;
        }

        public async Task<CharakterDto> Handle(
            EquipItemToCharakterCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var charakter = await _charakterRepository.EquipItemToCharakter(request.CharakterId, request.ItemId);
                return _mapper.Map<CharakterDto>(charakter);
            }
            catch (SameEquippmentTwiceException ex)
            {
                throw new SameEquippmentTwiceException("Item already equipped");
            }
            catch (CategoryAlreadyEquippedException ex)
            {
                throw new CategoryAlreadyEquippedException("Cannot equip Item of the same Category"); 
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error creating charakter", ex);
            }
        }
    }
}
