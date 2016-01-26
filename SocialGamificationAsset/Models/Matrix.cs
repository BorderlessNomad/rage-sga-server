using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	[ComplexType]
	public class Matrix
	{
		public float X { get; set; }

		public float Y { get; set; }
	}

	public class ConcernMatrix : Model
	{
		public Matrix Coordinates { get; set; }

		public ConcernCategories Category { get; set; }
	}

	public class RewardResourceMatrix : Model
	{
		public Matrix Coordinates { get; set; }

		public RewardResourceCategories Category { get; set; }
	}

	public enum ConcernCategories
	{
		Defeat,
		Collaborate,
		Withdraw,
		Accomodate,
		Compromise
	}

	public enum RewardResourceCategories
	{
		HyperCooperation,
		PDC, // TODO
		ContrariantCooperation
	}
}
