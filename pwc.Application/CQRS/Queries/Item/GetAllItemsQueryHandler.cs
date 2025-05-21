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
    public record class GetAllItemsQuery() : IRequest<IEnumerable<ItemDto>>;
    public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, IEnumerable<ItemDto>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        public GetAllItemsQueryHandler(
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ItemDto>> Handle(
            GetAllItemsQuery request,
            CancellationToken cancellationToken)
        {
            var items = await _itemRepository.GetAllAsync();
            return _mapper.Map<List<ItemDto>>(items);
        }
    }   
}
