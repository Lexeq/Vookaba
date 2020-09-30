using OakChan.Models;
using OakChan.Models.DB.Entities;

namespace OakChan.ViewModels
{
    public class BoardViewModel
    {
        public BoardPreview Board { get; set; }

        public OpPostViewModel OpPost { get; set; }
    }
}
