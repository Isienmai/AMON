using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AMON
{
	public class Animation
	{
		Texture2D texture;
		Rectangle frameSource;
		public Vector2 position;
		public Vector2 origin;

		int currentFrame;
		int frameCount;

		float timer;
		float frameDuration;

		bool playOnce;

		public Animation(Texture2D newTexture, Vector2 newPosition, float newFrameHeight, float newFrameWidth, int numOfFrames)
		{
			texture = newTexture;
			position = newPosition;

			frameSource = new Rectangle(0, 0, (int)(newFrameWidth + 0.5f), (int)(newFrameHeight + 0.5f));
			origin = new Vector2(newFrameWidth / 2.0f, newFrameHeight / 2.0f);

			frameDuration = 0.1f;
			frameCount = numOfFrames;

			playOnce = true;
		}

		public void Update(float dt)
		{
			timer += dt;
			if(timer > frameDuration)
			{
				currentFrame++;
				timer = 0;
				if (currentFrame >= frameCount)
				{
					if(playOnce)
					{
						Destroy();
					}
					else
					{
						currentFrame = 0;
					}
				}
			}

			frameSource.X = currentFrame * frameSource.Width;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, frameSource, Color.White, 0f, origin, 1.0f, SpriteEffects.None, 0);
		}

		public virtual void Destroy()
		{
			GraphicsManager.Instance.RemoveAnimation(this);
		}
	}

	public class ExplosionAnimation : Animation
	{
		public ExplosionAnimation(Vector2 position) : base(GraphicsManager.Instance.explosionAnimationTexture, position, 32, 32, 3) { }
	}
}
