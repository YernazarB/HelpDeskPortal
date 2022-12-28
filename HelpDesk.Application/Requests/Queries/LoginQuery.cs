using HelpDesk.Application.Responses;
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Requests.Queries
{
    public class LoginQuery : BaseRequest<LoginViewModel>
    {
        [Required]
        [MinLength(1)]
        public string Username { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
