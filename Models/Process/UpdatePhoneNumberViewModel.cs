using System.ComponentModel.DataAnnotations;

public class UpdatePhoneNumberViewModel
{
    [Required(ErrorMessage = "Please enter phone number")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }
}