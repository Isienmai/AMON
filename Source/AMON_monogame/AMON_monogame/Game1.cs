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
		//Monogame classes
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		//Store all physical objects and handle both physics and draw updates
		GameWorld coreGameClass;
		//Store all audio clips and play them on request
		AudioManager theAudioManager;
		//Store all textures and handle the drawing of animations
		GraphicsManager theGraphicsManager;
		//Handle all ingame timers/delays
		EventManager theEventManager;
		

		/// <summary>
		/// This stores the possible states that Game1 can be in.
		/// </summary>
		public enum GAME_STATE
		{
			MAIN_MENU = 0,
			PLAYING,
			GAME_LOST_MENU,
			GAME_WON_MENU
		}
		//Current state
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
			//Initialise all singletons
			coreGameClass = GameWorld.Instance;
			theAudioManager = AudioManager.Instance;
			theGraphicsManager = GraphicsManager.Instance;
			theEventManager = EventManager.Instance;

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
			//React to any keypresses
			ReceiveInput();

			//Tick all core game classes if the game is not in one of the menu states
			if(currentGameState == GAME_STATE.PLAYING)
			{
				coreGameClass.Tick(gameTime);

				float dt = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
				theEventManager.Tick(dt);
			}

			base.Update(gameTime);
		}

		private void ReceiveInput()
		{
			KeyboardState keybState = Keyboard.GetState();

			//Allow escape to quit the game
			if (keybState.IsKeyDown(Keys.Escape)) Exit();

			switch(currentGameState)
			{
				//Control the game character
				case GAME_STATE.PLAYING:
					int horizMotion = 0, verticMotion = 0;
					if (keybState.IsKeyDown(Keys.Up)) verticMotion -= 1;
					if (keybState.IsKeyDown(Keys.Down)) verticMotion += 1;
					if (keybState.IsKeyDown(Keys.Left)) horizMotion -= 1;
					if (keybState.IsKeyDown(Keys.Right)) horizMotion += 1;

					coreGameClass.UpdatePlayerInput(horizMotion, verticMotion, keybState.IsKeyDown(Keys.Space));
					break;
				//Exit or Play the game
				case GAME_STATE.MAIN_MENU:
				case GAME_STATE.GAME_LOST_MENU:
				case GAME_STATE.GAME_WON_MENU:
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
			spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);

			//Either draw the game's graphics or draw the appropriate menu
			switch (currentGameState)
			{
				case GAME_STATE.PLAYING:
					coreGameClass.Draw(spriteBatch);
					theGraphicsManager.Draw(spriteBatch);
					break;
				case GAME_STATE.MAIN_MENU:
					spriteBatch.Draw(GraphicsManager.Instance.beginMessage, new Rectangle(0, 0, 800, 480), Color.White);
					break;
				case GAME_STATE.GAME_LOST_MENU:
					spriteBatch.Draw(GraphicsManager.Instance.failureMessage, new Rectangle(0, 0, 800, 480), Color.White);
					break;
				case GAME_STATE.GAME_WON_MENU:
					spriteBatch.Draw(GraphicsManager.Instance.victoryMessage, new Rectangle(0, 0, 800, 480), Color.White);
					break;
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}

		/// <summary>
		/// Update the game's state by exiting one state, entering a new state, and updating the current state to the new state.
		/// </summary>
		/// <param name="newState"></param>
		public void SetState(GAME_STATE newState)
		{
			ExitState(currentGameState);
			EnterState(newState);
			currentGameState = newState;
		}

		/// <summary>
		/// Do everything that is required when a given state has been entered
		/// </summary>
		/// <param name="state"></param>
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
				case GAME_STATE.GAME_LOST_MENU:
					break;
				case GAME_STATE.GAME_WON_MENU:
					theAudioManager.PlayAudioClip(AudioManager.AUDIOCLIPS.TAUNT);
					break;
			}
		}

		/// <summary>
		/// Do everything that is required when a given state has been exited
		/// </summary>
		/// <param name="state"></param>
		private void ExitState(GAME_STATE state)
		{
			switch (state)
			{
				case GAME_STATE.MAIN_MENU:
					break;
				case GAME_STATE.PLAYING:
					theAudioManager.StopBackgroundMusic();
					break;
				case GAME_STATE.GAME_LOST_MENU:
					break;
				case GAME_STATE.GAME_WON_MENU:
					break;
			}
		}
	}
}
