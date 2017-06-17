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
				
		public float timeElapsed;

		private AudioManager audioManager;
		private CollisionManager collisionHandler;

		private List<PhysicalObject> allObjects;
		private Viewport viewport;
		private Rectangle worldBounds;

		private PlayerCharacter thePlayer;

		private Background scrollingBackground;

		private Random randNumGen;

		private Game1 upperGameClass;

		private EventTimer gameEndTimer;

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

			//Setup the initial events
			EventManager.Instance.Reset();
			EventManager.Instance.AddTimer(3, new TimedEvent(EventSpawnCloud));
			EventManager.Instance.AddTimer(5, new TimedEvent(EventSpawnMissile));
			EventManager.Instance.AddTimer(8, new TimedEvent(EventSpawnPlane));
			EventManager.Instance.AddTimer(30, new TimedEvent(EventMidwayPoint));
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

			//a 2D array with each row representing a draw layer. The rows can grow dynamically to acommodate any number of objects in that layer
			List<int>[] orderedIndices = new List<int>[100];
			for(int i = 0; i < 100; ++i)
			{
				orderedIndices[i] = new List<int>();
			}

			//populate the array with object indices, all draw layer 1 objects are stored in orderedIndices[1] as an index to the allObjects list
			for(int i = 0; i < allObjects.Count; ++i)
			{
				orderedIndices[allObjects[i].DrawLayer].Add(i);
			}

			//Draw the objects in order of their draw layer starting with layer 0 (objects in the same layer are ordered by their order within allObjects)
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
			audioManager.PlayAudioClip(AudioManager.AUDIOCLIPS.HOWLONG);
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
				upperGameClass.SetState(GAME_STATE.GAME_WON);
			}
			else
			{
				upperGameClass.SetState(GAME_STATE.GAME_LOST);
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
