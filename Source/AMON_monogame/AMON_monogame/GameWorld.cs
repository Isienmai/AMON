using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AMON
{
	/// <summary>
	/// Singleton holding all objects in the game and handle most of the game logic
	/// </summary>
	class GameWorld
	{
		private static GameWorld instance;

		public static GameWorld Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new GameWorld();
				}

				return instance;
			}
		}

		//Keep track of how long the game has been running (used when calling the DifficultyManager's functions)
		public float timeElapsed;
		
		//Object ot handle collision detection/resolution
		private CollisionManager collisionHandler;

		//The list of all objects currently in the scene
		private List<PhysicalObject> allObjects;

		//The size of the game's viewport
		private Viewport viewport;
		//Objects that leave this rectangle are destroyed
		private Rectangle worldBounds;

		private PlayerCharacter thePlayer;

		private Background scrollingBackground;

		private Random randNumGen;

		//Reference to the Game1 class that is running the game
		private Game1 upperGameClass;

		//The event timer that ends the game when it ends (used to display the remaining time)
		private EventTimer gameEndTimer;

		private GameWorld()
		{
			collisionHandler = new CollisionManager();
		}

		public void Initialise(Viewport viewSize, Game1 coreGameClass)
		{
			viewport = viewSize;
			worldBounds = viewport.Bounds;

			worldBounds.Location -= new Point(100, 100);
			worldBounds.Size += new Point(200, 200);

			upperGameClass = coreGameClass;

			randNumGen = new Random();

			allObjects = new List<PhysicalObject>();
			
			timeElapsed = 0;

			thePlayer = new PlayerCharacter(new Vector2(388, 10));
			allObjects.Add(thePlayer);

			scrollingBackground = new Background(60, viewport);
			
			EventManager.Instance.Reset();
			//Initial cloud, missile, and plane events
			EventManager.Instance.AddTimer(3, new TimedEvent(EventSpawnCloud));
			EventManager.Instance.AddTimer(5, new TimedEvent(EventSpawnMissile));
			EventManager.Instance.AddTimer(8, new TimedEvent(EventSpawnPlane));
			//Midpoint event
			EventManager.Instance.AddTimer(30, new TimedEvent(EventMidwayPoint));
			//Endgame events
			EventManager.Instance.AddTimer(56, new TimedEvent(EventSpawnCastle));
			gameEndTimer = EventManager.Instance.AddTimer(60, new TimedEvent(EventGameCompleted));
		}

		public void Tick(GameTime gameTime)
		{
			timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			float dt = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;

			scrollingBackground.Tick(dt);

			for (int i = 0; i < allObjects.Count; ++i)
			{
				allObjects[i].Tick(dt);
			}

			collisionHandler.HandleCollisions(allObjects, worldBounds);
			thePlayer.KeepWithinBounds(viewport.Bounds);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			scrollingBackground.Draw(spriteBatch);

			//Rather than draw all objects in the order they appear in the allObjects list a 2D array is created
			//This array references objects in the allObjects list but reorganises them by their draw layer
			//Each row represents a layer and can reference any number of objects

			//Initialise the 2D array as an array of lists
			List<int>[] orderedIndices = new List<int>[100];
			for(int i = 0; i < 100; ++i)
			{
				orderedIndices[i] = new List<int>();
			}

			//populate the array of lists
			for(int i = 0; i < allObjects.Count; ++i)
			{
				orderedIndices[allObjects[i].DrawLayer].Add(i);
			}

			//Draw the objects in order from draw layer 0 to 99
			for(int i = 0; i < 100; ++i)
			{
				for(int j = 0; j < orderedIndices[i].Count; ++j)
				{
					allObjects[orderedIndices[i][j]].Draw(spriteBatch);
				}
			}

			//display grenade cooldown
			DrawGrenadeCooldown(spriteBatch);
		}

		public void EventSpawnMissile()
		{
			allObjects.Add(new Missile(new Vector2(thePlayer.GetCentre().X, 500)));
			EventManager.Instance.AddTimer(DifficultyManager.GetMissileDelay(timeElapsed), new TimedEvent(EventSpawnMissile));
		}

		public void EventSpawnPlane()
		{
			//50:50 chance that the plane will be flying to the left or right
			if (randNumGen.Next(0, 200) < 100)
			{
				allObjects.Add(new Plane(new Vector2(viewport.Width, thePlayer.GetCentre().Y), true));
			}
			else
			{
				allObjects.Add(new Plane(new Vector2(GraphicsManager.Instance.planeMovingRight.Width * -1, thePlayer.GetCentre().Y), false));
			}

			EventManager.Instance.AddTimer(DifficultyManager.GetPlaneDelay(timeElapsed), new TimedEvent(EventSpawnPlane));
		}

		public void EventSpawnCloud()
		{
			allObjects.Add(new Cloud(viewport.Bounds));
			EventManager.Instance.AddTimer(DifficultyManager.GetCloudDelay(timeElapsed, randNumGen), new TimedEvent(EventSpawnCloud));
		}

		public void EventMidwayPoint()
		{
			//Play the midway point audio clip
			AudioManager.Instance.PlayAudioClip(AudioManager.AUDIOCLIPS.HOWLONG);
		}

		public void EventSpawnCastle()
		{
			allObjects.Add(new BackgroundCastle(viewport.Bounds));
		}

		public void EventGameCompleted()
		{
			EndGame(true);
		}

		public void UpdatePlayerInput(int horizontalMotion, int verticalMotion, bool dropBomb)
		{
			thePlayer.MovePlayer(horizontalMotion, verticalMotion);

			if (dropBomb) thePlayer.DropGrenade();
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
			//Make sure all events are wiped on game end
			EventManager.Instance.Reset();

			if (victory)
			{
				upperGameClass.SetState(Game1.GAME_STATE.GAME_WON_MENU);
			}
			else
			{
				upperGameClass.SetState(Game1.GAME_STATE.GAME_LOST_MENU);
			}
		}

		public void DrawGrenadeCooldown(SpriteBatch spriteBatch)
		{
			if (!thePlayer.grenadesActive)
			{
				GraphicsManager.Instance.DrawString(spriteBatch, "Waiting for bomb...", new Vector2(10, 10), Color.White);
			}
			else
			{
				GraphicsManager.Instance.DrawString(spriteBatch, "Bomb Ready!", new Vector2(10, 10), Color.White);
			}

			GraphicsManager.Instance.DrawString(spriteBatch, "Time till impact:" + Convert.ToString((int)(gameEndTimer.Timer + 0.5f)), new Vector2(550, 10), Color.Red);

			//Debug the out of bounds object removal
			//GraphicsManager.Instance.DrawString(spriteBatch, "Object Count:" + Convert.ToString(allObjects.Count), new Vector2(450, 20), Color.Blue);
		}
	}
}
