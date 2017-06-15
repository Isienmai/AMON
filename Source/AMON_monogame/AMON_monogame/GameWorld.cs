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
		
		public bool midwayPlayed;
		public int playInstance;
		
		public float timeElapsed;
				
		public int enemyRocketTimer, planeTimer, explosionTimer, powerupTimer, cloudTimer;

		private AudioManager audioManager;
		private CollisionManager collisionHandler;

		private List<PhysicalObject> allObjects;
		private Viewport viewport;

		private PlayerCharacter thePlayer;

		private Background scrollingBackground;

		private Random randNumGen;

		private Game1 upperGameClass;

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

		public void Initialise(Viewport viewSize, Game1 coreGameClass)
		{
			viewport = viewSize;
			upperGameClass = coreGameClass;

			randNumGen = new Random();

			allObjects = new List<PhysicalObject>();

			powerupTimer = 0;
			playInstance = 0;
			planeTimer = 500;
			midwayPlayed = false;
			timeElapsed = 0;
			enemyRocketTimer = 100;

			thePlayer = new PlayerCharacter(new Vector2(388, 10));
			allObjects.Add(thePlayer);

			scrollingBackground = new Background(60, viewport);
		}

		public void Tick(GameTime gameTime)
		{
			timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			float dt = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;

			CheckTime();
			CheckWallCollision();
			CheckTimer();
			if (timeElapsed > 3f) UpdateEnemyWeapons();

			scrollingBackground.Tick(dt);

			for (int i = 0; i < allObjects.Count; ++i)
			{
				allObjects[i].Tick(dt);
			}

			collisionHandler.HandleCollisions(allObjects);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			scrollingBackground.Draw(spriteBatch);
			
			for(int i = 0; i < allObjects.Count; ++i)
			{
				allObjects[i].Draw(spriteBatch);
			}

			//draw small explosion
			if (explosionTimer > 0)
			{
				//explosion.Draw(spriteBatch);
				explosionTimer--;
			}

			//display grenade cooldown
			DrawGrenadeCooldown(spriteBatch);
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

				allObjects.Add(new Missile(new Vector2(thePlayer.GetCentre().X, 500)));
			
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
					allObjects.Add(new Plane(new Vector2(viewport.Width, thePlayer.GetCentre().Y), true));
				}
				else
				{
					allObjects.Add(new Plane(new Vector2(GraphicsManager.Instance.planeMovingRight.Width * -1, thePlayer.GetCentre().Y),  false));
				}
			}
			else
			{
				planeTimer--;
			}

			if(cloudTimer == 0)
			{
				cloudTimer = randNumGen.Next((int)timeElapsed * 4, (int)timeElapsed * 10);

				allObjects.Add(new Cloud(viewport.Bounds));
			}
			else
			{
				cloudTimer--;
			}
		}

		public void CheckTime()
		{
			if ((timeElapsed > 60f))
			{
				upperGameClass.SetState(GAME_STATE.GAME_WON);
			}

			if ((timeElapsed > 30f) && !midwayPlayed)
			{
				audioManager.PlayAudioClip(AudioManager.AUDIOCLIPS.HOWLONG);
				midwayPlayed = true;
			}
		}

		public void UpdatePlayerInput(int horizontalMotion, int verticalMotion, bool dropBomb)
		{
			thePlayer.MovePlayer(horizontalMotion, verticalMotion);

			if (dropBomb) thePlayer.DropGrenade();
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
			if(victory)
			{
				upperGameClass.SetState(GAME_STATE.GAME_WON);
			}
			else
			{
				upperGameClass.SetState(GAME_STATE.GAME_LOST);
			}
		}

		public void DrawGrenadeCooldown(SpriteBatch spriteBatch)
		{
			if (thePlayer.grenadeTimer > 0)
			{
				GraphicsManager.Instance.DrawString(spriteBatch, "Wait for bomb: " + thePlayer.grenadeTimer.ToString("n2"), new Vector2(10, 10), Color.White);
			}
			else
			{
				GraphicsManager.Instance.DrawString(spriteBatch, "Bomb Ready!", new Vector2(10, 10), Color.White);
			}

			GraphicsManager.Instance.DrawString(spriteBatch, "Time till impact:" + Convert.ToString((int)(60 - timeElapsed)), new Vector2(550, 10), Color.Red);
		}
	}
}
