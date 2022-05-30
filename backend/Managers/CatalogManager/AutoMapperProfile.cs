using AutoMapper;

namespace CatalogManager;

internal class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Models.FrontendRequests.Test, Models.AccessorSubmits.Test>()
            .ForMember(dest => dest.MessageType, opt => opt.Ignore());

        CreateMap<Models.AccessorResults.TestChangeResult, Models.FrontendResponses.TestChangeResult>();

        CreateMap<Models.AccessorResults.QuestionChangeResult, Models.FrontendResponses.QuestionChangeResult>();

        CreateMap<Models.AccessorResults.Test, Models.FrontendResponses.Test>();

        CreateMap<Models.FrontendRequests.MultipleChoiceQuestion, Models.AccessorSubmits.MultipleChoiceQuestion>()
            .ForMember(dest => dest.MessageType, opt => opt.Ignore());

        CreateMap<Models.FrontendRequests.OpenTextQuestion, Models.AccessorSubmits.OpenTextQuestion>()
            .ForMember(dest => dest.MessageType, opt => opt.Ignore());

        CreateMap<Models.AccessorResults.MultipleChoiceQuestion, Models.FrontendResponses.MultipleChoiceQuestion>();

        CreateMap<Models.AccessorResults.OpenTextQuestion, Models.FrontendResponses.OpenTextQuestion>();

        CreateMap<Models.AccessorResults.Content, Models.FrontendResponses.Content>();

        CreateMap<Models.AccessorResults.AnswerOption, Models.FrontendResponses.AnswerOption>();

        CreateMap<Models.FrontendRequests.Content, Models.AccessorSubmits.Content>();

        CreateMap<Models.FrontendRequests.AnswerOption, Models.AccessorSubmits.AnswerOption>();
        
        CreateMap<Models.AccessorResults.TagChangeResult, Models.FrontendResponses.TagChangeResult>();

        CreateMap<Models.AccessorResults.CommonQuestion, Models.AccessorSubmits.CommonQuestion>()
            .ForMember(dest=>dest.MessageType, opt => opt.Ignore());
    }
}