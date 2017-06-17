using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AMON
{
	class BackgroundCastle : PhysicalObject
	{
		public BackgroundCastle(Rectangle viewportLimits) : base(new Vector2(0, viewportLimits.Height)/* + GraphicsManager.Instance.castleImage.Height)*/ , GraphicsManager.Instance.castleImage)
		{
			velocity.Y = -100;

			DrawLayer = 2;
		}

		protected override void SpecifyCollidableTypes()
		{
			//Don't collide with anything
		}
	}
}
