using AutoMapper;
using Test.Domain.Entities;
using Test.Service.DTOs;

namespace Test.Service.Meppers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        //Products
        CreateMap<Product, ProductForResultDto>().ReverseMap();
        CreateMap<Product, ProductForUpdateDto>().ReverseMap();
        CreateMap<Product, ProductForCreationDto>().ReverseMap();
    }
}