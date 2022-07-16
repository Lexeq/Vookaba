using AutoMapper;
using Vookaba.DAL.Entities;

namespace Vookaba.Services.Mapping
{
    public class ServicesMapProfile : Profile
    {
        public ServicesMapProfile()
        {
            CreateMap<Post, PostDto>()
                .ForMember(dto => dto.PostId, opt => opt.MapFrom(post => post.Id))
                .ForMember(dto => dto.AuthorId, opt => opt.MapFrom(post => post.AuthorToken))
                .ForMember(dto => dto.AuthorIP, opt => opt.MapFrom(post => post.IP))
                .ForMember(dto => dto.AuthorName, opt => opt.MapFrom(post => post.Name))
                .ForMember(dto => dto.Created, opt => opt.MapFrom(post => post.Created))
                .ForMember(dto => dto.ThreadId, opt => opt.MapFrom(post => post.ThreadId))
                .ForMember(dto => dto.Message, opt => opt.MapFrom(post => post.HtmlEncodedMessage))
                .ForMember(dto => dto.Image, opt => opt.MapFrom(post => post.Attachments.FirstOrDefault() as Image))
                .ForMember(dto => dto.PostNumber, opt => opt.MapFrom(post => post.Number))
                .ForMember(dto => dto.IsSaged, opt => opt.MapFrom(post => post.IsSaged))
                .ForMember(dto => dto.IsOpening, opt => opt.MapFrom(post => post.IsOP));

            CreateMap<Thread, ThreadDto>()
                .ForMember(dto => dto.ThreadId, opt => opt.MapFrom(thread => thread.Id))
                .ForMember(dto => dto.Posts, opt => opt.NullSubstitute(new List<Post>()))
                .ForMember(dto => dto.Posts, opt => opt.MapFrom(thread => thread.Posts))
                .ForMember(dto => dto.BoardKey, opt => opt.MapFrom(thread => thread.BoardKey))
                .ForMember(dto => dto.Subject, opt => opt.MapFrom(thread => thread.Subject))
                .ReverseMap()
                .ForMember(thread => thread.Posts, opt => opt.Ignore());

            CreateMap<Thread, ThreadPreviewDto>()
                .IncludeBase<Thread, ThreadDto>()
                .ForMember(dto => dto.TotalPostsCount, opt => opt.MapFrom(thread => thread.PostsCount))
                .ForMember(dto => dto.PostsWithImageCount, opt => opt.MapFrom(thread => thread.PostsWithAttachmentnsCount));

            CreateMap<Image, ImageDto>()
                .ForMember(dto => dto.ImageId, opt => opt.MapFrom(img => img.Id))
                .ForMember(dto => dto.Size, opt => opt.MapFrom(img => img.FileSize));

            CreateMap<PostCreationDto, Post>()
                .ForMember(p => p.AuthorToken, opt => opt.Ignore())
                .ForMember(p => p.IP, opt => opt.Ignore())
                .ForMember(p => p.UserAgent, opt => opt.Ignore())
                .ForMember(p => p.Created, opt => opt.Ignore())
                .ForMember(p => p.PlainMessageText, opt => opt.MapFrom(dto => dto.Message))
                .ForMember(p => p.HtmlEncodedMessage, opt => opt.MapFrom(dto => dto.EncodedMessage))
                .ForMember(p => p.Name, opt => opt.MapFrom(dto => dto.AuthorName))
                .ForMember(p => p.Id, opt => opt.Ignore())
                .ForMember(p => p.ThreadId, opt => opt.Ignore())
                .ForMember(p => p.Attachments, opt => opt.Ignore())
                .ForMember(p => p.Number, opt => opt.Ignore())
                .ForMember(p => p.IsSaged, opt => opt.MapFrom(dto => dto.IsSaged))
                .ForMember(p => p.Thread, opt => opt.Ignore())
                .ForMember(p => p.IsOP, opt => opt.Ignore());

            CreateMap<BoardDto, Board>()
                .ForMember(b => b.Threads, opt => opt.Ignore())
                .ForMember(b => b.Key, opt => opt.MapFrom(dto => dto.Key.ToLowerInvariant()))
                .ReverseMap();

            CreateMap<ModAction, ModLogDto>();

            CreateMap<Thread, ThreadInfoDto>()
                .ForMember(dto => dto.ThreadId, opt => opt.MapFrom(t => t.Id))
                .ForMember(dto => dto.Author, opt => opt.MapFrom(t => t.Posts.First(x => x.IsOP).AuthorToken));

            CreateMap<BanCreationDto, Ban>()
                .ForMember(b => b.Id, opt => opt.Ignore())
                .ForMember(b => b.Created, opt => opt.Ignore())
                .ForMember(b => b.UserId, opt => opt.Ignore())
                .ForMember(b => b.Creator, opt => opt.Ignore())
                .ForMember(b => b.BannedAuthor, opt => opt.Ignore())
                .ForMember(b => b.Board, opt => opt.Ignore())
                .ForMember(b => b.BoardKey, opt => opt.MapFrom(x => x.Board))
                .ForMember(b => b.IsCanceled, opt => opt.Ignore());

            CreateMap<Ban, BanInfoDto>();

            CreateMap<Ban, BanDto>();
        }
    }
}