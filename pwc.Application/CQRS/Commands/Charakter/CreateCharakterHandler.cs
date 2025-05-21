using AutoMapper;
using MediatR;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.CQRS.Commands.Charakter
{
    public record class CreateCharakterCommand(
        string Name)   
        : IRequest<CharakterDto>, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Name is required", new[] { nameof(Name) });
        }
    }
    public class CreateCharakterHandler : IRequestHandler<CreateCharakterCommand, CharakterDto>
    {
        private readonly ICharakterRepository _charakterRepository;
        private readonly IMapper _mapper;
        public CreateCharakterHandler(
            ICharakterRepository charakterRepository,
            IMapper mapper)
        {
            _charakterRepository = charakterRepository;
            _mapper = mapper;
        }
        public async Task<CharakterDto> Handle(
            CreateCharakterCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var charakter = _mapper.Map<Domain.Model.Charakter>(request);
                await _charakterRepository.AddAsync(charakter);
                return _mapper.Map<CharakterDto>(charakter);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error creating charakter", ex);
            }
        }
    }
}
