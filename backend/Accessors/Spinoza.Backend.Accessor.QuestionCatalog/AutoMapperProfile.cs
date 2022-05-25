using AutoMapper;

namespace Spinoza.Backend.Accessor.QuestionCatalog;

internal class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Spinoza.Backend.Accessor.QuestionCatalog.Models.DB.MultipleChoiceQuestion, Spinoza.Backend.Accessor.QuestionCatalog.Models.Results.MultipleChoiceQuestion>();

        CreateMap<Spinoza.Backend.Accessor.QuestionCatalog.Models.DB.OpenTextQuestion, Spinoza.Backend.Accessor.QuestionCatalog.Models.Results.OpenTextQuestion>();

        CreateMap<Spinoza.Backend.Accessor.QuestionCatalog.Models.DB.Content, Spinoza.Backend.Accessor.QuestionCatalog.Models.Results.Content>();

        CreateMap<Spinoza.Backend.Accessor.QuestionCatalog.Models.DB.AnswerOption, Spinoza.Backend.Accessor.QuestionCatalog.Models.Results.AnswerOption>();

        CreateMap<Spinoza.Backend.Accessor.QuestionCatalog.Models.Requests.MultipleChoiceQuestion, Spinoza.Backend.Accessor.QuestionCatalog.Models.DB.MultipleChoiceQuestion>()
            .ForMember(dest => dest._etag, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionVersion, opt => opt.MapFrom(src => "1.0"))
            .ForMember(dest => dest.PreviousVersionId, opt => opt.MapFrom(src => "none"))
            .ForMember(dest => dest.CreationTimeUTC, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.LastUpdateCreationTimeUTC, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));

        CreateMap<Spinoza.Backend.Accessor.QuestionCatalog.Models.Requests.OpenTextQuestion, Spinoza.Backend.Accessor.QuestionCatalog.Models.DB.OpenTextQuestion>()
            .ForMember(dest => dest._etag, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionVersion, opt => opt.MapFrom(src => "1.0"))
            .ForMember(dest => dest.PreviousVersionId, opt => opt.MapFrom(src => "none"))
            .ForMember(dest => dest.CreationTimeUTC, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.LastUpdateCreationTimeUTC, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));

        CreateMap<Spinoza.Backend.Accessor.QuestionCatalog.Models.Requests.Content, Spinoza.Backend.Accessor.QuestionCatalog.Models.DB.Content>();

        CreateMap<Spinoza.Backend.Accessor.QuestionCatalog.Models.Requests.AnswerOption, Spinoza.Backend.Accessor.QuestionCatalog.Models.DB.AnswerOption>();
    }
}