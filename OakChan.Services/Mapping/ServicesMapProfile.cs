using AutoMapper;
using OakChan.DAL.Entities;
using OakChan.Services.DTO;

namespace OakChan.Services.Mapping
{
    public class ServicesMapProfile : Profile
    {
        public ServicesMapProfile()
        {
            CreateMap<Anonymous, UserDto>();

            CreateMap<Thread, ThreadDto>()
                .ForMember(dto => dto.ThreadId, opt => opt.MapFrom(thread => thread.Id))
                .ForMember(dto => dto.Posts, opt => opt.MapFrom(thread => thread.Posts));

            CreateMap<Post, PostDto>()
                .ForMember(dto => dto.AuthorId, opt => opt.MapFrom(post => post.UserId))
                .ForMember(dto => dto.AuthorName, opt => opt.MapFrom(post => post.Name))
                .ForMember(dto => dto.Created, opt => opt.MapFrom(post => post.CreationTime))
                .ForMember(dto => dto.ThreadId, opt => opt.MapFrom(post => post.ThreadId))
                .ForMember(dto => dto.PostId, opt => opt.MapFrom(post => post.Id));

            CreateMap<Image, ImageDto>()
                .ForMember(dto => dto.ImageId, opt => opt.MapFrom(img => img.Id));
        }
    }
}
