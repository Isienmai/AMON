using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON_monogame
{
	//This class represents anything with a physical presence ingame
	class Object
	{
		private Rectangle objectCollider;
		private Point velocity;
		private Texture2D sprite;

		public Object() 
		{
			objectCollider = new Rectangle(0, 0, 0, 0);
			velocity = new Point(0, 0);
		}

		public void SetSprite(Texture2D newSprite)
		{
			sprite = newSprite;
		}

		public void SetVelocity(Point newVelocity)
		{
			velocity = newVelocity;
		}

		public Point GetVelocity()
		{
			return velocity;
		}

		public Rectangle GetCollider()
		{
			return objectCollider;
		}

		public void Tick(float deltaTime)
		{
			objectCollider.X += (int)((float)velocity.X * deltaTime);
			objectCollider.Y += (int)((float)velocity.Y * deltaTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(sprite, objectCollider, Color.White);
		}

		public bool Collided(Object other)
		{
			//Check if the two rectangles overlap horizontally
			int overallWidth = (other.objectCollider.X + other.objectCollider.Width) - this.objectCollider.X;
			int combinedWidth = other.objectCollider.Width + this.objectCollider.Width;

			if (overallWidth < 0) return false;
			if (overallWidth > combinedWidth) return false;

			//Check if they overlap vertically
			int overallHeight = (other.objectCollider.Y + other.objectCollider.Height) - objectCollider.Y;
			int combinedHeight = other.objectCollider.Height + this.objectCollider.Height;

			if (overallHeight < 0) return false;
			if (overallHeight > combinedHeight) return false;

			return true;
		}
	}
}
