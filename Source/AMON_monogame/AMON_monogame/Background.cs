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
	/// This class encapsulates the scrolling background texture
	/// </summary>
	class Background
	{
		//The image to scroll
		private Texture2D backgroundImage;
		//The image's vertical movement speed
		private int verticalScrollSpeed;
		//The number of tiles of the image
		private int tileCount;
		//The locations of the tiles
		Vector2[] tilePositions;

		public Background(int scrollSpeed, Viewport viewport)
		{
			backgroundImage = GraphicsManager.Instance.backgroundTexture;
			verticalScrollSpeed = scrollSpeed;
			tileCount = 2;

			tilePositions = new Vector2[tileCount];

			tilePositions[0] = new Vector2();
			tilePositions[1] = new Vector2(0, backgroundImage.Bounds.Height);
		}

		public void Tick(float dt)
		{
			//Move each tile up, snapping any tiles above the viewport to be below the viewport
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
			//draw all tiles
			for(int i = 0; i < tileCount; ++i)
			{
				spriteBatch.Draw(backgroundImage, tilePositions[i], Color.White);
			}
		}
	}
}
