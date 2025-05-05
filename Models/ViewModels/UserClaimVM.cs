
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace tao_project.Models.ViewModels{
public class UserClaimVM
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<Claim> UserClaims { get; set; }

    public UserClaimVM(string userId, string userName, List<Claim> userClaims)
    {
        UserId = userId;
        UserName = userName;
        UserClaims = userClaims;
    }
}
}