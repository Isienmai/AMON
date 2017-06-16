using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AMON
{
	public enum GAME_STATE
	{
		MAIN_MENU = 0,
		PLAYING,
		GAME_LOST,
		GAME_WON
	}

	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		GameWorld coreGameClass;

		AudioManager theAudioManager;
		GraphicsManager theGraphicsManager;

		GAME_STATE currentGameState;

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
			theAudioManager = AudioManager.Instance;
			theGraphicsManager = GraphicsManager.Instance;
			coreGameClass = GameWorld.Instance;

			currentGameState = GAME_STATE.MAIN_MENU;

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
			ReceiveInput();
			if(currentGameState == GAME_STATE.PLAYING)
			{
				coreGameClass.Tick(gameTime);
				theGraphicsManager.Tick(gameTime);
			}

			base.Update(gameTime);
		}

		private void ReceiveInput()
		{
			KeyboardState keybState = Keyboard.GetState();

			if (keybState.IsKeyDown(Keys.Escape)) Exit();

			switch(currentGameState)
			{
				case GAME_STATE.PLAYING:
					int horizMotion = 0, verticMotion = 0;
					if (keybState.IsKeyDown(Keys.Up)) verticMotion -= 1;
					if (keybState.IsKeyDown(Keys.Down)) verticMotion += 1;
					if (keybState.IsKeyDown(Keys.Left)) horizMotion -= 1;
					if (keybState.IsKeyDown(Keys.Right)) horizMotion += 1;

					coreGameClass.UpdatePlayerInput(horizMotion, verticMotion, keybState.IsKeyDown(Keys.Space));
					break;
				case GAME_STATE.MAIN_MENU:
				case GAME_STATE.GAME_LOST:
				case GAME_STATE.GAME_WON:
					if (keybState.IsKeyDown(Keys.Back)) Exit();
					if (keybState.IsKeyDown(Keys.Enter))
					{
						SetState(GAME_STATE.PLAYING);
					}
					break;
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);			

			spriteBatch.Begin(0, null, SamplerState.PointClamp, null, null);

			switch (currentGameState)
			{
				case GAME_STATE.MAIN_MENU:
					spriteBatch.Draw(GraphicsManager.Instance.beginMessage, new Rectangle(0, 0, 800, 480), Color.White);
					break;
				case GAME_STATE.PLAYING:
					coreGameClass.Draw(spriteBatch);
					theGraphicsManager.Draw(spriteBatch);
					break;
				case GAME_STATE.GAME_LOST:
					spriteBatch.Draw(GraphicsManager.Instance.failureMessage, new Rectangle(0, 0, 800, 480), Color.White);
					break;
				case GAME_STATE.GAME_WON:
					spriteBatch.Draw(GraphicsManager.Instance.victoryMessage, new Rectangle(0, 0, 800, 480), Color.White);
					break;
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}

		public void SetState(GAME_STATE newState)
		{
			ExitState(currentGameState);
			EnterState(newState);
			currentGameState = newState;
		}

		private void EnterState(GAME_STATE state)
		{
			switch(state)
			{
				case GAME_STATE.MAIN_MENU:
					break;
				case GAME_STATE.PLAYING:
					coreGameClass.Initialise(GraphicsDevice.Viewport, this);
					theGraphicsManager.Reset();
					theAudioManager.PlayAudioClip(AudioManager.AUDIOCLIPS.HATEFALLING);
					theAudioManager.StartBackgroundMusic();
					break;
				case GAME_STATE.GAME_LOST:
					break;
				case GAME_STATE.GAME_WON:
					theAudioManager.PlayAudioClip(AudioManager.AUDIOCLIPS.TAUNT);
					break;
			}
		}

		private void ExitState(GAME_STATE state)
		{
			switch (state)
			{
				case GAME_STATE.MAIN_MENU:
					break;
				case GAME_STATE.PLAYING:
					theAudioManager.StopBackgroundMusic();
					break;
				case GAME_STATE.GAME_LOST:
					break;
				case GAME_STATE.GAME_WON:
					break;
			}
		}
	}
}
