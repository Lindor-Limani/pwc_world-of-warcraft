using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Commands.Monster
{
    public record class CreateMonsterCommand(
    string Name,
    int Health,
    int Damage
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
    public class CreateMonsterCommandHandler : IRequestHandler<CreateMonsterCommand, MonsterDto>
    {
        private readonly IMonsterRepository _monsterRepository;
        private readonly IMapper _mapper;
        public CreateMonsterCommandHandler(
            IMonsterRepository monsterRepository,
            IMapper mapper)
        {
            _monsterRepository = monsterRepository;
            _mapper = mapper;
        }
        public async Task<MonsterDto> Handle(
            CreateMonsterCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var monster = _mapper.Map<Domain.Model.Monster>(request);
                await _monsterRepository.AddAsync(monster);
                return _mapper.Map<MonsterDto>(monster);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error creating monster", ex);
            }
        }
    }
}
