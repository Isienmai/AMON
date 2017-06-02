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

		BaseClass coreGameClass;

		int bombTimer = 0;

		public Animation explosion;

		public Texture2D bulletTexture, rocketTexture;
		public List<Bullet> bulletList, rocketList;

		bool bombPlayed = false;
		bool finalNukePlayed = false;

		public VideoPlayer theVideoPlayer;
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
			coreGameClass = new BaseClass();
			coreGameClass.Initialise(this);
			
			theVideoPlayer = new VideoPlayer();

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
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//video1 = Content.Load<Video>("Nuke");

			//Load the game audio
			theAudioManager.LoadContent(Content);
			//Load the game's sprites
			coreGameClass.LoadContent(Content);

			//startHere.font1 = Content.Load<SpriteFont>("SpriteFont1");
			bulletTexture = Content.Load<Texture2D>("Images/Bomb");
			rocketTexture = Content.Load<Texture2D>("Images/Rocket");

			explosion = new Animation(Content.Load<Texture2D>("Explosdi"), new Vector2(96, 32), 32, 32);
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
			//Exit the game if requested
			if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

			if ((!coreGameClass.won) && (bombTimer != 0))
			{
				bombTimer = 0;
			}

			coreGameClass.Tick(gameTime);

			if ((!bombPlayed) && (coreGameClass.won))
			{
				theVideoPlayer.IsLooped = true;
				//player.Play(video1);
				bombPlayed = true;
			}

			explosion.Update(gameTime);

			//update the bullets
			for (int i = 0; i < bulletList.Count; i++)
			{
				bulletList[i].BulletUpdate(5);

				if (bulletList[i].bulletPos.Y > GraphicsDevice.Viewport.Height)
				{
					bulletList.Remove(bulletList[i]);
				}
			}

			//update the rockets
			for (int i = 0; i < rocketList.Count; i++)
			{
				rocketList[i].BulletUpdate(-1 * (coreGameClass.terminalVelocity + 5));
				if (rocketList[i].bulletPos.Y < 0 - rocketList[i].bulletTexture.Height)
				{
					rocketList.Remove(rocketList[i]);
				}
			}

			//most innefficient botched attempt at bug-fixing ever attempted by mankind.
			if (!coreGameClass.won)
			{
				finalNukePlayed = false;
				bombPlayed = false;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);			

			spriteBatch.Begin();

			coreGameClass.Draw(spriteBatch);

			//draw bullets
			for (int i = 0; i < bulletList.Count; i++)
			{
				bulletList[i].Draw(spriteBatch, bulletTexture, Color.White);
			}

			//draw rockets
			for (int i = 0; i < rocketList.Count; i++)
			{
				rocketList[i].Draw(spriteBatch, rocketTexture, Color.White);
				rocketList[i].Draw(spriteBatch, rocketTexture, Color.Gray * 0.2f);
			}
			
			//draw small explosion
			if (coreGameClass.explosionTimer > 0)
			{
				explosion.Draw(spriteBatch);
				coreGameClass.explosionTimer--;
			}

			//draw titlescreen
			if (!coreGameClass.started) spriteBatch.Draw(coreGameClass.beginMessage, new Rectangle(0, 0, 800, 480), Color.White);
			if (coreGameClass.failed) spriteBatch.Draw(coreGameClass.failureMessage, new Rectangle(0, 0, 800, 480), Color.White);

			//draw the ending video of a large explosion
			if (coreGameClass.won)
			{
				if (theVideoPlayer.State != MediaState.Stopped)
				{
					videoTexture = theVideoPlayer.GetTexture();
				}

				if ((videoTexture != null) && (bombTimer <= 320))
				{
					spriteBatch.Draw(videoTexture, GraphicsDevice.Viewport.Bounds, Color.White);
					if (!finalNukePlayed)
					{
						theAudioManager.PlayNukeSound();
						finalNukePlayed = true;
					}
					bombTimer++;
				}

				if (bombTimer >= 300) spriteBatch.Draw(coreGameClass.victoryMessage, new Rectangle(0, 0, 800, 480), Color.White);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
