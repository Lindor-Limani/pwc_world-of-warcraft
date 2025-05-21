using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Queries.Item
{
    public record class GetItemByNameQuery(string Name) : IRequest<IEnumerable<ItemDto>>;
    public class GetItemByNameQueryHandler : IRequestHandler<GetItemByNameQuery, IEnumerable<ItemDto>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper; 
        public GetItemByNameQueryHandler(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ItemDto>> Handle(GetItemByNameQuery request, CancellationToken cancellationToken)
        {
            var items = await _itemRepository.GetByNameAsync(request.Name);
            if (items == null || !items.Any())
            {
                throw new KeyNotFoundException($"No items found with the name '{request.Name}'.");
            }
            return _mapper.Map<List<ItemDto>>(items);
        }
    }   
}
