using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Data.Entity;

namespace SocialGamificationAsset.Models
{
	public class MySQLDbConfiguration : DbConfiguration
	{
		public MySQLDbConfiguration()
		{
			// Register ADO.NET provider
			var dataSet = (DataSet)ConfigurationManager.GetSection("system.data");
			dataSet.Tables[0].Rows.Clear();
			dataSet.Tables[0].Rows.Add(
				"MySQL Data Provider",
				".Net Framework Data Provider for MySQL",
				"MySql.Data.MySqlClient",
				typeof(MySqlClientFactory).AssemblyQualifiedName
			);

			// Register Entity Framework provider
			SetProviderServices("MySql.Data.MySqlClient", new MySqlProviderServices());
			SetDefaultConnectionFactory(new MySqlConnectionFactory());
		}
	}
}
