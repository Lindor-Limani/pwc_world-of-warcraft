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
    public record class GetItemByIdQuery(int Id) : IRequest<ItemDto>;
    public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        public GetItemByIdQueryHandler(
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<ItemDto> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.GetByIdAsync(request.Id);
            if (item == null)
            {
                throw new KeyNotFoundException($"Item with ID {request.Id} not found.");
            }
            return _mapper.Map<ItemDto>(item);
        }
       
    }
}
