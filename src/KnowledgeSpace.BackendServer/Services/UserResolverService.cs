using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Services
{
    public class UserResolverService : IUserResolverService
    {
        private readonly IHttpContextAccessor _accessor;
        public UserResolverService(IHttpContextAccessor accessor)
        {
            this._accessor = accessor;
        }

        public string GetUserId()
        {
            var userId = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var username = accessor?.HttpContext?.User?.Identity?.Name;
            return userId;
        }
    }
}
