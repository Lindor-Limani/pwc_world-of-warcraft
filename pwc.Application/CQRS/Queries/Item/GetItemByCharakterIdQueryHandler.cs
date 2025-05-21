using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Queries.Item
{
    public record class GetItemByCharakterIdQuery(int CharakterId) : IRequest<List<ItemDto>>;
    public class GetItemByCharakterIdQueryHandler : IRequestHandler<GetItemByCharakterIdQuery, List<ItemDto>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICharakterRepository _charakterRepository;
        private readonly IMapper _mapper;
        public GetItemByCharakterIdQueryHandler(
            IItemRepository itemRepository, ICharakterRepository charakterRepository,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _charakterRepository = charakterRepository;
            _mapper = mapper;
        }
        public async Task<List<ItemDto>> Handle(
            GetItemByCharakterIdQuery request,
            CancellationToken cancellationToken)
        {
            var charakterExists = await _charakterRepository.GetByIdAsync(request.CharakterId);
            if (charakterExists == null)
            {
                throw new KeyNotFoundException($"Character with ID {request.CharakterId} not found");
            }

            var items = await _itemRepository.GetByCharacterIdAsync(request.CharakterId);
            if (items == null || !items.Any())
            {
                throw new KeyNotFoundException($"No items found for character with ID {request.CharakterId}");
            }
            return _mapper.Map<List<ItemDto>>(items);
        }
    }
}
