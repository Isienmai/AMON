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
		public Powerup(Vector2 SpawnLocation, Texture2D projectileSprite) : base(projectileSprite)
		{
			position = SpawnLocation;
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(PlayerCharacter));
		}
	}
}
