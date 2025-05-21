using MediatR;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Commands.Charakter
{
    public record class DeleteCharakterCommand(int Id) : IRequest<bool>;
    public class DeleteCharakterCommandHandler : IRequestHandler<DeleteCharakterCommand, bool>
    {
        private readonly ICharakterRepository _charakterRepository;
        public DeleteCharakterCommandHandler(ICharakterRepository charakterRepository)
        {
            _charakterRepository = charakterRepository;
        }
        public async Task<bool> Handle(DeleteCharakterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _charakterRepository.DeleteAsync(request.Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deleting charakter, please check for references to Items you posses and first remove those before continueing.", ex);
            }
        }
    }
}
