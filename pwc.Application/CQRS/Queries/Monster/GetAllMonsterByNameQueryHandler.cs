using AutoMapper;
using MediatR;
using pwc.Application.CQRS.Queries.Charakter;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Queries.Monster
{
    public record class GetAllMonsterByNameQuery (string name) : IRequest<IEnumerable<MonsterDto>>;
    public class GetAllMonsterByNameQueryHandler : IRequestHandler<GetAllMonsterByNameQuery, IEnumerable<MonsterDto>>
    {
        private readonly IMonsterRepository _monsterRepository;
        private readonly IMapper _mapper;
        public GetAllMonsterByNameQueryHandler(
            IMonsterRepository monsterRepository,
            IMapper mapper)
        {
            _monsterRepository = monsterRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<MonsterDto>> Handle(
            GetAllMonsterByNameQuery request,
            CancellationToken cancellationToken)
        {
            var charakter = await _monsterRepository.GetByNameAsync(request.name);
            if (charakter == null)
                throw new KeyNotFoundException($"Monster with Name {request.name} not found.");
            return _mapper.Map<List<MonsterDto>>(charakter);
        }

        
    }
}
