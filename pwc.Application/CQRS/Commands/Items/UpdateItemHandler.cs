using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model.Enum;
using System.ComponentModel.DataAnnotations;
using ZstdSharp.Unsafe;

namespace pwc.Application.CQRS.Commands.Items
{

    public record class UpdateItemCommand(
        int Id, 
        string Name,
        int Geschicklichkeit,
        int Staerke,
        int Ausdauer,
        ItemCategory Category)
        : IRequest<ItemDto>, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Name is required", new[] { nameof(Name) });

            if (Geschicklichkeit < 0 || Staerke < 0 || Ausdauer < 0)
                yield return new ValidationResult("Attributes cannot be negative",
                    new[] { nameof(Geschicklichkeit), nameof(Staerke), nameof(Ausdauer) });
            if (!Enum.IsDefined(typeof(ItemCategory), Category))
                yield return new ValidationResult("Invalid category value.", new[] { nameof(Category) });

        }
    }
    public class UpdateItemHandler : IRequestHandler<UpdateItemCommand, ItemDto>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public UpdateItemHandler(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<ItemDto> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var existingItem = await _itemRepository.GetByIdAsync(request.Id);
            if (existingItem == null)
            {
                return null!;
            }

            existingItem.Name = request.Name;
            existingItem.Geschicklichkeit = request.Geschicklichkeit;
            existingItem.Staerke = request.Staerke;
            existingItem.Ausdauer = request.Ausdauer;
            existingItem.Category = request.Category;

            var updatedItem = await _itemRepository.UpdateAsync(existingItem);
            if (updatedItem == null)
            {
                return null!;
            }

            return _mapper.Map<ItemDto>(updatedItem);
        }
    }
}
