using AutoMapper;
using OakChan.Services.DTO;
using OakChan.ViewModels;

namespace OakChan.Mapping
{
    public class ViewModelsMapProfile : Profile
    {
        public ViewModelsMapProfile()
        {
            CreateMap<ThreadDto, ThreadViewModel>()
                .ForMember(vm => vm.Post, opt => opt.Ignore())
                    .AfterMap((d, v) => v.Post = new PostFormViewModel());

            CreateMap<ThreadPreviewDto, ThreadPreviewViewModel>()
                .ForMember(vm => vm.PostsCount, opt => opt.MapFrom(dto => dto.TotalPostsCount));

            CreateMap<BoardPageDto, BoardPageViewModel>()
                .ForMember(vm => vm.Key, opt => opt.MapFrom(dto => dto.BoardId))
                .ForMember(vm => vm.Name, opt => opt.MapFrom(
                    (_, __, ___, context) => context.Items[StringConstants.BoardName]))
                .ForMember(vm => vm.OpPost, opt => opt.MapFrom(dto => new OpPostFormViewModel()))
                .ForMember(vm => vm.PageNumber, opt => opt.MapFrom(dto => dto.PageNumber))
                .ForMember(vm => vm.ThreadsOnPage, opt => opt.MapFrom(dto => dto.Threads))
                .ForMember(vm => vm.TotalPages, opt => opt.MapFrom(
                    (_, __, ___, context) => context.Items[StringConstants.PagesCount]));

            CreateMap<PostDto, PostViewModel>()
                .ForMember(vm => vm.AuthorId, opt => opt.MapFrom(dto => dto.AuthorId))
                .ForMember(vm => vm.Number, opt => opt.MapFrom(dto => dto.PostId))
                .ForMember(vm => vm.Date, opt => opt.MapFrom(dto => dto.Created))
                .ForMember(vm => vm.ThreadId, opt => opt.MapFrom(dto => dto.ThreadId))
                .ForMember(vm => vm.Image, opt => opt.MapFrom(dto => dto.Image));

            CreateMap<ImageDto, ImageViewModel>();
        }
    }
}
