using Microsoft.AspNetCore.Authorization;

namespace RealEstateMillion.API.Attributes
{
    public class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
