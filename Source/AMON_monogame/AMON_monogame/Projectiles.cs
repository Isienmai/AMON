using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	/// <summary>
	/// This class represents the basic projectile, defining standard reactions to collisions for all projectiles
	/// </summary>
	abstract class Projectile : PhysicalObject
	{
		public Projectile(Vector2 spawnLocation, Texture2D projectileSprite) : base(spawnLocation, projectileSprite){}

		public override void ReactToCollisionEntry(PhysicalObject other)
		{
			AudioManager.Instance.PlayAudioClip(AudioManager.AUDIOCLIPS.EXPLOSION);
			GraphicsManager.Instance.AddAnimation(new ExplosionAnimation(this.GetCentre()));
			Destroy();
		}		
	}

	/// <summary>
	/// The following THREE classes are specific projectiles, defining different velocities, images, and collidableTypes.
	/// The Plane projectile also contains a special collision response when colliding with a missile.
	/// </summary>
	class Grenade : Projectile
	{
		public Grenade(Vector2 spawnLocation) : base(spawnLocation, GraphicsManager.Instance.grenadeTexture)
		{
			velocity.Y = 300;
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(Missile));
			collidableTypes.Add(typeof(Plane));
		}
	}

	class Missile : Projectile
	{
		public Missile(Vector2 spawnLocation) : base(spawnLocation, GraphicsManager.Instance.rocketTexture)
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
		public Plane(Vector2 spawnLocation, bool movingLeft) : base(spawnLocation, GraphicsManager.Instance.PlaneSprite(movingLeft))
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

		public override void ReactToCollisionEntry(PhysicalObject other)
		{
			if (other is Missile)
			{
				GameWorld.Instance.AddObject(new Powerup(this.GetCentre()));
				AudioManager.Instance.PlayAudioClip(AudioManager.AUDIOCLIPS.BRILLIANT);
			}

			base.ReactToCollisionEntry(other);
		}
	}
}
