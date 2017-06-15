using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AMON
{
	class CollisionDetection : IEquatable<CollisionDetection>
	{
		public PhysicalObject object1;
		public PhysicalObject object2;

		public CollisionDetection(PhysicalObject a, PhysicalObject b)
		{
			object1 = a;
			object2 = b;
		}

		//given a valid CollisionDetection returns true if both register a collision between the same two objects
		public bool Equals(CollisionDetection other)
		{
			if (other == null) return false;

			bool result1 = (object1 == other.object1) && (object2 == other.object2);
			bool result2 = (object1 == other.object2) && (object2 == other.object1);

			return result1 || result2;
		}
	}

	//class for detecting collisions and calling the appropriate object collision responses
	class CollisionManager
	{
		//Store a list of all collisions (will need to treat a->b and b->a as the same...)
		private List<CollisionDetection> detectedCollisions;

		public CollisionManager()
		{
			detectedCollisions = new List<CollisionDetection>();
		}

		public void HandleCollisions(List<PhysicalObject> worldObjects, Rectangle worldBounds)
		{
			//Create a list of all collisions
			List<CollisionDetection> currentCollisions = new List<CollisionDetection>();
			for (int i = 0; i < worldObjects.Count; ++i)
			{
				for (int j = i; j < worldObjects.Count; ++j)
				{
					if(!worldObjects[j].Collided(worldBounds))
					{
						worldObjects[j].Destroy();
					}
					else if (worldObjects[i].Collided(worldObjects[j]))
					{
						CollisionDetection newCollision = new CollisionDetection(worldObjects[i], worldObjects[j]);
						currentCollisions.Add(newCollision);

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

			//handle collision exit
			for(int i = 0; i < detectedCollisions.Count; ++i)
			{
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
