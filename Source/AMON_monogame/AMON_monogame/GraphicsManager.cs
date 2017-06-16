using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	//Class to store all graphical content and manage graphical effects
	public class GraphicsManager
	{
		private static GraphicsManager instance;
		
		public Texture2D charFine, charNotFine, charArmoured, backgroundTexture, beginMessage, failureMessage, victoryMessage, castleImage, powerUpImage;
		public Texture2D grenadeTexture, rocketTexture, planeMovingRight, planeMovingLeft;
		public Texture2D explosionAnimationTexture;
		public Texture2D[] cloudTextures = new Texture2D[2];

		private SpriteFont font1;
		private List<Animation> animations;



		private GraphicsManager()
		{
			animations = new List<Animation>();
		}

		public static GraphicsManager Instance
		{
			get 
			{
				if(instance == null)
				{
					instance = new GraphicsManager();
				}

				return instance;
			}
		}

		public void LoadContent(ContentManager Content)
		{
			planeMovingRight = Content.Load<Texture2D>("Images/plane");
			planeMovingLeft = Content.Load<Texture2D>("Images/Plane flipped");

			powerUpImage = Content.Load<Texture2D>("Images/Shield");

			castleImage = Content.Load<Texture2D>("Castle");

			charFine = Content.Load<Texture2D>("Images/Parachute Midget");
			charNotFine = Content.Load<Texture2D>("Images/Parachute midget damaged");
			charArmoured = Content.Load<Texture2D>("Images/ArmouredMidget");

			backgroundTexture = Content.Load<Texture2D>("Images/Background11.fw");

			beginMessage = Content.Load<Texture2D>("Images/StartupMessage");

			failureMessage = Content.Load<Texture2D>("Images/FailureMessage");

			victoryMessage = Content.Load<Texture2D>("Images/VictoryMessage");

			cloudTextures[0] = Content.Load<Texture2D>("Images/Cloud2");
			cloudTextures[1] = Content.Load<Texture2D>("Images/Cloud4");

			grenadeTexture = Content.Load<Texture2D>("Images/Bomb");
			rocketTexture = Content.Load<Texture2D>("Images/Rocket");

			font1 = Content.Load<SpriteFont>("SpriteFont1");

			explosionAnimationTexture = Content.Load<Texture2D>("Explosdi");
		}

		public void Reset()
		{
			animations.Clear();
		}

		public void Tick(GameTime gameTime)
		{
			float dt = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;

			for (int i = 0; i < animations.Count; ++i)
			{
				animations[i].Update(dt);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < animations.Count; ++i)
			{
				animations[i].Draw(spriteBatch);
			}
		}
		
		public void AddAnimation(Animation newAnimation)
		{
			animations.Add(newAnimation);
		}

		public void RemoveAnimation(Animation animationToRemove)
		{
			animations.Remove(animationToRemove);
		}

		public void DrawString(SpriteBatch spriteBatch, string textToDraw, Vector2 position, Color textColour)
		{
			spriteBatch.DrawString(font1, textToDraw, position, textColour);
		}

		public Texture2D PlaneSprite(bool movingLeft)
		{
			if (movingLeft) return planeMovingLeft;
			else return planeMovingRight;
		}
	}
}
