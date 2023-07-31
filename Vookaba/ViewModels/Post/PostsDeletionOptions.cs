using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Vookaba.ViewModels.Ban;

namespace Vookaba.ViewModels.Post
{
    public class PostsDeletionOptions
    {
        public enum DeletingArea
        {
            Single = 1,
            Thread = 2,
            Board = 3,
            All = 4
        }

        [Required, JsonPropertyName("board")]
        public string Board { get; set; }

        [Range(1, int.MaxValue), JsonPropertyName("number")]
        public int PostNumber { get; set; }

        [Required, JsonPropertyName("reason")]
        public string Reason { get; set; }

        [EnumDataType(typeof(DeletingArea))]
        public DeletingArea Area { get; set; }

        public bool IPMode { get; set; }

        public PostBanViewModel Ban { get; set; }
    }
}



