using Microsoft.AspNetCore.Identity;  
using Microsoft.AspNetCore.Mvc;      
using Microsoft.EntityFrameworkCore;
using tao_project.Models.ViewModels;
using System.Security.Claims;
using tao_project.Models.Process;


namespace tao_project.Controllers{
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    //action CreateRole
    public IActionResult Create()
{
    return View();
}

[HttpPost]
public async Task<IActionResult> Create(string roleName)
{
    if (!string.IsNullOrEmpty(roleName))
    {
        var role = new IdentityRole(roleName.Trim());
        var result = await _roleManager.CreateAsync(role);
        
        if (!result.Succeeded)
        {
            // Xử lý khi tạo role thất bại (ví dụ: log lỗi)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
    }
    return RedirectToAction("Index");
}

//action edit role
public async Task<IActionResult> Edit(string id)
{
    var role = await _roleManager.FindByIdAsync(id);
    if (role == null)
    {
        return NotFound();
    }
    return View(role);
}

[HttpPost]
public async Task<IActionResult> Edit(string id, string newName)
{
    var role = await _roleManager.FindByIdAsync(id);
    if (role == null)
    {
        return NotFound();
    }
    
    role.Name = newName;
    var result = await _roleManager.UpdateAsync(role);
    
    if (!result.Succeeded)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(role);
    }
    
    return RedirectToAction("Index");
}
// action delete role
[HttpPost]
public async Task<IActionResult> Delete(string id)
{
    var role = await _roleManager.FindByIdAsync(id);
    if (role == null)
    {
        return NotFound();
    }
    
    var result = await _roleManager.DeleteAsync(role);
    
    if (!result.Succeeded)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(role);
    }
    
    return RedirectToAction("Index");   
}

    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }


    //action AssignClaim
    public async Task<IActionResult> AssignClaim(string id)
{
    var role = await _roleManager.FindByIdAsync(id);
    if (role == null)
    {
        return BadRequest();
    }

    var allPermissions = Enum.GetValues(typeof(SystemPermissions))
                            .Cast<SystemPermissions>()
                            .Select(p => p.ToString())
                            .ToList();

    var roleClaims = await _roleManager.GetClaimsAsync(role);
    if (roleClaims == null)
    {
        roleClaims = new List<Claim>();
    }

    var model = new RoleClaimVM
    {
        RoleId = role.Id,
        RoleName = role.Name,
        Claims = allPermissions.Select(p => new RoleClaim
        {
            Type = "Permission",
            Value = p,
            Selected = roleClaims.Any(c => c.Type == "Permission" && c.Value == p)
        }).ToList()
    };

    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AssignClaim(RoleClaimVM model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    var role = await _roleManager.FindByIdAsync(model.RoleId);
    if (role == null)
    {
        return BadRequest();
    }

    var claims = await _roleManager.GetClaimsAsync(role);
    if (claims == null)
    {
        claims = new List<Claim>();
    }

    // Xóa tất cả claims cũ có Type là "Permission"
    foreach (var claim in claims.Where(c => c.Type == "Permission"))
    {
        await _roleManager.RemoveClaimAsync(role, claim);
    }

    // Thêm claims mới được chọn
    foreach (var claim in model.Claims.Where(c => c.Selected))
    {
        await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
    }

    return RedirectToAction(nameof(Index));
}
}
}