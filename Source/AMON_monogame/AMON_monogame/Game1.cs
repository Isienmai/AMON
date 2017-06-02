using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AMON
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		BaseClass startHere;

		int bombTimer = 0;

		public Animation explosion;

		public Texture2D bulletTexture, rocketTexture;
		public List<Bullet> bulletList, rocketList;

		bool bombPlayed = false;
		bool finalNukePlayed = false;

		//public Video video1;
		public VideoPlayer player;
		public Texture2D videoTexture;

		public AudioManager theAudioManager;
		
		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			startHere = new BaseClass();
			startHere.Initialise(this);

			explosion = new Animation(Content.Load<Texture2D>("Explosdi"), new Vector2(96, 32), 32, 32);

			player = new VideoPlayer();

			bulletList = new List<Bullet>();
			rocketList = new List<Bullet>();

			theAudioManager = new AudioManager();

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			//video1 = Content.Load<Video>("Nuke");

			theAudioManager.LoadContent(Content);

			startHere.planeImage[0] = Content.Load<Texture2D>("Images/plane");
			startHere.planeImage[1] = Content.Load<Texture2D>("Images/Plane flipped");

			startHere.powerUpImage = Content.Load<Texture2D>("Images/Shield");

			startHere.castleImage = Content.Load<Texture2D>("Castle");

			//startHere.font1 = Content.Load<SpriteFont>("SpriteFont1");
			bulletTexture = Content.Load<Texture2D>("Images/Bomb");
			rocketTexture = Content.Load<Texture2D>("Images/Rocket");
			spriteBatch = new SpriteBatch(GraphicsDevice);
			startHere.charFine = Content.Load<Texture2D>("Images/Parachute Midget");

			startHere.charNotFine = Content.Load<Texture2D>("Images/Parachute midget damaged");

			startHere.scrolling1Texture = Content.Load<Texture2D>("Images/Background11.fw");

			startHere.scrolling2Texture = Content.Load<Texture2D>("Images/Background12.fw");

			startHere.beginMessage = Content.Load<Texture2D>("Images/StartupMessage");

			startHere.failureMessage = Content.Load<Texture2D>("Images/FailureMessage");

			startHere.victoryMessage = Content.Load<Texture2D>("Images/VictoryMessage");

			startHere.cloud[0] = Content.Load<Texture2D>("Images/Cloud2");
			startHere.cloud[1] = Content.Load<Texture2D>("Images/Cloud2");
			startHere.cloud[2] = Content.Load<Texture2D>("Images/Cloud4");
			startHere.cloud[3] = Content.Load<Texture2D>("Images/Cloud4");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if ((!startHere.won) && (bombTimer != 0))
			{
				bombTimer = 0;
			}

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (startHere.started) startHere.timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if ((!bombPlayed) && (startHere.won))
			{
				player.IsLooped = true;
				//player.Play(video1);
				bombPlayed = true;
			}

			explosion.Update(gameTime);

			startHere.CheckAll();
			startHere.UpdateAll();

			if (bulletList.Count > 0)
			{
				for (int i = 0; i < bulletList.Count; i++)
				{
					bulletList[i].BulletUpdate(5);
				}
			}

			for (int i = 0; i < bulletList.Count; i++)
			{
				if (bulletList[i].bulletPos.Y > GraphicsDevice.Viewport.Height)
				{
					bulletList.Remove(bulletList[i]);
				}
			}

			if (rocketList.Count > 0)
			{
				for (int i = 0; i < rocketList.Count; i++)
				{
					rocketList[i].BulletUpdate(-1 * (startHere.terminalVelocity + 5));
				}
			}

			for (int i = 0; i < rocketList.Count; i++)
			{
				if (rocketList[i].bulletPos.Y < 0 - rocketList[i].bulletTexture.Height)
				{
					rocketList.Remove(rocketList[i]);
				}
			}

			//most innefficient botched attempt at bug-fixing ever attempted by mankind.
			if (!startHere.won)
			{
				finalNukePlayed = false;
				bombPlayed = false;
			}


			//update enemies
			//check for end


			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			Rectangle screen = new Rectangle(GraphicsDevice.Viewport.X,
				GraphicsDevice.Viewport.Y,
				GraphicsDevice.Viewport.Width,
				GraphicsDevice.Viewport.Height);

			GraphicsDevice.Clear(Color.CornflowerBlue);

			if (player.State != MediaState.Stopped)
				videoTexture = player.GetTexture();

			spriteBatch.Begin();

			//draw background
			spriteBatch.Draw(startHere.scrolling1Texture, startHere.scrolling1, Color.White);
			spriteBatch.Draw(startHere.scrolling2Texture, startHere.scrolling2, Color.White);

			spriteBatch.Draw(startHere.castleImage, startHere.CastleLocation, Color.White);

			//draw bullets
			if (bulletList.Count > 0)
			{
				for (int i = 0; i < bulletList.Count; i++)
				{
					bulletList[i].Draw(spriteBatch, bulletTexture, Color.White);
				}
			}

			if (rocketList.Count > 0)
			{
				for (int i = 0; i < rocketList.Count; i++)
				{
					rocketList[i].Draw(spriteBatch, rocketTexture, Color.White);
				}
			}
			//draw character
			spriteBatch.Draw(startHere.charImage, startHere.charLocation, startHere.charColor);

			spriteBatch.Draw(startHere.planeImage[startHere.planeSpriteUsed], startHere.planeLocation, Color.White);

			startHere.DrawClouds(spriteBatch);

			if (rocketList.Count > 0)
			{
				for (int i = 0; i < rocketList.Count; i++)
				{
					rocketList[i].Draw(spriteBatch, rocketTexture, Color.Gray * 0.2f);
				}
			}

			spriteBatch.Draw(startHere.powerUpImage, startHere.powerUpLocation, Color.White);

			startHere.DrawGrenadeCooldown(spriteBatch);
			if (startHere.explosionTimer > 0)
			{
				explosion.Draw(spriteBatch);
				startHere.explosionTimer--;
			}
			//draw enemies


			//draw "titlescreen"
			if (!startHere.started) spriteBatch.Draw(startHere.beginMessage, new Rectangle(0, 0, 800, 480), Color.White);
			if (startHere.failed) spriteBatch.Draw(startHere.failureMessage, new Rectangle(0, 0, 800, 480), Color.White);

			if (startHere.won)
			{
				if ((videoTexture != null) && (bombTimer <= 320))
				{
					spriteBatch.Draw(videoTexture, screen, Color.White);
					if (!finalNukePlayed)
					{
						theAudioManager.PlayNukeSound();
						finalNukePlayed = true;
					}
					bombTimer++;
				}

				if (bombTimer >= 300) spriteBatch.Draw(startHere.victoryMessage, new Rectangle(0, 0, 800, 480), Color.White);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
