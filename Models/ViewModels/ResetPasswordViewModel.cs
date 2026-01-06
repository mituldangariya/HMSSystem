using System.ComponentModel.DataAnnotations;

namespace HMSSystem.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }

        [Required, MinLength(6)]
        public string NewPassword { get; set; }

        [Required, Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }

}
