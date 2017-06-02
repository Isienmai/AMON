using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	public class Bullet
	{
		public Vector2 bulletPos = new Vector2();
		public Texture2D bulletTexture;

		public void BulletCreate(Rectangle playerPos, Texture2D bt)
		{
			bulletPos.X = playerPos.X;
			bulletPos.Y = playerPos.Y;
			bulletTexture = bt;
		}
		public void BulletUpdate(int input)
		{
			bulletPos.Y += input;
		}
		public void Draw(SpriteBatch spriteBatch, Texture2D bulletTexture, Color colour)
		{
			spriteBatch.Draw(bulletTexture, bulletPos, colour);
		}
	}
}
