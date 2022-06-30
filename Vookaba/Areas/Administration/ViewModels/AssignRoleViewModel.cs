using Vookaba.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vookaba.Areas.Administration.ViewModels
{
    public class AssignRoleViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }

        public IList<CheckableItem<string>> Boards { get; set; }
    }
}
