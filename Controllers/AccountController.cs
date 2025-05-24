using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tao_project.Models;
using tao_project.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using tao_project.Models.Process;

namespace tao_project.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Policy = "PolicyByPhoneNumber")]
        [Authorize(Policy=nameof(SystemPermissions.AccountView))]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<UserWithRoleVM>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserWithRoleVM
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }
            return View(usersWithRoles);
        }
        [Authorize(Policy=nameof(SystemPermissions.AssignRole))]
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles
                .Select(r => new RoleVM { Id = r.Id, Name = r.Name })
                .ToListAsync();

            var viewModel = new AssignRoleVM
            {
                UserId = userId,
                AllRoles = allRoles,
                SelectedRoles = userRoles.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(AssignRoleVM model)
        {
            if (model.SelectedRoles == null)
            {
                model.SelectedRoles = new List<string>();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                // Thêm role mới
                foreach (var role in model.SelectedRoles)
                {
                    if (!userRoles.Contains(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }

                // Xóa role không chọn
                foreach (var role in userRoles)
                {
                    if (!model.SelectedRoles.Contains(role))
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }


        //action Addclaim
    [Authorize(Policy = "PolicyByPhoneNumber")]
    public async Task<IActionResult> AddClaim(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var userClaims = await _userManager.GetClaimsAsync(user);
        var model = new UserClaimVM(userId, user.UserName, userClaims.ToList());
        return View(model);
    }

   [HttpPost]
   public async Task<IActionResult> AddClaim(string userId, string claimType, string claimValue)
   {
    var user = await _userManager.FindByIdAsync(userId);
    var result = await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
    
    if (result.Succeeded)
    {
        return RedirectToAction("AddClaim", new { userId });
    }
    
    return View();
    }

//action AddPhoneNumber
[Authorize]
[HttpGet]
public async Task<IActionResult> UpdatePhoneNumber()
{
   
    var user = await _userManager.GetUserAsync(User);
    
    if (user == null)
    {
        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
    }

    var model = new UpdatePhoneNumberViewModel
    {
        PhoneNumber = user.PhoneNumber
    };

    return View(model);
}

[HttpGet]
public IActionResult Profile()
{
    return View();
}



[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UpdatePhoneNumber(UpdatePhoneNumberViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    // Lấy user hiện tại
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return NotFound("Không tìm thấy người dùng.");
    }

    // Cập nhật SỐ ĐIỆN THOẠI vào thuộc tính PhoneNumber có sẵn
    user.PhoneNumber = model.PhoneNumber; // Gán giá trị vào thuộc tính chuẩn

    // Cách 1: Dùng SetPhoneNumberAsync (chuyên biệt cho phone number)
    var result = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

    // Hoặc Cách 2: Dùng UpdateAsync (cập nhật mọi thông tin user)
    // var result = await _userManager.UpdateAsync(user);

    if (!result.Succeeded)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    return RedirectToAction("Profile");
}

//action delete accout 
// [Authorize(Policy = nameof(SystemPermissions.AccountDelete))]
public async Task<IActionResult> Delete(string userId)
{
    if (string.IsNullOrEmpty(userId))
    {
        return NotFound();
    }

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        return NotFound();
    }

    return View(user);
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
// [Authorize(Policy = nameof(SystemPermissions.AccountDelete))]
public async Task<IActionResult> DeleteConfirmed(string userId)
{
    if (string.IsNullOrEmpty(userId))
    {
        return NotFound();
    }

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        return NotFound();
    }

    var result = await _userManager.DeleteAsync(user);
    if (result.Succeeded)
    {
        return RedirectToAction(nameof(Index));
    }

    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(string.Empty, error.Description);
    }

    return View(user);
}
}

  
}