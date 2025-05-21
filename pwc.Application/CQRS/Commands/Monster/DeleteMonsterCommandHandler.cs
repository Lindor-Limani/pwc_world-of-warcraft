using MediatR;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Commands.Monster
{
    public record class DeleteMonsterCommand(int id) : IRequest<bool>;
    
    public class DeleteMonsterCommandHandler : IRequestHandler<DeleteMonsterCommand, bool>
    {
        private readonly IMonsterRepository _monsterRepository;
        public DeleteMonsterCommandHandler(IMonsterRepository monsterRepository)
        {
            _monsterRepository = monsterRepository;
        }
        public async Task<bool> Handle(DeleteMonsterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _monsterRepository.DeleteAsync(request.id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deleting monster", ex);
            }
        }
    }
}
