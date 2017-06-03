using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	class Projectile : PhysicalObject
	{
		public Projectile(Vector2 spawnLocation, Texture2D projectileSprite) : base(spawnLocation, projectileSprite){}
	}

	class Grenade : Projectile
	{
		public Grenade(Vector2 spawnLocation, Texture2D grenadeSprite) : base(spawnLocation, grenadeSprite)
		{
			velocity.Y = 500;
		}
	}

	class Missile : Projectile
	{
		public Missile(Vector2 spawnLocation, Texture2D grenadeSprite) : base(spawnLocation, grenadeSprite)
		{
			velocity.Y = -500;
		}
	}
}
