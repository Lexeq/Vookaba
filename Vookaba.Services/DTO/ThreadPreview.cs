namespace Vookaba.Services.DTO
{
    public class ThreadPreviewDto : ThreadDto
    {
        public int TotalPostsCount { get; set; }

        public int PostsWithImageCount { get; set; }
    }
}
