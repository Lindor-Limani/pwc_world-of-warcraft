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
    public record class GetAllMonsterQuery : IRequest<IEnumerable<MonsterDto>>;
    public class GetAllMonstersQueryHandler : IRequestHandler<GetAllMonsterQuery, IEnumerable<MonsterDto>>
    {
        private readonly IMonsterRepository _monsterRepository;
        private readonly IMapper _mapper;
        public GetAllMonstersQueryHandler(
            IMonsterRepository monsterRepository,
            IMapper mapper)
        {
            _monsterRepository = monsterRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MonsterDto>> Handle(GetAllMonsterQuery request, CancellationToken cancellationToken)
        {
            var monster = await _monsterRepository.GetAllAsync();
            return _mapper.Map<List<MonsterDto>>(monster);
        }
    }
}
