using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models
{
    public class _UserInfo
    {
        [Key]
        public Nullable <int> UID { get; set; }

        [Required(ErrorMessage ="User Role is Required.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Security Question is Required")]
        public int? SecurityQuestionId { get; set; }

        [Required(ErrorMessage = "Answer for Security Question is Required")]
        public string SecurityQuestionAnswer { get; set; }

        [Required(ErrorMessage ="User Name is Required")]
        [StringLength(20, ErrorMessage ="The {0} must be least {2} characters long.",MinimumLength = 4)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }       

        [Required(ErrorMessage = "User SVCNO is Required")]
        [StringLength(20, ErrorMessage = "The {0} must be least {2} characters long.", MinimumLength = 4)]        
        public string ServiceNo { get; set; }
       
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Location is Required")]
        public string LocationId { get; set; }

        [Required(ErrorMessage = "Old Password is Required")]
        public string OldPassword { get; set; }
    }
}