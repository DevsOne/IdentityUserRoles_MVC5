namespace IdentityUserRoles_MVC5.Migrations
{
    using Controllers;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.ApplicationDbContext context)
        {
            HomeController ctrl = new HomeController();
            //ctrl.UseContext();
            ctrl.UseRoleManager();
        }
    }
}
