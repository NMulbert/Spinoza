using AutoMapper;


internal class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CatalogManager.Models.FrontendRequests.Test, CatalogManager.Models.AccessorSubmits.Test>()
            .ForMember(dest => dest.MessageType, opt => opt.Ignore());

        CreateMap<CatalogManager.Models.AccessorResults.TestChangeResult, CatalogManager.Models.FrontendResponses.TestChangeResult>();

        CreateMap<CatalogManager.Models.AccessorResults.QuestionChangeResult, CatalogManager.Models.FrontendResponses.QuestionChangeResult>();

        CreateMap<CatalogManager.Models.AccessorResults.Test, CatalogManager.Models.FrontendResponses.Test>();

        CreateMap<CatalogManager.Models.FrontendRequests.MultipleChoiceQuestion, CatalogManager.Models.AccessorSubmits.MultipleChoiceQuestion>()
            .ForMember(dest => dest.MessageType, opt => opt.Ignore());

        CreateMap<CatalogManager.Models.FrontendRequests.OpenTextQuestion, CatalogManager.Models.AccessorSubmits.OpenTextQuestion>()
            .ForMember(dest => dest.MessageType, opt => opt.Ignore());

        CreateMap<CatalogManager.Models.AccessorResults.MultipleChoiceQuestion, CatalogManager.Models.FrontendResponses.MultipleChoiceQuestion>();

        CreateMap<CatalogManager.Models.AccessorResults.OpenTextQuestion, CatalogManager.Models.FrontendResponses.OpenTextQuestion>();

        CreateMap<CatalogManager.Models.AccessorResults.Content, CatalogManager.Models.FrontendResponses.Content>();

        CreateMap<CatalogManager.Models.AccessorResults.AnswerOption, CatalogManager.Models.FrontendResponses.AnswerOption>();

        CreateMap<CatalogManager.Models.FrontendRequests.Content, CatalogManager.Models.AccessorSubmits.Content>();

        CreateMap<CatalogManager.Models.FrontendRequests.AnswerOption, CatalogManager.Models.AccessorSubmits.AnswerOption>();
        
        CreateMap<CatalogManager.Models.AccessorResults.TagChangeResult, CatalogManager.Models.FrontendResponses.TagChangeResult>();

    }
}