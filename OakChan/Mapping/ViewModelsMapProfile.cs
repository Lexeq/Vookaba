using AutoMapper;
using OakChan.Services.DTO;
using OakChan.ViewModels;
using System.Diagnostics;

namespace OakChan.Mapping
{
    public class ViewModelsMapProfile : Profile
    {
        public ViewModelsMapProfile()
        {
            CreateMap<PostDto, PostViewModel>()
                .ForMember(vm => vm.AuthorId, opt => opt.MapFrom(dto => dto.AuthorId))
                .ForMember(vm => vm.Number, opt => opt.MapFrom(dto => dto.PostId))
                .ForMember(vm => vm.Date, opt => opt.MapFrom(dto => dto.Created))
                .ForMember(vm => vm.ThreadId, opt => opt.MapFrom(dto => dto.ThreadId))
                .ForMember(vm => vm.AuthorName, opt => opt.MapFrom(dto => dto.AuthorName))
                .ForMember(vm => vm.Message, opt => opt.MapFrom(dto => dto.Message))
                .ForMember(vm => vm.Image, opt => opt.MapFrom(dto => dto.Image));

            CreateMap<PostDto, OpPostViewModel>()
                .IncludeBase<PostDto, PostViewModel>()
                .ForMember(vm => vm.Subject, opt => opt.Ignore()); ;

            CreateMap<PostFormViewModel, PostCreationDto>()
                .ForMember(dto => dto.Attachment, opt => opt.MapFrom(vm => vm.Image))
                .ForMember(dto => dto.AuthorId, opt => opt.MapFrom(
                    (_, __, ___, context) => context.Items[StringConstants.UserId]))
                .ForMember(dto => dto.AuthorName, opt => opt.MapFrom(vm => vm.Name))
                .ForMember(dto => dto.Message, opt => opt.MapFrom(vm => vm.Text));

            CreateMap<ThreadDto, ThreadViewModel>()
                .ForMember(vm => vm.BoardId, opt => opt.MapFrom(dto => dto.BoardId))
                .ForMember(vm => vm.ThreadId, opt => opt.MapFrom(dto => dto.ThreadId))
                .ForMember(vm => vm.Replies, opt => opt.MapFrom(dto => dto.Replies))
                .ForMember(vm => vm.OpPost, opt => opt.MapFrom(dto => dto.OpPost))
                .AfterMap((dto, vm) => vm.OpPost.Subject = dto.Subject);

            CreateMap<ThreadPreviewDto, ThreadPreviewViewModel>()
                .ForMember(vm => vm.PostsCount, opt => opt.MapFrom(dto => dto.TotalPostsCount))
                .ForMember(vm => vm.ThreadId, opt => opt.MapFrom(dto => dto.ThreadId))
                .ForMember(vm => vm.OpPost, opt => opt.MapFrom(dto => dto.OpPost))
                .ForMember(vm => vm.PostsWithImageCount, opt => opt.MapFrom(dto => dto.PostsWithImageCount))
                .ForMember(vm => vm.RecentPosts, opt => opt.MapFrom(dto => dto.Replies))
                .ForMember(vm => vm.Subject, opt => opt.MapFrom(dto => dto.Subject))
                .AfterMap((dto, vm) => vm.OpPost.Subject = vm.Subject);

            CreateMap<ThreadFormViewModel, PostCreationDto>()
                .ForMember(dto => dto.Attachment, opt => opt.MapFrom(vm => vm.Image))
                .ForMember(dto => dto.AuthorId, opt => opt.MapFrom(
                    (_, __, ___, context) => context.Items[StringConstants.UserId]))
                .ForMember(dto => dto.AuthorName, opt => opt.MapFrom(vm => vm.Name))
                .ForMember(dto => dto.Message, opt => opt.MapFrom(vm => vm.Text));

            CreateMap<ThreadFormViewModel, ThreadCreationDto>()
                .ForMember(dto => dto.Subject, opt => opt.MapFrom(vm => vm.Subject))
                .ForMember(dto => dto.OpPost, opt => opt.MapFrom(vm => vm));

            CreateMap<BoardPageDto, BoardPageViewModel>()
                .ForMember(vm => vm.Key, opt => opt.MapFrom(dto => dto.BoardId))
                .ForMember(vm => vm.Name, opt => opt.MapFrom(
                    (_, __, ___, context) => context.Items[StringConstants.BoardName]))
                .ForMember(vm => vm.PageNumber, opt => opt.MapFrom(dto => dto.PageNumber))
                .ForMember(vm => vm.Threads, opt => opt.MapFrom(dto => dto.Threads))
                .ForMember(vm => vm.TotalPages, opt => opt.MapFrom(
                    (_, __, ___, context) => context.Items[StringConstants.PagesCount]));

            CreateMap<ImageDto, ImageViewModel>();
        }
    }
}
