using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AMON
{
	/// <summary>
	/// This class encapsulates everything that is required to display a sprite animation taken from a spritesheet.
	/// </summary>
	public class Animation
	{
		//THe spritesheet
		Texture2D texture;
		//Position and size of the first animation sprite in the sprite sheet
		Rectangle frameSource;
		//Location of the current animation frame
		Vector2 position;
		
		//THe frame currently being shown and the total number of frames in the animation
		int currentFrame;
		int frameCount;
		
		//Duration of each frame
		float frameDuration;

		//If true the animation will be destroyed after showing all frames once
		bool playOnce;

		//The centre of the sprite (rather than drawing from the top left corner draw from the sprite's centre instead)
		Vector2 origin;

		public Animation(Texture2D newTexture, Vector2 newPosition, float newFrameHeight, float newFrameWidth, int numOfFrames)
		{
			texture = newTexture;
			position = newPosition;

			frameSource = new Rectangle(0, 0, (int)(newFrameWidth + 0.5f), (int)(newFrameHeight + 0.5f));

			frameDuration = 0.1f;
			frameCount = numOfFrames;

			playOnce = true;

			currentFrame = 0;
			EventManager.Instance.AddTimer(frameDuration, new TimedEvent(UpdateAnimationFrame));

			origin = new Vector2(newFrameWidth / 2.0f, newFrameHeight / 2.0f);
		}

		public void UpdateAnimationFrame()
		{
			//Increment the frame
			currentFrame++;
			if (currentFrame >= frameCount)
			{
				if (!playOnce)
				{
					currentFrame = 0;
				}
				else
				{
					//Destroy the object if the animation should only play once
					Destroy();
					return;
				}
			}

			//Update the frame position
			frameSource.X = currentFrame * frameSource.Width;

			//Set the event timer to update the animation again
			EventManager.Instance.AddTimer(frameDuration, new TimedEvent(UpdateAnimationFrame));
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

	/// <summary>
	/// A class that encapsulates all the data needed to spawn the Explosion animation.
	/// </summary>
	public class ExplosionAnimation : Animation
	{
		public ExplosionAnimation(Vector2 position) : base(GraphicsManager.Instance.explosionAnimationTexture, position, 32, 32, 3) { }
	}
}
