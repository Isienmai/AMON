using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AMON
{
	class BaseClass
	{
		private Rectangle CastleLocation;
		public Texture2D charFine, charNotFine, backgroundTexture, beginMessage, failureMessage, victoryMessage, castleImage, powerUpImage;
		public Color charColor;
		public int terminalVelocity;
		public bool started, failed, won, midwayPlayed;
		//public SpriteFont font1;
		public int playInstance;
		public bool poweredUp;
		
		public float timeElapsed;

		public int cloudNumber = 2;
		int[] cloudTextureIndex = new int[9];
		Rectangle[] testCloud = new Rectangle[9];
		public Texture2D[] cloud = new Texture2D[4];


		int timer;
		public int enemyRocketTimer, painTimer, planeTimer, explosionTimer;		

		public Animation explosion;



		public Texture2D grenadeTexture, rocketTexture, planeMovingRight, planeMovingLeft;

		private AudioManager audioManager;

		private List<PhysicalObject> allObjects;
		private Viewport viewport;

		private PlayerCharacter thePlayer;

		private Background scrollingBackground;

		public BaseClass(Viewport viewSize, ref AudioManager mainAudioManager)
		{
			viewport = viewSize;
			audioManager = mainAudioManager;
		}

		public void Initialise()
		{
			allObjects = new List<PhysicalObject>();

			timer = 0;
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

			charColor = Color.White;
			terminalVelocity = 5;
			started = false;
			failed = false;
			won = false;

			InitialiseClouds();

		}

		public void InitialiseClouds()
		{
			for (int index = 0; index <= cloudNumber - 1; index++)
			{
				testCloud[index] = new Rectangle(0, index * 10000, 0, 0);
			}

			for (int index = 0; index <= 3; index++)
			{
				cloudTextureIndex[index] = index;
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

			cloud[0] = Content.Load<Texture2D>("Images/Cloud2");
			cloud[1] = Content.Load<Texture2D>("Images/Cloud2");
			cloud[2] = Content.Load<Texture2D>("Images/Cloud4");
			cloud[3] = Content.Load<Texture2D>("Images/Cloud4");

			grenadeTexture = Content.Load<Texture2D>("Images/Bomb");
			rocketTexture = Content.Load<Texture2D>("Images/Rocket");

			explosion = new Animation(Content.Load<Texture2D>("Explosdi"), new Vector2(96, 32), 32, 32);
		}

		public void Tick(GameTime gameTime)
		{
			if (started) timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
			
			CheckAll();
			UpdateAll();

			float dt = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;

			scrollingBackground.Tick(dt);

			for (int i = 0; i < allObjects.Count; ++i)
			{
				allObjects[i].Tick(dt);
			}

			explosion.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			scrollingBackground.Draw(spriteBatch);

			spriteBatch.Draw(castleImage, CastleLocation, Color.White);
			
			//draw clouds
			DrawClouds(spriteBatch);

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
				UpdateCloud();
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
				if (new Random().Next(0, 200) < 100)
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
		}

		public void RandomizeCloud(int index)
		{
			Random randX, randY, randHeight, randWidth, randImage;
			int baseSeed = (int)DateTime.Now.Ticks;

			randHeight = new Random(baseSeed + 3 * (index + 1));
			randWidth = new Random(baseSeed + 4 * (index + 1));
			randX = new Random(baseSeed + (index + 1));
			randY = new Random(baseSeed + 2 * (index + 1));
			randImage = new Random(baseSeed * index);

			int newHeight = randHeight.Next(100, 300);
			int newWidth = randWidth.Next(150, 500);
			int newX = randX.Next(0, 800 - newWidth);
			int newY = randY.Next(480, 700);
			//cloudTextureIndex[index] = randImage.Next(0,3);
			cloudTextureIndex[index]++;
			if (cloudTextureIndex[index] > 3) cloudTextureIndex[index] -= 3;
			testCloud[index] = new Rectangle(newX, newY, newWidth, newHeight);
		}

		public void CheckAll()
		{
			if (!failed && !won)
			{
				CheckPowerUpCollision();
				CheckPlaneRocketCollision();
				CheckBombPlaneCollision();
				CheckTime();
				CheckWallCollision();
				CheckCloudCollision();
				CheckCharRocketCollision();
				CheckBombRocketCollision();
				CheckTimer();
				CheckPlaneCollision();
			}
		}

		public void CheckTime()
		{
			if ((timeElapsed > 60f) && !failed)
			{
				won = true;
				audioManager.StopBackgroundMusic();
				audioManager.PlaySpeech();
				timeElapsed = 0;
			}

			if ((timeElapsed > 30f) && (!midwayPlayed))
			{
				audioManager.PlayMidway();
				midwayPlayed = true;
			}
		}
		
		public void CheckCharRocketCollision()
		{
			/*for (int i = 0; i < rocketList.Count; i++)
			{
				if (CheckCollision(charLocation, new Rectangle((int)rocketList[i].bulletPos.X, (int)rocketList[i].bulletPos.Y, rocketList[i].bulletTexture.Width, rocketList[i].bulletTexture.Height)))
				{
					if (poweredUp)
					{
						poweredUp = false;

						audioManager.PlayWorseThanMySister();
						audioManager.PlayExplosion();

						explosion.position = new Vector2(charLocation.X, charLocation.Y);

						explosionTimer = 25;
						rocketList.Remove(rocketList[i]);

					}
					else
					{
						failed = true;
						MediaPlayer.Stop();
						audioManager.PlayPathetic();
						timeElapsed = -100f;
						audioManager.PlayExplosion();
					}
				}
			}*/
		}

		public void CheckBombRocketCollision()
		{
			/*for (int j = 0; j < bulletList.Count; j++)
			{
				for (int i = 0; i < rocketList.Count; i++)
				{
					if (CheckCollision(new Rectangle((int)bulletList[j].bulletPos.X, (int)bulletList[j].bulletPos.Y, bulletList[j].bulletTexture.Width, bulletList[j].bulletTexture.Height), new Rectangle((int)rocketList[i].bulletPos.X, (int)rocketList[i].bulletPos.Y, rocketList[i].bulletTexture.Width, rocketList[i].bulletTexture.Height)))
					{
						audioManager.PlayBrilliant();
						audioManager.PlayExplosion();

						explosion.position = new Vector2(bulletList[j].bulletPos.X, bulletList[j].bulletPos.Y);

						explosionTimer = 25;
						rocketList.Remove(rocketList[i]);
						bulletList.Remove(bulletList[j]);
					}
				}
			}*/
		}

		public void CheckPlaneRocketCollision()
		{
			/*for (int i = 0; i < rocketList.Count; i++)
			{
				if (CheckCollision(planeLocation, new Rectangle((int)rocketList[i].bulletPos.X, (int)rocketList[i].bulletPos.Y, rocketList[i].bulletTexture.Width, rocketList[i].bulletTexture.Height)))
				{
					audioManager.PlayBrilliant();
					audioManager.PlayExplosion();

					explosion.position = new Vector2(rocketList[i].bulletPos.X, rocketList[i].bulletPos.Y);
					if (!poweredUp)
					{
						powerUpLocation.X = (int)rocketList[i].bulletPos.X;
						powerUpLocation.Y = (int)rocketList[i].bulletPos.Y;
					}


					explosionTimer = 25;
					rocketList.Remove(rocketList[i]);

					if (planeSpriteUsed == 0) planeLocation.X = 800;
					if (planeSpriteUsed == 1) planeLocation.X = 0 - planeLocation.Width;
				}
			}*/
		}

		public void CheckBombPlaneCollision()
		{
			/*for (int j = 0; j < bulletList.Count; j++)
			{
				if (CheckCollision(new Rectangle((int)bulletList[j].bulletPos.X, (int)bulletList[j].bulletPos.Y, bulletList[j].bulletTexture.Width, bulletList[j].bulletTexture.Height), planeLocation))
				{
					audioManager.PlayExplosion();

					explosion.position = new Vector2(bulletList[j].bulletPos.X, bulletList[j].bulletPos.Y);

					explosionTimer = 25;
					if (planeSpriteUsed == 0) planeLocation.X = 800;
					if (planeSpriteUsed == 1) planeLocation.X = 0 - planeLocation.Width;
					bulletList.Remove(bulletList[j]);
				}
			}*/
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
				audioManager.PlayHateFalling();
				started = true;
				audioManager.StartBackgroundMusic();
			}

			if (keybState.IsKeyDown(Keys.Escape)) return 1;
			if ((keybState.IsKeyDown(Keys.Back)) && ((failed) || (won))) return 1;
			if ((keybState.IsKeyDown(Keys.Enter)) && ((failed) || (won))) Initialise();

			return 0;
		}

		public void CheckWallCollision()
		{
			/*if (charLocation.X < 0) charLocation.X = 0;
			if (charLocation.X + charLocation.Width > 800) charLocation.X = (800 - charLocation.Width);
			if (charLocation.Y < 0) charLocation.Y = 0;
			if (charLocation.Y + charLocation.Height > 480) charLocation.Y = (480 - charLocation.Height);*/
		}

		public bool CheckCollision(Rectangle input1, Rectangle input2)
		{
			if (
				((((input1.X > input2.X) && (input1.X < input2.X + input2.Width)) &&
				((input1.Y > input2.Y) && (input1.Y < input2.Y + input2.Height))) ||
				(((input1.X + input1.Width > input2.X) && (input1.X + input1.Width < input2.X + input2.Width)) &&
				((input1.Y + input1.Height > input2.Y) && (input1.Y + input1.Height < input2.Y + input2.Height))))
				||
				((((input1.X + input1.Width > input2.X) && (input1.X < input2.X + input2.Width)) &&
				((input1.Y + input1.Height > input2.Y) && (input1.Y < input2.Y + input2.Height))) ||
				(((input1.X > input2.X) && (input1.X + input1.Width < input2.X + input2.Width)) &&
				((input1.Y > input2.Y) && (input1.Y + input1.Height < input2.Y + input2.Height)))))
			{
				return true;
			}
			return false;
		}

		public void CheckCloudCollision()
		{
			/*for (int index = 0; index <= cloudNumber - 1; index++)
			{
				if (CheckCollision(charLocation, testCloud[index]))
				{
					if (!poweredUp)
					{
						if (timer == 0)
						{
							timer = 130;
						}

						if (timer <= 60) timer = 60;

						if (painTimer == 0)
						{
							audioManager.PlayRandomPain();
							painTimer = 20;
						}
					}
				}
			}*/
		}

		public void CheckPlaneCollision()
		{
			/*if (CheckCollision(charLocation, planeLocation))
			{
				if (poweredUp)
				{
					poweredUp = false;
					audioManager.PlayWorseThanMySister();
					audioManager.PlayExplosion();

					explosion.position = new Vector2(charLocation.X, charLocation.Y);

					explosionTimer = 25;
					if (planeSpriteUsed == 0) planeLocation.X = 800;
					if (planeSpriteUsed == 1) planeLocation.X = 0 - planeLocation.Width;

				}
				else
				{
					failed = true;
					audioManager.StopBackgroundMusic();
					audioManager.PlayPathetic();
				}
			}*/
		}

		public void CheckPowerUpCollision()
		{
			/*if (CheckCollision(powerUpLocation, charLocation))
			{
				poweredUp = true;
				charColor = Color.LightBlue * 0.8f;
				powerUpLocation.X = 800;
				powerUpLocation.Y = 480;
				timer = 0;
				charImage = charFine;
			}*/
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

		public void UpdateCloud()
		{
			for (int index = 0; index <= cloudNumber - 1; index++)
			{
				testCloud[index].Y -= terminalVelocity - 2;
				if (testCloud[index].Y < 0 - testCloud[index].Height)
				{
					RandomizeCloud(index);
				}
			}
		}

		public void DrawClouds(SpriteBatch spriteBatch)
		{
			for (int index = 0; index <= cloudNumber - 1; index++)
			{
				spriteBatch.Draw(cloud[cloudTextureIndex[index]], testCloud[index], Color.White);
			}
		}

		public void DrawGrenadeCooldown(SpriteBatch spriteBatch)
		{
			//if ((bulletTimer < 100) && (bulletTimer > 0))
			//	spriteBatch.DrawString(font1, "Wait for bomb: " + Convert.ToString(100 - bulletTimer), new Vector2(10, 10), Color.White);
			//else
			//	spriteBatch.DrawString(font1, "Bomb Ready!", new Vector2(10, 10), Color.White);

			//spriteBatch.DrawString(font1, "Time till impact:" + Convert.ToString((int)(60 - timeElapsed)), new Vector2(550, 10), Color.Red);
		}
	}
}
