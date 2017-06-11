using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	abstract class Projectile : PhysicalObject
	{
		public Projectile(Vector2 spawnLocation, Texture2D projectileSprite) : base(spawnLocation, projectileSprite){}

		public override void ReactToCollisionEntry(PhysicalObject other)
		{
			AudioManager.Instance.PlayAudioClip(AudioManager.AUDIOCLIPS.Explosion);
			//Play explosion graphic
			Destroy();
		}		
	}

	class Grenade : Projectile
	{
		public Grenade(Vector2 spawnLocation, Texture2D grenadeSprite) : base(spawnLocation, grenadeSprite)
		{
			velocity.Y = 500;
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(Missile));
			collidableTypes.Add(typeof(Plane));
		}
	}

	class Missile : Projectile
	{
		public Missile(Vector2 spawnLocation, Texture2D missileSprite) : base(spawnLocation, missileSprite)
		{
			velocity.Y = -500;
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(Grenade));
			collidableTypes.Add(typeof(Plane));
			collidableTypes.Add(typeof(PlayerCharacter));
		}
	}

	class Plane : Projectile 
	{
		public Plane(Vector2 spawnLocation, Texture2D planeSprite, bool movingLeft) : base(spawnLocation, planeSprite)
		{
			velocity.X = 200;
			if (movingLeft)
			{
				velocity.X *= -1;
			}
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(Grenade));
			collidableTypes.Add(typeof(Missile));
			collidableTypes.Add(typeof(PlayerCharacter));
		}
	}
}
