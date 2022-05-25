using AutoMapper;

namespace Spinoza.Backend.Accessor.TestCatalog;

internal class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Models.DB.Test, Spinoza.Backend.Accessor.TestCatalog.Models.Results.Test>();
        CreateMap<Models.Requests.Test, Models.DB.Test>()
            .ForMember(dest => dest._etag, opt => opt.Ignore())
            .ForMember(dest => dest.TestVersion, opt => opt.MapFrom(src => "1.0"))
            .ForMember(dest => dest.PreviousVersionId, opt => opt.MapFrom(src => "none"))
            .ForMember(dest => dest.CreationTimeUTC, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.LastUpdateCreationTimeUTC, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));
       
    }
}