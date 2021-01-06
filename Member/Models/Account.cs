using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Member.Models
{
    public class RegisterViewModel
    { 
        [DisplayName("帳號：")]
        [Required]
        [EmailAddress]
        public string UsEmail { get; set; }
        [Required]
        [DisplayName("密碼：")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "會員密碼的長度需再6~10個字元內！")]
        [DataType(DataType.Password)]
        [RegularExpression(@"[a-zA-Z]+[a-zA-Z0-9]*$", ErrorMessage = "密碼僅能有英文或數字，且開頭需為英文字母！")]
        public string UsPassword { get; set; }
        [Display(Name = "確認會員密碼：")]
        [DataType(DataType.Password)]
        [Required]
        [Compare("UsPassword", ErrorMessage = "兩次輸入的密碼必須相符！")]
        public string ConfirmPassword { get; set; }
        }
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "帳號：")]
        [EmailAddress]
        public string UsEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼：")]
        public string UsPassword { get; set; }

    }
}