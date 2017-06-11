using Microsoft.Xna.Framework;
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

		GameWorld coreGameClass;
		
		int bombTimer;
		bool bombPlayed = false;
		bool finalNukePlayed = false;

		public VideoPlayer theVideoPlayer;
		public Texture2D videoTexture;

		public AudioManager theAudioManager;
		public GraphicsManager theGraphicsManager;
		
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
			theVideoPlayer = new VideoPlayer();
			theAudioManager = AudioManager.Instance;
			theGraphicsManager = GraphicsManager.Instance;
			coreGameClass = GameWorld.Instance;
			
			bombTimer = 0;

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
			theGraphicsManager.LoadContent(Content);
			
			coreGameClass.Initialise(GraphicsDevice.Viewport);
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
			if (coreGameClass.CheckInput() == 1) Exit();
			
			coreGameClass.Tick(gameTime);

			if ((!bombPlayed) && (coreGameClass.won))
			{
				theVideoPlayer.IsLooped = true;
				//player.Play(video1);
				bombPlayed = true;
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

			//draw titlescreen
			if (!coreGameClass.started) spriteBatch.Draw(GraphicsManager.Instance.beginMessage, new Rectangle(0, 0, 800, 480), Color.White);
			if (coreGameClass.failed) spriteBatch.Draw(GraphicsManager.Instance.failureMessage, new Rectangle(0, 0, 800, 480), Color.White);

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
						theAudioManager.PlayAudioClip(AudioManager.AUDIOCLIPS.NUKE);
						finalNukePlayed = true;
					}
					bombTimer++;
				}

				if (bombTimer >= 300) spriteBatch.Draw(GraphicsManager.Instance.victoryMessage, new Rectangle(0, 0, 800, 480), Color.White);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
