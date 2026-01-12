using Microsoft.AspNetCore.Authorization;

namespace SWD392.Authorization
{
    public class PrivilegeRequirement : IAuthorizationRequirement
    {
        public string Privilege { get; }

        public PrivilegeRequirement(string privilege)
        {
            Privilege = privilege;
        }
    }
}
