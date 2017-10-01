namespace AutomatedTellerMachine.Migrations {
    using AutomatedTellerMachine.Models;
    using AutomatedTellerMachine.Services;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AutomatedTellerMachine.Models.ApplicationDbContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.ApplicationDbContext context) {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);


            if (!context.Users.Any(t => t.UserName == "admin@mvcatm.com")) {
                var user = new ApplicationUser { UserName = "admin@mvcatm.com", Email = "admin@mvcatm.com" };
                userManager.Create(user, "Ma!234");

                var service = new CheckingAccountService(context);
                service.CreateCheckingAccount("admin", "user", user.Id, 1000);

                context.Roles.AddOrUpdate(r => r.Name, new IdentityRole { Name = "Admin" });
                context.SaveChanges();

                userManager.AddToRole(user.Id, "Admin");

                context.Transactions.Add(new Transaction { Amount = 71, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = -21, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 100000, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 19.99m, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 64.40m, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 100, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = -300, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 211.71m, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 198, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 2, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 10, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = -1.10m, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 6100, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = 164.84m, CheckingAccountId = 1 });
                context.Transactions.Add(new Transaction { Amount = .01m, CheckingAccountId = 1 });
            }
        }
    }
}
