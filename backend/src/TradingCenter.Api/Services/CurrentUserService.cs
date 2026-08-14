using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TradingCenter.Domain.Interfaces;

namespace TradingCenter.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst(ClaimTypes.NameIdentifier) 
                     ?? user?.FindFirst("sub") 
                     ?? user?.FindFirst("nameidentifier");
            return claim != null && Guid.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
