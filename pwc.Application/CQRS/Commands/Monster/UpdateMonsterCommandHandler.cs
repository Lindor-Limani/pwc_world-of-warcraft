using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Commands.Monster
{
    public record class UpdateMonsterCommand(
    int Id,
    string Name,
    int Health,
    int Damage,
    ICollection<int> DropItemIds 
) : IRequest<MonsterDto>, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Name is required", new[] { nameof(Name) });
            if (Health < 0)
                yield return new ValidationResult("Health cannot be negative", new[] { nameof(Health) });
            if (Damage < 0)
                yield return new ValidationResult("Damage cannot be negative", new[] { nameof(Damage) });
        }
    }
    public class UpdateMonsterCommandHandler : IRequestHandler<UpdateMonsterCommand, MonsterDto>
    {
        
            private readonly IMonsterRepository _monsterRepository;
            private readonly IItemRepository _itemRepository;
            private readonly IMapper _mapper;

            public UpdateMonsterCommandHandler(
                IMonsterRepository monsterRepository,
                IItemRepository itemRepository,
                IMapper mapper)
            {
                _monsterRepository = monsterRepository;
                _itemRepository = itemRepository;
                _mapper = mapper;
            }

            public async Task<MonsterDto> Handle(UpdateMonsterCommand request, CancellationToken cancellationToken)
            {
                var monster = await _monsterRepository.GetByIdAsync(request.Id);
                if (monster == null)
                    throw new KeyNotFoundException($"Monster with ID {request.Id} not found.");

                monster.Name = request.Name;
                monster.Health = request.Health;
                monster.Damage = request.Damage;

                // Drops ersetzen
                monster.MonsterItemDrops.Clear();

                foreach (var itemId in request.DropItemIds.Distinct())
                {
                    var item = await _itemRepository.GetByIdAsync(itemId);
                    if (item == null)
                        throw new KeyNotFoundException($"Item with ID {itemId} not found.");

                    monster.MonsterItemDrops.Add(new MonsterItemDrop
                    {
                        MonsterId = monster.Id,
                        ItemId = itemId,
                        Item = item,
                        Monster = monster,
                        DropChance = 1.0 // Standardwert, falls du keine DropChance im Command hast
                    });
                }

                await _monsterRepository.UpdateAsync(monster);

                return _mapper.Map<MonsterDto>(monster);
            }
        }
    
}
