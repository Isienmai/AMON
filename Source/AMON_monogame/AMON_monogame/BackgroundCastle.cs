using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AMON
{
	/// <summary>
	/// Encapsulates the background castle that appears at the end of the game
	/// </summary>
	class BackgroundCastle : PhysicalObject
	{
		public BackgroundCastle(Rectangle viewportLimits) : base(new Vector2(0, viewportLimits.Height), GraphicsManager.Instance.castleImage)
		{
			velocity.Y = -100;

			//Draw the castle behind almost everything
			DrawLayer = 2;

			//Scale the image up to fix the screen
			float scale = (float)viewportLimits.Width / dimensions.X;
			dimensions.X *= scale;
			dimensions.Y *= scale;
		}

		protected override void SpecifyCollidableTypes()
		{
			//Don't collide with anything
		}
	}
}
