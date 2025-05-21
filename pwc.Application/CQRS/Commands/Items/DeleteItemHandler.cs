using MediatR;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Commands.Items
{
    public record class DeleteItemCommand(int Id) : IRequest<bool>;
    public class DeleteItemHandler : IRequestHandler<DeleteItemCommand, bool>
    {
        private readonly IItemRepository _itemRepository;
        public DeleteItemHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var existingItem = await _itemRepository.GetByIdAsync(request.Id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"No item found with id {request.Id}.");
            }

            try
            {
                return await _itemRepository.DeleteAsync(request.Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deleting item", ex);
            }
        }
    }

}
