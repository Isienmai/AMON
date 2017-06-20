using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AMON
{
	/// <summary>
	/// Store all information about a given collision.
	/// Also includes an equality operator to allow a->b and b->a to be seen as the same collision
	/// </summary>
	class CollisionDetection : IEquatable<CollisionDetection>
	{
		public PhysicalObject object1;
		public PhysicalObject object2;

		public CollisionDetection(PhysicalObject a, PhysicalObject b)
		{
			object1 = a;
			object2 = b;
		}

		/// <summary>
		/// Returns true if the provided CollisionDetection matches this CollisionDetection
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(CollisionDetection other)
		{
			if (other == null) return false;

			//Ignore the order of the colliding objects
			bool result1 = (object1 == other.object1) && (object2 == other.object2);
			bool result2 = (object1 == other.object2) && (object2 == other.object1);

			return result1 || result2;
		}
	}

	/// <summary>
	/// Handles collision detection and resolution, including keeping track of collision enter, stay, and exit events
	/// </summary>
	class CollisionManager
	{
		//Store a list of all collisions from the previous collision detection pass
		private List<CollisionDetection> detectedCollisions;

		public CollisionManager()
		{
			detectedCollisions = new List<CollisionDetection>();
		}

		public void HandleCollisions(List<PhysicalObject> worldObjects, Rectangle worldBounds)
		{
			//Create a list of all collisions
			List<CollisionDetection> currentCollisions = new List<CollisionDetection>();
			foreach (PhysicalObject obj1 in worldObjects.ToArray())
			{
				foreach (PhysicalObject obj2 in worldObjects.ToArray())
				{
					//Destroy objects that are outside the worldbounds
					if(!obj2.Collided(worldBounds))
					{
						obj2.Destroy();
					}
					else if (obj1.Collided(obj2))
					{
						//Store the new collision
						CollisionDetection newCollision = new CollisionDetection(obj1, obj2);
						currentCollisions.Add(newCollision);

						//handle collision stay
						newCollision.object1.ReactToCollision(newCollision.object2);
						newCollision.object2.ReactToCollision(newCollision.object1);

						//handle collision entry
						if(!detectedCollisions.Contains(newCollision))
						{
							newCollision.object1.ReactToCollisionEntry(newCollision.object2);
							newCollision.object2.ReactToCollisionEntry(newCollision.object1);
						}
					}
				}
			}

			//Check for any collisions from the last pass that were not present this pass
			for(int i = 0; i < detectedCollisions.Count; ++i)
			{
				//handle collision exit
				if (!currentCollisions.Contains(detectedCollisions[i]))
				{
					detectedCollisions[i].object1.ReactToCollisionExit(detectedCollisions[i].object2);
					detectedCollisions[i].object2.ReactToCollisionExit(detectedCollisions[i].object1);
				}
			}

			detectedCollisions = currentCollisions;
		}
	}
}
