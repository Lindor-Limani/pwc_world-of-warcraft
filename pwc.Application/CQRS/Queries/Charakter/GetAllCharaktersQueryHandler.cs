using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Queries.Charakter
{
    public record class GetAllCharakterQuery : IRequest<IEnumerable<CharakterDto>>;
    public class GetAllCharaktersQueryHandler : IRequestHandler<GetAllCharakterQuery, IEnumerable<CharakterDto>>
    {
        private readonly ICharakterRepository _charakterRepository;
        private readonly IMapper _mapper;
        public GetAllCharaktersQueryHandler(
            ICharakterRepository charakterRepository,
            IMapper mapper)
        {
            _charakterRepository = charakterRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CharakterDto>> Handle(GetAllCharakterQuery request, CancellationToken cancellationToken)
        {
            var charakters = await _charakterRepository.GetAllAsync();
            return _mapper.Map<List<CharakterDto>>(charakters);
        }
    }
}
