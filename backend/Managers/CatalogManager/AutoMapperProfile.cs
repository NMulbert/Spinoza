using AutoMapper;


internal class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CatalogManager.Models.FrontendRequests.Test, CatalogManager.Models.AccessorSubmits.Test>()
            .ForMember(dest => dest.MessageType, opt => opt.Ignore());
        CreateMap<CatalogManager.Models.FrontendRequests.Tag, CatalogManager.Models.AccessorSubmits.Tag>();
        CreateMap<CatalogManager.Models.FrontendRequests.Question, CatalogManager.Models.AccessorSubmits.Question>();
        CreateMap<CatalogManager.Models.AccessorResults.TestChangeResult, CatalogManager.Models.FrontendResponses.TestChangeResult>();
        
        CreateMap<CatalogManager.Models.AccessorResults.Test, CatalogManager.Models.FrontendResponses.Test>();

        
    }
}