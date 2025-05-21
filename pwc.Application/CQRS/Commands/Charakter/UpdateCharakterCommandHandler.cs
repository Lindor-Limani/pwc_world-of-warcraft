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

namespace pwc.Application.CQRS.Commands.Charakter
{
    public record class UpdateCharakterCommand : IRequest<CharakterDto>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<int> EquippedItemIds { get; set; } = new List<int>();
    }
    public class UpdateCharakterCommandHandler  : IRequestHandler <UpdateCharakterCommand, CharakterDto>
    {
        private readonly ICharakterRepository _charakterRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public UpdateCharakterCommandHandler(
            ICharakterRepository charakterRepository,
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _charakterRepository = charakterRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<CharakterDto> Handle(UpdateCharakterCommand request, CancellationToken cancellationToken)
        {
            var charakter = await _charakterRepository.GetByIdAsync(request.Id);
            if (charakter == null)
                throw new KeyNotFoundException($"Charakter with ID {request.Id} not found.");

            charakter.Name = request.Name;

            // Remove all current equipped items
            charakter.CharakterItems.Clear();

            // Add new equipped items
            foreach (var itemId in request.EquippedItemIds.Distinct())
            {
                var item = await _itemRepository.GetByIdAsync(itemId);
                if (item == null)
                    throw new KeyNotFoundException($"Item with ID {itemId} not found.");

                charakter.CharakterItems.Add(new CharakterItem
                {
                    CharakterId = charakter.Id,
                    ItemId = itemId,
                    Item = item
                });
            }

            await _charakterRepository.UpdateAsync(charakter);

            return _mapper.Map<CharakterDto>(charakter);
        }
    }
}
