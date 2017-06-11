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
		
		public Texture2D charFine, charNotFine, backgroundTexture, beginMessage, failureMessage, victoryMessage, castleImage, powerUpImage;
		public Texture2D grenadeTexture, rocketTexture, planeMovingRight, planeMovingLeft;
		public Texture2D[] cloudTextures = new Texture2D[2];

		private SpriteFont font1;
		private Animation explosion;

		private GraphicsManager()
		{

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

			backgroundTexture = Content.Load<Texture2D>("Images/Background11.fw");

			beginMessage = Content.Load<Texture2D>("Images/StartupMessage");

			failureMessage = Content.Load<Texture2D>("Images/FailureMessage");

			victoryMessage = Content.Load<Texture2D>("Images/VictoryMessage");

			cloudTextures[0] = Content.Load<Texture2D>("Images/Cloud2");
			cloudTextures[1] = Content.Load<Texture2D>("Images/Cloud4");

			grenadeTexture = Content.Load<Texture2D>("Images/Bomb");
			rocketTexture = Content.Load<Texture2D>("Images/Rocket");

			font1 = Content.Load<SpriteFont>("SpriteFont1");

			explosion = new Animation(Content.Load<Texture2D>("Explosdi"), new Vector2(96, 32), 32, 32);
		}

		public void Tick(GameTime gameTime)
		{
			explosion.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{

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
