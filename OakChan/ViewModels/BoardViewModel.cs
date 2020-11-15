using OakChan.Models;
using OakChan.Models.DB.Entities;

namespace OakChan.ViewModels
{
    public class BoardViewModel
    {
        public BoardPreview Board { get; set; }

        public OpPostFormViewModel OpPost { get; set; }
        
        public int PageNumber { get; set; }

        public int TotalPages { get; set; }
    }
}
