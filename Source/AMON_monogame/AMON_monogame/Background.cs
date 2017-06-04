using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	class Background
	{
		private Texture2D backgroundImage;
		private int verticalScrollSpeed;
		private int tileCount = 2;
		Vector2[] tilePositions;

		public Background(Texture2D tilingBackgroundTexture, int scrollSpeed, Viewport viewport)
		{
			backgroundImage = tilingBackgroundTexture;
			verticalScrollSpeed = scrollSpeed;

			tilePositions = new Vector2[tileCount];

			tilePositions[0] = new Vector2();
			tilePositions[1] = new Vector2(0, backgroundImage.Bounds.Height);
		}

		public void Tick(float dt)
		{
			for(int i = 0; i < tileCount; ++i)
			{
				tilePositions[i].Y -= verticalScrollSpeed * dt;
				if(tilePositions[i].Y <= backgroundImage.Bounds.Height * -1)
				{
					tilePositions[i].Y += backgroundImage.Bounds.Height * tileCount;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for(int i = 0; i < tileCount; ++i)
			{
				spriteBatch.Draw(backgroundImage, tilePositions[i], Color.White);
			}
		}
	}
}
