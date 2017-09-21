namespace Backend.Migrations.SeedData
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_User : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ShouldChangePasswordOnNextLogin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ShouldChangePasswordOnNextLogin");
        }
    }
}
