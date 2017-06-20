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
	/// The cloud object
	/// </summary>
	class Cloud : PhysicalObject
	{
		public Cloud(Rectangle viewportLimits) : base(GraphicsManager.Instance.cloudTextures[new Random().Next(0,2)])
		{
			Random randGenerator = new Random();

			//Generate a random size
			int height = randGenerator.Next(100, 300);
			int width = randGenerator.Next(150, 500);

			dimensions.X = width;
			dimensions.Y = height;

			//Put the cloud in a random location
			position.X = randGenerator.Next(-100, viewportLimits.Width - width);
			position.Y = randGenerator.Next(viewportLimits.Height, viewportLimits.Height + 90);

			velocity.Y = -300;			

			//Make the clouds partially transparent
			drawColour = drawColour * 0.8f;

			DrawLayer = 80;
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(PlayerCharacter));
		}
	}
}
