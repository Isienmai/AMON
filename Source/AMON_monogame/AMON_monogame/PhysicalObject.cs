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
	/// Parent class representing any physical object
	/// </summary>
	abstract class PhysicalObject
	{
		//The location and size of the object on screen
		private Rectangle drawDest;
		//The image used to represent the object on screen
		protected Texture2D sprite;
		//The colour passed to spriteBatch draw
		protected Color drawColour;

		//Control if this object is drawn in front of or behind other objects. Lower draw layers are bahind higher layers.
		private int drawLayer;

		public int DrawLayer
		{
			get
			{
				return drawLayer;
			}
			protected set
			{
				//only draw layer values of 0->99 incl are valid
				if (value > 99) drawLayer = 99;
				else if (value < 0) drawLayer = 0;
				else drawLayer = value;
			}
		}

		//The width and height of the object
		protected Vector2 dimensions;
		//The location of the top left corner of the object
		protected Vector2 position;
		//The object's velocity
		protected Vector2 velocity;

		//A list identifying what other objects this one can collide with
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

			drawLayer = 10;

			SpecifyCollidableTypes();
		}

		//All subclasses need to specify what they can collide with
		protected abstract void SpecifyCollidableTypes();

		//Collision reactions to be implemented by base classes if needed.
		public virtual void ReactToCollision(PhysicalObject other) { }
		public virtual void ReactToCollisionEntry(PhysicalObject other) { }
		public virtual void ReactToCollisionExit(PhysicalObject other) { }

		//Get the coordinates of the object's centre
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
			//Check the other object can collide with this one
			bool ignoreOther = true;
			for(int i = 0; i < collidableTypes.Count; ++i)
			{
				if (collidableTypes[i].IsInstanceOfType(other)) ignoreOther = false;
			}
			
			if (ignoreOther) return false;

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
