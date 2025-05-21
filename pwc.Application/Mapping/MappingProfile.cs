using AutoMapper;
using pwc.Application.CQRS.Commands.Charakter;
using pwc.Application.CQRS.Commands.Items;
using pwc.Application.CQRS.Commands.Monster;
using pwc.Domain.DTOs;
using pwc.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Item, ItemDto>();
            CreateMap<CreateItemCommand, Item>();
            CreateMap<CreateCharakterCommand, Charakter>();
            CreateMap<EquipItemToCharakterCommand, CharakterItem>()
                .ForMember(dest => dest.CharakterId, opt => opt.MapFrom(src => src.CharakterId))
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ItemId));
            CreateMap<CreateMonsterCommand, Monster>();

            CreateMap<Charakter, CharakterDto>()
    .ForMember(dest => dest.EquippedItems, opt => opt.MapFrom(src => src.CharakterItems.Select(ci => ci.Item)));

            CreateMap<Monster, MonsterDto>();
        }
    }
}
