using AutoMapper;
using MediatR;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model.Enum;
using pwc.Domain.Model;
using pwc.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace pwc.Application.CQRS.Commands.Items
{
    public record class CreateItemCommand(
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
        }
    }

    public class CreateItemHandler : IRequestHandler<CreateItemCommand, ItemDto>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public CreateItemHandler(
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<ItemDto> Handle(
            CreateItemCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var item = _mapper.Map<Item>(request);
                await _itemRepository.AddAsync(item);
                return _mapper.Map<ItemDto>(item);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error creating item", ex);
            }
        }
    }
}