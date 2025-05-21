using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Commands.Monster
{
    public record class AddDropToMonsterCommand(
        int MonsterId,
        int ItemId, double DropChance)
        : IRequest<MonsterDto>
    {
    }
    public class AddDropToMonserCommandHandler : IRequestHandler<AddDropToMonsterCommand, MonsterDto>
    {
        private readonly IMonsterRepository _monsterRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        public AddDropToMonserCommandHandler(
            IMonsterRepository monsterRepository,
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _monsterRepository = monsterRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<MonsterDto> Handle(AddDropToMonsterCommand request, CancellationToken cancellationToken)
        {
            // Prüfe, ob das Item existiert
            var item = await _itemRepository.GetByIdAsync(request.ItemId);
            if (item == null)
                throw new KeyNotFoundException($"Item with ID {request.ItemId} not found.");

            var monsterE = await _monsterRepository.GetByIdAsync(request.MonsterId);
            if (monsterE == null)
                throw new KeyNotFoundException($"Monster with ID {request.MonsterId} not found.");
            // Drop zum Monster hinzufügen (inkl. Validierung im Repo)
            var monster = await _monsterRepository.AddItemDropAsync(request.MonsterId, request.ItemId, request.DropChance);

            // Mapping zu DTO
            return _mapper.Map<MonsterDto>(monster);
        }
    }
}
