using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	class Powerup : PhysicalObject
	{
		public Powerup(Vector2 SpawnLocation) : base(GraphicsManager.Instance.powerUpImage)
		{
			position = SpawnLocation;

			DrawLayer = 30;
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(PlayerCharacter));
		}

		public override void ReactToCollision(PhysicalObject other)
		{
			Destroy();
		}
	}
}
