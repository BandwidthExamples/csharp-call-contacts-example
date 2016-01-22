using CallApp.Models;
using Owin;
using System.Data.Entity;
using SQLite.CodeFirst;

namespace CallApp
{

    public class CallAppDbContext: DbContext
    {

        public DbSet<Number> Numbers { get; set; }

        public DbSet<Contact> Contacts { get; set; }


        public CallAppDbContext(): base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<CallAppDbContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }

    }

    public static class Data
    { 
        public static void Load(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new CallAppDbContext());
        }

    }
}