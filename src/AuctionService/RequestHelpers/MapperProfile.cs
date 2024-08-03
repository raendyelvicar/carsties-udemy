using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.RequestHelpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Auction, AuctionDTO>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionDTO>();
            CreateMap<CreateAuctionDTO, Auction>().ForMember(d => d.Item, o => o.MapFrom(s => s));
            CreateMap<CreateAuctionDTO, Item>();
        }
    }
}
