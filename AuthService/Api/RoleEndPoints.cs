using AuthService.Service;

namespace AuthService.Api
{
    public static class RoleEndPoints
    {
        public static void MapRoleEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/roles").WithTags("Roles");

            group.MapGet("", async (IRoleService roleService) =>
            {
                var roles = await roleService.GetAllAsync();
                return Results.Ok(roles);
            });

            group.MapGet("/{id:int}", async (int id, IRoleService roleService) =>
            {
                var role = await roleService.GetByIdAsync(id);

                return role is null
                    ? Results.NotFound("Role not found.")
                    : Results.Ok(role);
            });

            group.MapPost("", async (string roleName, IRoleService roleService) =>
            {
                var result = await roleService.CreateAsync(roleName);

                return result == "Role already exists."
                    ? Results.BadRequest(result)
                    : Results.Ok(result);
            });

            group.MapPut("/{id:int}", async (int id, string roleName, IRoleService roleService) =>
            {
                var result = await roleService.UpdateAsync(id, roleName);

                return result switch
                {
                    "Role not found." => Results.NotFound(result),
                    "Role already exists." => Results.BadRequest(result),
                    _ => Results.Ok(result)
                };
            });

            group.MapDelete("/{id:int}", async (int id, IRoleService roleService) =>
            {
                var deleted = await roleService.DeleteAsync(id);

                return deleted
                    ? Results.Ok("Role deleted successfully.")
                    : Results.NotFound("Role not found.");
            });
        }
    }
}
