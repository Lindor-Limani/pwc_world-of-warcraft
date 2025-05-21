using AutoMapper;
using pwc.Application.CQRS.Commands.Items;
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
            CreateMap<Charakter, CharakterDto>();
                //.ForMember(dest => dest.EquippedItems, opt => opt.MapFrom(src => src.CharakterItems.Select(ci => ci.Item)));

            /*CreateMap<EquipItemDTO, CharakterItem>()
                .ForMember(dest => dest.CharakterId, opt => opt.MapFrom(src => src.CharacterId))
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ItemId));*/

            CreateMap<Item, ItemDto>();
            CreateMap<CreateItemCommand, Item>();


            CreateMap<Monster, MonsterDto>();
        }
    }
}
