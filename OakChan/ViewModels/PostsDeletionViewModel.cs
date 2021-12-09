using OakChan.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OakChan.ViewModels
{
    public class PostsDeletionViewModel
    {
        public enum DeletingArea
        {
            Single = 0,
            Thread = 1,
            Board = 2,
            All = 3
        }

        [JsonPropertyName("number")]
        public int PostNumber { get; set; }

        [Required]
        public string Reason { get; set; }

        public Mode Mode { get; set; }

        public DeletingArea Area { get; set; }
    }

}