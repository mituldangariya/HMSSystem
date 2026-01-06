using System.ComponentModel.DataAnnotations;

namespace HMSSystem.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }

}
