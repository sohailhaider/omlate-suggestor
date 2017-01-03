namespace AprioriSelf.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class OmlateSuggestion : DbContext
    {
        // Your context has been configured to use a 'OmlateSuggestion' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'AprioriSelf.Models.OmlateSuggestion' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'OmlateSuggestion' 
        // connection string in the application configuration file.
        public OmlateSuggestion()
            : base("name=OmlateSuggestion")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.
        
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}