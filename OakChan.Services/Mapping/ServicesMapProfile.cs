using AutoMapper;
using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using OakChan.Services.Internal;
using System.Linq;

namespace OakChan.Services.Mapping
{
    public class ServicesMapProfile : Profile
    {
        public ServicesMapProfile()
        {
            CreateMap<Anonymous, UserDto>();

            CreateMap<Post, PostDto>()
                .ForMember(dto => dto.AuthorId, opt => opt.MapFrom(post => post.UserId))
                .ForMember(dto => dto.AuthorName, opt => opt.MapFrom(post => post.Name))
                .ForMember(dto => dto.Created, opt => opt.MapFrom(post => post.CreationTime))
                .ForMember(dto => dto.ThreadId, opt => opt.MapFrom(post => post.ThreadId))
                .ForMember(dto => dto.Message, opt => opt.MapFrom(post => post.Message))
                .ForMember(dto => dto.Image, opt => opt.MapFrom(post => post.Image))
                .ForMember(dto => dto.PostId, opt => opt.MapFrom(post => post.Id));

            CreateMap<Thread, ThreadDto>()
                .ForMember(dto => dto.ThreadId, opt => opt.MapFrom(thread => thread.Id))
                .ForMember(dto => dto.OpPost, opt => opt.MapFrom(thread => thread.Posts.First()))
                .ForMember(dto => dto.Replies, opt => opt.MapFrom(thread => thread.Posts.Skip(1)))
                .ForMember(dto => dto.BoardId, opt => opt.MapFrom(thread => thread.BoardId))
                .ForMember(dto => dto.Subject, opt => opt.MapFrom(thread => thread.Subject))
                .ReverseMap()
                .ForMember(thread => thread.Posts, opt => opt.Ignore());

            CreateMap<Thread, ThreadBoardAggregationDto>()
                .ForMember(dto => dto.Thread, opt => opt.MapFrom(thread => thread))
                .ForMember(dto => dto.Board, opt => opt.MapFrom(thread => thread.Board));

            CreateMap<ThreadPreviewQueryResult, ThreadPreviewDto>()
                .ForMember(dto => dto.BoardId, opt => opt.MapFrom(q => q.BoardId))
                .ForMember(dto => dto.OpPost, opt => opt.MapFrom(q => q.OpPost))
                .ForMember(dto => dto.PostsWithImageCount, opt => opt.MapFrom(q => q.ImagesCount))
                .ForMember(dto => dto.Replies, opt => opt.MapFrom(q => q.RecentPosts))
                .ForMember(dto => dto.Subject, opt => opt.MapFrom(q => q.Subject))
                .ForMember(dto => dto.ThreadId, opt => opt.MapFrom(q => q.ThreadId))
                .ForMember(dto => dto.TotalPostsCount, opt => opt.MapFrom(q => q.PostsCount));

            CreateMap<Board, BoardInfoDto>()
                .ForMember(dto => dto.Name, opt => opt.MapFrom(b => b.Name))
                .ForMember(dto => dto.Key, opt => opt.MapFrom(b => b.Key))
                .ForMember(dto => dto.ThreadsCount, opt => opt.MapFrom(b => b.Threads.Count()));

            CreateMap<Image, ImageDto>()
                .ForMember(dto => dto.ImageId, opt => opt.MapFrom(img => img.Id));
        }
    }
}
