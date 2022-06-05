using AutoMapper;
using Microsoft.AspNetCore.Http;
using OakChan.Areas.Administration.ViewModels;
using OakChan.Common;
using OakChan.Services.DTO;
using OakChan.ViewModels;
using System.Linq;

namespace OakChan.Mapping
{
    public class ViewModelsMapProfile : Profile
    {
        public ViewModelsMapProfile()
        {
            CreateMap<PostDto, PostViewModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(dto => dto.PostId))
                .ForMember(vm => vm.IsOpening, opt => opt.MapFrom(dto => dto.IsOpening))
                .ForMember(vm => vm.AuthorId, opt => opt.MapFrom(dto => dto.AuthorId))
                .ForMember(vm => vm.Number, opt => opt.MapFrom(dto => dto.PostNumber))
                .ForMember(vm => vm.Date, opt => opt.MapFrom(dto => dto.Created))
                .ForMember(vm => vm.ThreadId, opt => opt.MapFrom(dto => dto.ThreadId))
                .ForMember(vm => vm.AuthorName, opt => opt.MapFrom(dto => dto.AuthorName))
                .ForMember(vm => vm.Message, opt => opt.MapFrom(dto => dto.Message))
                .ForMember(vm => vm.Image, opt => opt.MapFrom(dto => dto.Image));

            CreateMap<PostFormViewModel, PostCreationDto>()
                .ForMember(dto => dto.Attachments, opt => opt.Ignore())
                .ForMember(dto => dto.AuthorName, opt => opt.MapFrom(vm => vm.Name))
                .ForMember(dto => dto.Message, opt => opt.MapFrom(vm => vm.Text))
                .ForMember(dto => dto.IsSaged, opt => opt.MapFrom(vm => vm.IsSaged))
                .ForMember(dto => dto.OpMark, opt => opt.Ignore())
                .ForMember(dto => dto.Tripcode, opt => opt.Ignore())
                .AfterMap((vm, dto, ctx) =>
                {
                    if (vm.Image != null)
                    {
                        var files = new FormFileCollection();
                        files.Add(vm.Image);
                        dto.Attachments = files;
                    }
                });

            CreateMap<ThreadDto, ThreadViewModel>()
                .ForMember(vm => vm.BoardKey, opt => opt.MapFrom(dto => dto.BoardKey))
                .ForMember(vm => vm.ThreadId, opt => opt.MapFrom(dto => dto.ThreadId))
                .ForMember(vm => vm.Subject, opt => opt.MapFrom(dto => dto.Subject))
                .ForMember(vm => vm.Replies, opt => opt.MapFrom(dto => dto.Posts.Skip(1)))
                .ForMember(vm => vm.OpPost, opt => opt.MapFrom(dto => dto.Posts.First()));

            CreateMap<ThreadPreviewDto, ThreadPreviewViewModel>()
                .ForMember(vm => vm.PostsCount, opt => opt.MapFrom(dto => dto.TotalPostsCount))
                .ForMember(vm => vm.ThreadId, opt => opt.MapFrom(dto => dto.ThreadId))
                .ForMember(vm => vm.OpPost, opt => opt.MapFrom(dto => dto.Posts.First()))
                .ForMember(vm => vm.PostsWithImageCount, opt => opt.MapFrom(dto => dto.PostsWithImageCount))
                .ForMember(vm => vm.RecentPosts, opt => opt.MapFrom(dto => dto.Posts.Skip(1)))
                .ForMember(vm => vm.Subject, opt => opt.MapFrom(dto => dto.Subject));

            CreateMap<ThreadFormViewModel, PostCreationDto>()
                .ForMember(dto => dto.Attachments, opt => opt.Ignore())
                .ForMember(dto => dto.AuthorName, opt => opt.MapFrom(vm => vm.Name))
                .ForMember(dto => dto.Message, opt => opt.MapFrom(vm => vm.Text))
                .ForMember(dto => dto.IsSaged, opt => opt.Ignore())
                .ForMember(dto => dto.OpMark, opt => opt.Ignore())
                .ForMember(dto => dto.Tripcode, opt => opt.Ignore())
                .AfterMap((vm, dto, ctx) =>
                {
                    if (vm.Image != null)
                    {
                        var files = new FormFileCollection();
                        files.Add(vm.Image);
                        dto.Attachments = files;
                    }
                });

            CreateMap<ThreadFormViewModel, ThreadCreationDto>()
                .ForMember(dto => dto.Subject, opt => opt.MapFrom(vm => vm.Subject))
                .ForMember(dto => dto.OpPost, opt => opt.MapFrom(vm => vm));

            CreateMap<ImageDto, ImageViewModel>();

            CreateMap<BoardPropertiesViewModel, BoardDto>()
                .ForMember(dto => dto.Key, opt => opt.MapFrom(vm => vm.BoardKey))
                .ReverseMap();

            CreateMap<BoardDto, BoardPropertiesViewModel>()
                .ForMember(vm => vm.BoardKey, opt => opt.MapFrom(dto => dto.Key))
                .ReverseMap();

            CreateMap<ApplicationOptions, AppConfiguratonViewModel>()
                .ForMember(vm => vm.RequireInvitation, opt => opt.MapFrom(x => x.RegistrationByInvitation))
                .ForMember(vm => vm.AnonymousCanPost, opt => opt.MapFrom(x => x.IsAnonymousPostingAllowed))
                .ReverseMap();
        }
    }
}
