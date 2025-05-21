using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Queries.Monster
{
    public record class GetMonsterByIdQuery (int id) : IRequest<MonsterDto>;
    
    public class GetMonsterByIdQueryHandler : IRequestHandler<GetMonsterByIdQuery, MonsterDto>
    {
        private readonly IMonsterRepository _monsterRepository;
        private readonly IMapper _mapper;
        public GetMonsterByIdQueryHandler(
            IMonsterRepository monsterRepository,
            IMapper mapper)
        {
            _monsterRepository = monsterRepository;
            _mapper = mapper;
        }
        public async Task<MonsterDto> Handle(
            GetMonsterByIdQuery request,
            CancellationToken cancellationToken)
        {
            var monster = await _monsterRepository.GetByIdAsync(request.id);
            if (monster == null)
                throw new KeyNotFoundException($"Monster with ID {request.id} not found.");
            return _mapper.Map<MonsterDto>(monster);
        }
    }
}
