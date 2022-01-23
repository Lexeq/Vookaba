using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IHtmlFormatter
    {
        Task<string> FormatAsync(string message);
    }
}
