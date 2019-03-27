using System.Collections.Generic;
using Root.AuthenticationModels;
using Root.Models;

namespace Root.Migrations
{
	using System.Data.Entity.Migrations;
	using System.Linq;

	public class Configuration : DbMigrationsConfiguration<Root.Data.SGTSVNDBContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			//Because we are developing product so lossingn data is not a big issue
			AutomaticMigrationDataLossAllowed = true;
		}

		protected override void Seed(Root.Data.SGTSVNDBContext context)
		{
			if (context.Clients.Count() > 0)
			{
				return;
			}

			context.Clients.AddRange(BuildClientsList());
			context.SaveChanges();
		}

		private static List<Client> BuildClientsList()
		{
			var clientsList = new List<Client> 
				{
                new Client
                { Id = "ngAuthApp", 
                    Secret= Helper.Helper.GetHash("abc@123"), 
                    Name="AngularJS front-end Application", 
                    ApplicationType =  ApplicationTypes.JavaScript, 
                    Active = true, 
                    RefreshTokenLifeTime = 1, 
					AllowedOrigin = "http://webdev.local"
                },
                new Client
                { Id = "consoleApp", 
                    Secret= Helper.Helper.GetHash("123@abc"), 
                    Name="Console Application", 
                    ApplicationType = ApplicationTypes.NativeConfidential, 
                    Active = true, 
                    RefreshTokenLifeTime = 1, 
                    AllowedOrigin = "*"
                }
            };
			return clientsList;
		}
	}
}
