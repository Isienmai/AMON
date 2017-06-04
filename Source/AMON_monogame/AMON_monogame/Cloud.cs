using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	class Cloud : PhysicalObject
	{
		public Cloud(Rectangle viewportLimits, Texture2D objectSprite) : base(objectSprite)
		{
			Random randGenerator = new Random();

			//Generate a random size
			int height = randGenerator.Next(100, 300);
			int width = randGenerator.Next(150, 500);

			dimensions.X = width;
			dimensions.Y = height;

			//Put the cloud in a random location
			position.X = randGenerator.Next(0, 800 - width);
			position.Y = randGenerator.Next(480, 700);

			velocity.Y = -300;

			collidableTypes.Add(typeof(PlayerCharacter));
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(PlayerCharacter));
		}
	}
}
