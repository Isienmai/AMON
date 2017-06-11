using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AMON
{
	class GameWorld
	{
		private static GameWorld instance;

		private Rectangle CastleLocation;
		public Texture2D charFine, charNotFine, backgroundTexture, beginMessage, failureMessage, victoryMessage, castleImage, powerUpImage;
		public int terminalVelocity;
		public bool started, failed, won, midwayPlayed;
		public SpriteFont font1;
		public int playInstance;
		public bool poweredUp;
		
		public float timeElapsed;
		
		public Texture2D[] cloudTextures = new Texture2D[2];
				
		public int enemyRocketTimer, painTimer, planeTimer, explosionTimer, powerupTimer, cloudTimer;

		public Animation explosion;



		public Texture2D grenadeTexture, rocketTexture, planeMovingRight, planeMovingLeft;

		private AudioManager audioManager;
		private CollisionManager collisionHandler;

		private List<PhysicalObject> allObjects;
		private Viewport viewport;

		private PlayerCharacter thePlayer;

		private Background scrollingBackground;

		private Random randNumGen;

		public static GameWorld Instance
		{
			get 
			{
				if(instance == null)
				{
					instance = new GameWorld();
				}

				return instance;
			}
		}

		private GameWorld()
		{
			audioManager = AudioManager.Instance;
			collisionHandler = new CollisionManager();
		}

		public void Initialise(Viewport viewSize)
		{
			viewport = viewSize;

			randNumGen = new Random();

			allObjects = new List<PhysicalObject>();

			powerupTimer = 0;
			poweredUp = false;
			CastleLocation = new Rectangle(0, 480, 800, 480);
			playInstance = 0;
			planeTimer = 500;
			midwayPlayed = false;
			timeElapsed = 0;
			enemyRocketTimer = 100;

			thePlayer = new PlayerCharacter(new Vector2(388, 10), charFine);
			allObjects.Add(thePlayer);

			scrollingBackground = new Background(backgroundTexture, 60, viewport);

			terminalVelocity = 5;
			started = false;
			failed = false;
			won = false;

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
			if (started) timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			float dt = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;

			CheckAll();
			UpdateAll();

			scrollingBackground.Tick(dt);

			for (int i = 0; i < allObjects.Count; ++i)
			{
				allObjects[i].Tick(dt);
			}

			collisionHandler.HandleCollisions(allObjects);

			explosion.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			scrollingBackground.Draw(spriteBatch);

			spriteBatch.Draw(castleImage, CastleLocation, Color.White);
			
			for(int i = 0; i < allObjects.Count; ++i)
			{
				allObjects[i].Draw(spriteBatch);
			}

			//draw small explosion
			if (explosionTimer > 0)
			{
				explosion.Draw(spriteBatch);
				explosionTimer--;
			}

			//display grenade cooldown
			DrawGrenadeCooldown(spriteBatch);
		}

		public void UpdateAll()
		{
			if (started && !failed && !won)
			{
				UpdateCastle();
				UpdatePainTimer();
				if (timeElapsed > 10f) UpdateEnemyWeapons();
			}
		}

		public void UpdateCastle()
		{
			if ((timeElapsed > 59) && (CastleLocation.Y > 10))
			{
				CastleLocation.Y -= terminalVelocity;
			}
			//if (!(CastleLocation.Y > 10)) charLocation.Y += terminalVelocity;
		}

		public void UpdatePainTimer()
		{
			if (painTimer > 0) painTimer--;
		}
				
		public void UpdateEnemyWeapons()
		{
			if (enemyRocketTimer == 0)
			{
				if (timeElapsed < 35)
				{
					enemyRocketTimer = 100;
				}
				else
				if (timeElapsed < 45)
				{
					enemyRocketTimer = 80;
				}
				else
				if (timeElapsed < 55)
				{
					enemyRocketTimer = 60;
				}
				else
				{
					enemyRocketTimer = 40;
				}

				allObjects.Add(new Missile(new Vector2(thePlayer.GetCentre().X, 500), rocketTexture));
			
			}
			else
			{
				enemyRocketTimer--;
			}


			if (planeTimer == 0)
			{
				if (timeElapsed < 35)
				{
					planeTimer = 250;
				}
				else
				if (timeElapsed < 45)
				{
					planeTimer = 200;
				}
				else
				if (timeElapsed < 55)
				{
					planeTimer = 150;
				}
				else
				{
					planeTimer = 100;
				}
				
				//randomise direction of movement
				if (randNumGen.Next(0, 200) < 100)
				{
					allObjects.Add(new Plane(new Vector2(viewport.Width, thePlayer.GetCentre().Y), planeMovingLeft, true));
				}
				else
				{
					allObjects.Add(new Plane(new Vector2(planeMovingRight.Width * -1, thePlayer.GetCentre().Y), planeMovingRight, false));
				}
			}
			else
			{
				planeTimer--;
			}

			if(cloudTimer == 0)
			{
				cloudTimer = randNumGen.Next((int)timeElapsed * 4, (int)timeElapsed * 10);

				allObjects.Add(new Cloud(viewport.Bounds, cloudTextures[randNumGen.Next(0,2)]));
			}
			else
			{
				cloudTimer--;
			}
		}

		public void CheckAll()
		{
			if (!failed && !won)
			{
				CheckTime();
				CheckWallCollision();
				CheckTimer();
			}
		}

		public void CheckTime()
		{
			if ((timeElapsed > 60f) && !failed)
			{
				won = true;
				audioManager.StopBackgroundMusic();
				audioManager.PlayAudioClip(AudioManager.AUDIOCLIPS.TAUNT);
				timeElapsed = 0;
			}

			if ((timeElapsed > 30f) && (!midwayPlayed))
			{
				audioManager.PlayAudioClip(AudioManager.AUDIOCLIPS.HOWLONG);
				midwayPlayed = true;
			}
		}

		public int CheckInput()
		{
			KeyboardState keybState = Keyboard.GetState();
			if (started && !failed)
			{
				int horizMotion = 0, verticMotion = 0;
				if (keybState.IsKeyDown(Keys.Up)) verticMotion -= 1;
				if (keybState.IsKeyDown(Keys.Down)) verticMotion += 1;
				if (keybState.IsKeyDown(Keys.Left)) horizMotion -= 1;
				if (keybState.IsKeyDown(Keys.Right)) horizMotion += 1;

				thePlayer.MovePlayer(horizMotion, verticMotion);

				if (keybState.IsKeyDown(Keys.Space)) thePlayer.DropGrenade();
			}
			else if ((keybState.IsKeyDown(Keys.Enter)) && !started)
			{
				audioManager.PlayAudioClip(AudioManager.AUDIOCLIPS.HATEFALLING);
				started = true;
				audioManager.StartBackgroundMusic();
			}

			if (keybState.IsKeyDown(Keys.Escape)) return 1;
			if ((keybState.IsKeyDown(Keys.Back)) && ((failed) || (won))) return 1;
			if ((keybState.IsKeyDown(Keys.Enter)) && ((failed) || (won))) Initialise(viewport);

			return 0;
		}

		public void CheckWallCollision()
		{
			/*if (charLocation.X < 0) charLocation.X = 0;
			if (charLocation.X + charLocation.Width > 800) charLocation.X = (800 - charLocation.Width);
			if (charLocation.Y < 0) charLocation.Y = 0;
			if (charLocation.Y + charLocation.Height > 480) charLocation.Y = (480 - charLocation.Height);*/
		}

		public void CheckTimer()
		{
			/*if (timer > 0)
			{
				charImage = charNotFine;
				charColor = Color.Aquamarine;
				timer--;
				charLocation.Height = 50;
				charLocation.Width = 26;
				terminalVelocity = 9;
			}
			else
			{
				if (!poweredUp) charColor = Color.White;
				charImage = charFine;
				charLocation.Height = 46;
				charLocation.Width = 38;
				terminalVelocity = 7;
			}*/
		}

		public void AddObject(PhysicalObject objToAdd)
		{
			allObjects.Add(objToAdd);
		}

		public void RemoveObject(PhysicalObject objToRemove)
		{
			allObjects.Remove(objToRemove);
		}

		public void EndGame(bool victory)
		{
			failed = !victory;
			won = victory;

			audioManager.StopBackgroundMusic();
		}

		public void DrawGrenadeCooldown(SpriteBatch spriteBatch)
		{
			if (thePlayer.grenadeTimer > 0)
			{
				spriteBatch.DrawString(font1, "Wait for bomb: " + thePlayer.grenadeTimer.ToString("n2"), new Vector2(10, 10), Color.White);
			}
			else
			{
				spriteBatch.DrawString(font1, "Bomb Ready!", new Vector2(10, 10), Color.White);
			}

			spriteBatch.DrawString(font1, "Time till impact:" + Convert.ToString((int)(60 - timeElapsed)), new Vector2(550, 10), Color.Red);
		}
	}
}
