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
		private Rectangle charLocation, scrolling1, scrolling2, planeLocation, CastleLocation, powerUpLocation;
		private Viewport viewport;
		public Texture2D charImage, charFine, charNotFine, scrolling1Texture, scrolling2Texture, beginMessage, failureMessage, victoryMessage, castleImage, powerUpImage;
		public Color charColor;
		public int terminalVelocity, charVertSpeed, charHorizSpeed;
		public bool started, failed, won, midwayPlayed;
		//public SpriteFont font1;
		public int playInstance;
		public bool poweredUp;

		public int planeSpriteUsed;

		public Texture2D[] planeImage = new Texture2D[4];

		public float timeElapsed;

		public int cloudNumber = 2;
		int[] cloudTextureIndex = new int[9];
		Rectangle[] testCloud = new Rectangle[9];
		public Texture2D[] cloud = new Texture2D[4];


		int timer;
		int bulletTimer;
		public int enemyRocketTimer, painTimer, planeTimer, explosionTimer;
		
		public Texture2D bulletTexture, rocketTexture;
		public List<Bullet> bulletList, rocketList;

		public Animation explosion;

		private AudioManager audioManager;
		
		public BaseClass(Viewport viewSize, ref AudioManager mainAudioManager)
		{
			Initialise();

			viewport = viewSize;
			audioManager = mainAudioManager;
		}

		public void Initialise()
		{
			timer = 0;
			poweredUp = false;
			powerUpLocation = new Rectangle(800, 480, 15, 20);
			CastleLocation = new Rectangle(0, 480, 800, 480);
			playInstance = 0;
			planeTimer = 0;
			midwayPlayed = false;
			timeElapsed = 0;
			planeSpriteUsed = 0;
			planeLocation = new Rectangle(-148, 200, 148, 38);
			enemyRocketTimer = 100;
			charLocation = new Rectangle(388, 10, 38, 46);
			charColor = Color.White;
			terminalVelocity = 5;
			charVertSpeed = 3;
			charHorizSpeed = 7;
			scrolling1 = new Rectangle(0, 0, 800, 500);
			scrolling2 = new Rectangle(0, 480, 800, 500);
			started = false;
			failed = false;
			won = false;

			InitialiseClouds();

			bulletList = new List<Bullet>();
			rocketList = new List<Bullet>();
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
			planeImage[0] = Content.Load<Texture2D>("Images/plane");
			planeImage[1] = Content.Load<Texture2D>("Images/Plane flipped");

			powerUpImage = Content.Load<Texture2D>("Images/Shield");

			castleImage = Content.Load<Texture2D>("Castle");

			charFine = Content.Load<Texture2D>("Images/Parachute Midget");

			charNotFine = Content.Load<Texture2D>("Images/Parachute midget damaged");

			scrolling1Texture = Content.Load<Texture2D>("Images/Background11.fw");

			scrolling2Texture = Content.Load<Texture2D>("Images/Background12.fw");

			beginMessage = Content.Load<Texture2D>("Images/StartupMessage");

			failureMessage = Content.Load<Texture2D>("Images/FailureMessage");

			victoryMessage = Content.Load<Texture2D>("Images/VictoryMessage");

			cloud[0] = Content.Load<Texture2D>("Images/Cloud2");
			cloud[1] = Content.Load<Texture2D>("Images/Cloud2");
			cloud[2] = Content.Load<Texture2D>("Images/Cloud4");
			cloud[3] = Content.Load<Texture2D>("Images/Cloud4");

			bulletTexture = Content.Load<Texture2D>("Images/Bomb");
			rocketTexture = Content.Load<Texture2D>("Images/Rocket");

			explosion = new Animation(Content.Load<Texture2D>("Explosdi"), new Vector2(96, 32), 32, 32);
		}

		public void Tick(GameTime gameTime)
		{
			if (started) timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
			
			CheckAll();
			UpdateAll();

			explosion.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			//draw background
			spriteBatch.Draw(scrolling1Texture, scrolling1, Color.White);
			spriteBatch.Draw(scrolling2Texture, scrolling2, Color.White);
			spriteBatch.Draw(castleImage, CastleLocation, Color.White);

			//draw character
			spriteBatch.Draw(charImage, charLocation, charColor);

			//draw plane
			spriteBatch.Draw(planeImage[planeSpriteUsed], planeLocation, Color.White);

			//draw clouds
			DrawClouds(spriteBatch);

			//draw powerup
			spriteBatch.Draw(powerUpImage, powerUpLocation, Color.White);

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
			if (explosionTimer > 0)
			{
				explosion.Draw(spriteBatch);
				explosionTimer--;
			}

			//display grenade cooldown
			DrawGrenadeCooldown(spriteBatch);
		}
				
		public void UpdatePainTimer()
		{
			if (painTimer > 0) painTimer--;
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
				CheckPlane();
			}
		}

		public void UpdateCastle()
		{
			if ((timeElapsed > 59) && (CastleLocation.Y > 10))
			{
				CastleLocation.Y -= terminalVelocity;
			}
			if (!(CastleLocation.Y > 10)) charLocation.Y += terminalVelocity;
		}

		public void UpdateAll()
		{
			if (started && !failed && !won)
			{
				UpdateCastle();
				UpdatePainTimer();
				UpdateBackground();
				UpdateCloud();
				if (timeElapsed > 10f) UpdateEnemyRocket();
				UpdatePlane();

				//update the bullets
				for (int i = 0; i < bulletList.Count; i++)
				{
					bulletList[i].BulletUpdate(5);

					if (bulletList[i].bulletPos.Y > viewport.Height)
					{
						bulletList.Remove(bulletList[i]);
					}
				}

				//update the rockets
				for (int i = 0; i < rocketList.Count; i++)
				{
					rocketList[i].BulletUpdate(-1 * (terminalVelocity + 5));
					if (rocketList[i].bulletPos.Y < 0 - rocketList[i].bulletTexture.Height)
					{
						rocketList.Remove(rocketList[i]);
					}
				}

				if (bulletTimer <= 100) bulletTimer++;
			}
		}

		public void UpdatePlane()
		{
			if (timeElapsed > 25)
			{
				if ((planeSpriteUsed == 0) && (planeTimer == 0)) planeLocation.X += 7;
				if ((planeSpriteUsed == 1) && (planeTimer == 0)) planeLocation.X -= 7;
			}
		}

		public void CheckPlane()
		{
			if (planeLocation.X > 800)
			{
				planeSpriteUsed = 1;
				planeLocation.Y = charLocation.Y;
				planeLocation.X = 800;
				if (timeElapsed < 30) planeTimer = 150;
				else
				if (timeElapsed < 45) planeTimer = 100;
				else planeTimer = 50;
			}
			if (planeLocation.X < -148)
			{
				planeSpriteUsed = 0;
				planeLocation.Y = charLocation.Y;
				planeLocation.X = -148;
				if (timeElapsed < 30) planeTimer = 150;
				else
				if (timeElapsed < 45) planeTimer = 100;
				else planeTimer = 50;
			}

			if (planeTimer != 0) planeTimer--;
		}

		public void UpdateEnemyRocket()
		{
			if (enemyRocketTimer == 0)
			{

				if (timeElapsed < 35) enemyRocketTimer = 100;
				else
				if (timeElapsed < 45) enemyRocketTimer = 80;
				else
				if (timeElapsed < 55) enemyRocketTimer = 60;
				else enemyRocketTimer = 40;
				LaunchEnemyRocket();
			}
			else enemyRocketTimer--;
		}

		public void LaunchEnemyRocket()
		{
			Bullet bulletInst = new Bullet();
			bulletInst.BulletCreate(new Rectangle(charLocation.X, 500, 8000, 6000), rocketTexture);
			rocketList.Add(bulletInst);
			bulletInst = null;
		}

		public void CheckCharRocketCollision()
		{
			for (int i = 0; i < rocketList.Count; i++)
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
			}
		}

		public void CheckBombRocketCollision()
		{
			for (int j = 0; j < bulletList.Count; j++)
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
			}
		}

		public void CheckPlaneRocketCollision()
		{
			for (int i = 0; i < rocketList.Count; i++)
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
			}
		}

		public void CheckBombPlaneCollision()
		{
			for (int j = 0; j < bulletList.Count; j++)
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
			}
		}

		public int CheckInput()
		{
			KeyboardState keybState = Keyboard.GetState();
			if (started && !failed)
			{
				if (keybState.IsKeyDown(Keys.Up)) charLocation.Y -= charVertSpeed;
				if (keybState.IsKeyDown(Keys.Down)) charLocation.Y += charVertSpeed;
				if (keybState.IsKeyDown(Keys.Left)) charLocation.X -= charHorizSpeed;
				if (keybState.IsKeyDown(Keys.Right)) charLocation.X += charHorizSpeed;
				if (keybState.IsKeyDown(Keys.Space) && bulletTimer > 100)
				{
					bulletTimer = 0;
					Bullet bulletInst = new Bullet();
					bulletInst.BulletCreate(new Rectangle(charLocation.X + 10, charLocation.Y + 40, 0, 0), bulletTexture);
					bulletList.Add(bulletInst);
					bulletInst = null;
				}
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
			if (charLocation.X < 0) charLocation.X = 0;
			if (charLocation.X + charLocation.Width > 800) charLocation.X = (800 - charLocation.Width);
			if (charLocation.Y < 0) charLocation.Y = 0;
			if (charLocation.Y + charLocation.Height > 480) charLocation.Y = (480 - charLocation.Height);
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
			for (int index = 0; index <= cloudNumber - 1; index++)
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
			}
		}

		public void CheckPlaneCollision()
		{
			if (CheckCollision(charLocation, planeLocation))
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
			}
		}

		public void CheckPowerUpCollision()
		{
			if (CheckCollision(powerUpLocation, charLocation))
			{
				poweredUp = true;
				charColor = Color.LightBlue * 0.8f;
				powerUpLocation.X = 800;
				powerUpLocation.Y = 480;
				timer = 0;
				charImage = charFine;
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

		public void UpdateBackground()
		{
			scrolling1.Y -= terminalVelocity - 3;
			scrolling2.Y -= terminalVelocity - 3;
			if (scrolling1.Y + scrolling1Texture.Height <= 0)
				scrolling1.Y = scrolling2.Y + scrolling2Texture.Height;
			if (scrolling2.Y + scrolling2Texture.Height <= 0)
				scrolling2.Y = scrolling1.Y + scrolling1Texture.Height;
		}

		public void CheckTimer()
		{
			if (timer > 0)
			{
				charImage = charNotFine;
				charColor = Color.Aquamarine;
				timer--;
				charLocation.Height = 50;
				charLocation.Width = 26;
				charVertSpeed = 1;
				charHorizSpeed = 4;
				terminalVelocity = 9;
			}
			else
			{
				if (!poweredUp) charColor = Color.White;
				charImage = charFine;
				charLocation.Height = 46;
				charLocation.Width = 38;
				charVertSpeed = 3;
				charHorizSpeed = 7;
				terminalVelocity = 7;
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
