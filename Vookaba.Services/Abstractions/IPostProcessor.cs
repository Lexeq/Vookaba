namespace Vookaba.Services.Abstractions
{
    public interface IPostProcessor
    {
        public Task ProcessAsync(PostCreationDto post);
    }
}
