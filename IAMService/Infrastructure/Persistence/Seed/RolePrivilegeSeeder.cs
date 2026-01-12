using Application.Enum;
using Application.Helper;
using Domain.Aggregate;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed
{
    public static class RolePrivilegeSeeder
    {
        public static async Task SeedAsync(IAMDBContext context)
        {
            // Flag
            if (await context.Roles.AnyAsync(r => r.Code == RoleKey.ADMIN))
            {
                ServiceLogger.Warning(
                    Level.Infrastructure, 
                    "Database already seeded, seeding action has been terminated");
                return;
            }

            // -----------------------------
            // 1️ Seed Roles (constructor used)
            // -----------------------------
            var rolesToSeed = new List<Role>
            {
                new Role(Guid.NewGuid(), "Super Administrator", RoleKey.SUPER_ADMIN, "Full access to all system features."),
                new Role(Guid.NewGuid(), "Administrator", RoleKey.ADMIN, "Full access to all system features but has some limitation compare to Super Administrator."),
                new Role(Guid.NewGuid(), "Citizen", RoleKey.CITIZEN, "Citizen are people who report waste/recycling."),
                new Role(Guid.NewGuid(), "Collector", RoleKey.COLLECTOR, "Collectors are responsible for receiving, assigning, and confirming waste and recycling collection reports."),
                new Role(Guid.NewGuid(), "Enterprise", RoleKey.ENTERPRISE, "Enterprises represent organizations that manage collection operations, personnel, and service areas."),
            };

            foreach (var role in rolesToSeed)
                await context.Roles.AddAsync(role);

            await context.SaveChangesAsync();

            // -----------------------------
            // 2️ Seed Privileges (keep comments)
            // -----------------------------
            var privilegeList = new (string Name, string Description)[]
            {
                ("ReadOnly", "Have right to view read only fields."),

                // IAM service
                // User Controller
                ("ViewUser", "Have right to view all user profiles."),
                ("CreateUser", "Have right to create new users."),
                ("ModifyUser", "Have right to modify user profiles."),
                ("DeleteUser", "Have right to delete users."),
                ("ChangePassword", "Have right to change owned account password"),
                // Role Controller
                ("ViewRole", "Have right to view all roles and privileges."),
                ("CreateRole", "Have right to create new custom roles."),
                ("UpdateRole", "Have right to update role privileges."),
                ("DeleteRole", "Have right to delete custom roles."),
                // Privilege Controller
                ("ViewPrivilege", "Have right to view all system privileges."),
                ("CreatePrivilege", "Have right to create new privileges."),
                ("UpdatePrivilege", "Have right to update existing privileges."),
                ("DeletePrivilege", "Have right to delete privileges."),

                // Citizen service
                // Citizen Controller
                ("ViewCitizenArea", "Have right to view citizen area."),
                ("ViewCitizenProfile", "Have right to view citizen profile."),
                ("ReportCollection", "Have right to report collection."),
                ("ReportComplaint", "Have right to report complaint."),
            };

            foreach (var (name, desc) in privilegeList)
            {
                if (!await context.Privileges.AnyAsync(p => p.Name == name))
                    await context.Privileges.AddAsync(new Privilege(Guid.NewGuid(), name, desc));
            }

            await context.SaveChangesAsync();

            // -----------------------------
            // 3️ Seed Role-Privilege mappings
            // -----------------------------
            var rolesFromDb = await context.Roles.ToListAsync();
            var privilegesFromDb = await context.Privileges.ToListAsync();

            var rolePrivilegesToInsert = new List<RolePrivilege>();

            void AddPrivilegesToRole(string roleCode, params string[] privilegeNames)
            {
                var role = rolesFromDb.First(r => r.Code == roleCode);
                foreach (var name in privilegeNames)
                {
                    var privilege = privilegesFromDb.First(p => p.Name == name);
                    rolePrivilegesToInsert.Add(new RolePrivilege(Guid.NewGuid(), role.RoleID, privilege.PrivilegeID, true));
                }
            }

            // SUPER_ADMIN → all privileges
            AddPrivilegesToRole(RoleKey.SUPER_ADMIN, privilegesFromDb.Select(p => p.Name).ToArray());

            // ADMIN → all privileges except role/privilege management (limitation)
            var adminPrivileges = privilegesFromDb
                .Where(p => !new[]
                {
                    "CreateRole", "UpdateRole", "DeleteRole",
                    "CreatePrivilege", "UpdatePrivilege", "DeletePrivilege"
                }.Contains(p.Name))
                .Select(p => p.Name)
                .ToArray();
            AddPrivilegesToRole(RoleKey.ADMIN, adminPrivileges);

            // CITIZEN
            AddPrivilegesToRole(RoleKey.CITIZEN,
                "ChangePassword",
                "ReadOnly",
                "ViewCitizenArea",
                "ViewCitizenProfile",
                "ReportCollection",
                "ReportComplaint"
            );

            await context.RolePrivileges.AddRangeAsync(rolePrivilegesToInsert);
            await context.SaveChangesAsync();

            // -----------------------------
            // 4️ Seed Default Super Admin User
            // -----------------------------
            var adminEmail = "longdong32120@gmail.com";
            if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
            {
                var adminRole = rolesFromDb.First(r => r.Code == RoleKey.SUPER_ADMIN);

                var id = Guid.NewGuid();
                var adminUser = new User(
                    userID: id,
                    email: adminEmail,
                    fullName: "Dong Xuan Bao Long",
                    dob: new DateTime(2005, 1, 28),
                    gender: "Male",
                    password: "28012005",
                    createdBy: id,
                    isActive: true
                );

                adminUser.AddRole(adminRole.RoleID);

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
