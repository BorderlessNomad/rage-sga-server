using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class GameRegistry : DbEntity
	{
		public string Name { get; set; }

		public string DeveloperName { get; set; }

		public string Url { get; set; }
	}
}
