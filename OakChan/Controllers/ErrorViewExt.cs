using Microsoft.AspNetCore.Mvc;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public static class ErrorViewExt
    {
        public static ViewResult ErrorView(this Controller controller, ErrorViewModel viewModel)
        {
            controller.ViewData.Model = viewModel;

            return new ViewResult
            {
                ViewName = "Error",
                ViewData = controller.ViewData,
                TempData = controller.TempData,
                StatusCode = viewModel.Code
            };
        }
    }
}
