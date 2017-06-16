﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	//This class represents anything with a physical presence ingame
	abstract class PhysicalObject
	{
		private Rectangle drawDest;
		protected Vector2 dimensions;
		protected Vector2 position;
		protected Vector2 velocity;
		protected Texture2D sprite;
		protected Color drawColour;

		//Keep track of the types that this object can collide with.
		//Implementation needs work
		//Empty list indicates this object can collide with everything
		protected List<Type> collidableTypes;

		public PhysicalObject(Texture2D objectSprite)
		{
			Instantiate(new Vector2(), objectSprite);
		}

		public PhysicalObject(Vector2 SpawnLocation, Texture2D objectSprite)
		{
			Instantiate(SpawnLocation, objectSprite);
		}

		private void Instantiate(Vector2 SpawnLocation, Texture2D objectSprite)
		{
			velocity = new Vector2();
			drawDest = new Rectangle();
			collidableTypes = new List<Type>();

			position = SpawnLocation;
			sprite = objectSprite;
			dimensions = new Vector2(sprite.Bounds.Width, sprite.Bounds.Height);
			drawColour = Color.White;

			SpecifyCollidableTypes();
		}

		protected abstract void SpecifyCollidableTypes();

		//Collision reactions to be implemented by base classes if needed.
		public virtual void ReactToCollision(PhysicalObject other) { }
		public virtual void ReactToCollisionEntry(PhysicalObject other) { }
		public virtual void ReactToCollisionExit(PhysicalObject other) { }

		public Vector2 GetCentre()
		{
			return new Vector2(position.X + dimensions.X/2.0f, position.Y + dimensions.Y/2.0f);
		}

		public virtual void Tick(float deltaTime)
		{
			position.X += velocity.X * deltaTime;
			position.Y += velocity.Y * deltaTime;
		}

		public bool Collided(PhysicalObject other)
		{
			bool ignoreOther = true;
			for(int i = 0; i < collidableTypes.Count; ++i)
			{
				if (collidableTypes[i].IsInstanceOfType(other)) ignoreOther = false;
			}

			//Return no collision if the other object cannot collide with this one
			if (ignoreOther && collidableTypes.Count != 0) return false;

			return Collided(other.position, other.dimensions);
		}

		public bool Collided(Rectangle otherBounds)
		{
			return Collided(new Vector2(otherBounds.Location.X, otherBounds.Location.Y), new Vector2(otherBounds.Width, otherBounds.Height));
		}

		private bool Collided(Vector2 otherPosition, Vector2 otherDimensions)
		{
			//Check if the two rectangles overlap horizontally
			float overallWidth = (otherPosition.X + otherDimensions.X) - this.position.X;
			float combinedWidth = otherDimensions.X + this.dimensions.X;

			if (overallWidth < 0) return false;
			if (overallWidth > combinedWidth) return false;

			//Check if they overlap vertically
			float overallHeight = (otherPosition.Y + otherDimensions.Y) - this.position.Y;
			float combinedHeight = otherDimensions.Y + this.dimensions.Y;

			if (overallHeight < 0) return false;
			if (overallHeight > combinedHeight) return false;

			return true;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			drawDest.X = (int)(position.X + 0.5f);
			drawDest.Y = (int)(position.Y + 0.5f);
			drawDest.Width = (int)(dimensions.X + 0.5f);
			drawDest.Height = (int)(dimensions.Y + 0.5f);

			spriteBatch.Draw(sprite, drawDest, drawColour);
		}

		public virtual void Destroy()
		{
			GameWorld.Instance.RemoveObject(this);
		}
	}
}
