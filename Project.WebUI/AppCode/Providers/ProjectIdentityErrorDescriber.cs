using Microsoft.AspNetCore.Identity;

namespace Project.AppCode.Providers
{
    public class ProjectIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateRoleName(string role)
        {
            //return base.DuplicateRoleName(role);
            return new IdentityError
            {
                Code = nameof(DuplicateRoleName),
                Description = $"{role} role is already available"
            };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(InvalidRoleName),
                Description = "Role name can not be empty"
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            //return base.DuplicateEmail(email);

            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"'{email}' is already available"
            };
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyInRole),
                Description = $"User in {role}-role"
            };
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserNotInRole),
                Description = $"User not in {role}-role"
            };
        }
    }
}
