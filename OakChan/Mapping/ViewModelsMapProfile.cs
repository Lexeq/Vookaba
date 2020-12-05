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

            CreateMap<PostDto, PostViewModel>()
                .ForMember(vm => vm.AuthorId, opt => opt.MapFrom(dto => dto.AuthorId))
                .ForMember(vm => vm.Number, opt => opt.MapFrom(dto => dto.PostId))
                .ForMember(vm => vm.Date, opt => opt.MapFrom(dto => dto.Created))
                .ForMember(vm => vm.Board, opt => opt.Ignore())
                .ForMember(vm => vm.Thread, opt => opt.MapFrom(dto => dto.ThreadId))
                .ForMember(vm => vm.Image, opt => opt.MapFrom(dto => dto.Image));

            CreateMap<ImageDto, ImageViewModel>();
        }
    }
}
