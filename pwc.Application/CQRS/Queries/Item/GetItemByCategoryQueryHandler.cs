using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Queries.Item
{
    public record class GetItemByCategoryQuery(
        ItemCategory Category) : IRequest<List<ItemDto>>;
    public class GetItemByCategoryQueryHandler : IRequestHandler<GetItemByCategoryQuery, List<ItemDto>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        public GetItemByCategoryQueryHandler(
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<List<ItemDto>> Handle(
            GetItemByCategoryQuery request,
            CancellationToken cancellationToken)
        {
            var items = await _itemRepository.GetByCategoryAsync(request.Category);
            return _mapper.Map<List<ItemDto>>(items);
        }
    }   
}
