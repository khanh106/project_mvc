using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace tao_project.Models.Process;

public class PolicyByPhoneNumberRequirement : IAuthorizationRequirement
{
    // Lớp requirement không cần thêm logic gì đặc biệt
}

public class PolicyByPhoneNumberHandler : AuthorizationHandler<PolicyByPhoneNumberRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public PolicyByPhoneNumberHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PolicyByPhoneNumberRequirement requirement)
    {
        var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext;
        
        if (httpContext == null)
        {
            return;
        }

        var userManager = httpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.GetUserAsync(context.User);
        
        if (user != null && !string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            context.Succeed(requirement);
        }
    }
}