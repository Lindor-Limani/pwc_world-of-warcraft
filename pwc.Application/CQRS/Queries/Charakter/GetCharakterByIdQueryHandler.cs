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
    public record class GetCharakterByIdQuery(int Id) : IRequest<CharakterDto>;
    public class GetCharakterByIdQueryHandler : IRequestHandler<GetCharakterByIdQuery, CharakterDto>
    {
        private readonly ICharakterRepository _charakterRepository;
        private readonly IMapper _mapper;
        public GetCharakterByIdQueryHandler(
            ICharakterRepository charakterRepository,
            IMapper mapper)
        {
            _charakterRepository = charakterRepository;
            _mapper = mapper;
        }
        public async Task<CharakterDto> Handle(
            GetCharakterByIdQuery request,
            CancellationToken cancellationToken)
        {
            var charakter = await _charakterRepository.GetByIdAsync(request.Id);
            if (charakter == null)
                throw new KeyNotFoundException($"Charakter with ID {request.Id} not found.");
            return _mapper.Map<CharakterDto>(charakter);
        }
    }
}
