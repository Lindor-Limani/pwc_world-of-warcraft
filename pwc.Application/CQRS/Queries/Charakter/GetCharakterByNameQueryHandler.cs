using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;

namespace pwc.Application.CQRS.Queries.Charakter
{
    public record class GetCharakterByNameQuery(string Name) : IRequest<IEnumerable<CharakterDto>>;
    public class GetCharakterByNameQueryHandler : IRequestHandler<GetCharakterByNameQuery, IEnumerable<CharakterDto>>
    {
        private readonly ICharakterRepository _charakterRepository;
        private readonly IMapper _mapper;
        public GetCharakterByNameQueryHandler(
            ICharakterRepository charakterRepository,
            IMapper mapper)
        {
            _charakterRepository = charakterRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CharakterDto>> Handle(
            GetCharakterByNameQuery request,
            CancellationToken cancellationToken)
        {
            var charakter = await _charakterRepository.GetByNameAsync(request.Name);
            if (charakter == null)
                throw new KeyNotFoundException($"Charakter with Name {request.Name} not found.");
            return _mapper.Map<List<CharakterDto>>(charakter);
        }
    }
}
