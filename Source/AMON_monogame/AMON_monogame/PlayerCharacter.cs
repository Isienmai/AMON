using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	/// <summary>
	/// Enum to represent the possible states the player can be in
	/// </summary>
	enum PLAYER_STATE
	{
		PCS_NORMAL,
		PCS_WET,
		PCS_POWEREDUP
	}

	class PlayerCharacter : PhysicalObject
	{
		//the player's current state
		PLAYER_STATE playerState;

		//The player's current and modified velocities
		Vector2 currentSpeed, normalSpeed, wetMovementSpeed;
		//The player's normal and modified dimensions
		Vector2 normalSize, wetSize;

		//if true then the player is allowed to drop a grenade should they choose to
		public bool grenadesActive;

		//The event timer  for leaving the PCS_WET state. This reference is kept to allow the timer to be reset whenever the player collides with a cloud
		EventTimer dampnessTimer;

		public PlayerCharacter(Vector2 spawnLocation) : base(spawnLocation, GraphicsManager.Instance.charFine)
		{
			normalSpeed = new Vector2(700, 300);
			wetMovementSpeed = new Vector2(400, 100);

			normalSize = new Vector2(38, 46);
			wetSize = new Vector2(26, 50);

			playerState = PLAYER_STATE.PCS_NORMAL;
			currentSpeed = normalSpeed;
			dimensions = normalSize;

			grenadesActive = true;

			DrawLayer = 50;
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(Missile));
			collidableTypes.Add(typeof(Plane));
			collidableTypes.Add(typeof(Cloud));
			collidableTypes.Add(typeof(Powerup));
		}

		//Called every tick that a collision is detected
		public override void ReactToCollision(PhysicalObject other)
		{
			if (other is Cloud)
			{
				switch (playerState)
				{
					//Keep the dampness timer at max whilst inside a cloud
					case PLAYER_STATE.PCS_WET:
						if(dampnessTimer != null)
						{
							dampnessTimer.ResetTimer();
						}
						break;
					case PLAYER_STATE.PCS_NORMAL:
					case PLAYER_STATE.PCS_POWEREDUP:
						break;
				}
			}
		}

		//Called when a new collision occurs
		public override void ReactToCollisionEntry(PhysicalObject other)
		{
			//TODO: Look into IDictionary and the Visitor pattern as possible alternatives to these if/else statements
			if (other is Projectile)
			{
				switch(playerState)
				{
					case PLAYER_STATE.PCS_NORMAL:
					case PLAYER_STATE.PCS_WET:
						AudioManager.Instance.PlayAudioClip(AudioManager.AUDIOCLIPS.PATHETIC);
						Destroy();
						break;
					case PLAYER_STATE.PCS_POWEREDUP:
						AudioManager.Instance.PlayRandomPain();
						SetState(PLAYER_STATE.PCS_NORMAL);
						break;
				}
			}
			else if(other is Powerup)
			{
				SetState(PLAYER_STATE.PCS_POWEREDUP);
			}
			else if (other is Cloud)
			{
				switch (playerState)
				{
					case PLAYER_STATE.PCS_NORMAL:
						AudioManager.Instance.PlayRandomPain();
						SetState(PLAYER_STATE.PCS_WET);
						break;
					case PLAYER_STATE.PCS_WET:
						AudioManager.Instance.PlayRandomPain();
						break;
					case PLAYER_STATE.PCS_POWEREDUP:
						break;
				}
			}
		}
		
		//Negative inputs represent motion to the left or up
		public void MovePlayer(int horizontalMovement, int verticalMovement)
		{
			//convert the inputs to -1, 0, or +1 depending on their sign
			if (horizontalMovement != 0) horizontalMovement = horizontalMovement / Math.Abs(horizontalMovement);
			if (verticalMovement != 0) verticalMovement = verticalMovement / Math.Abs(verticalMovement);

			velocity.X = currentSpeed.X * horizontalMovement;
			velocity.Y = currentSpeed.Y * verticalMovement;
		}

		public void DropGrenade()
		{
			if(grenadesActive)
			{
				GameWorld.Instance.AddObject(new Grenade(this.GetCentre()));
				grenadesActive = false;
				EventManager.Instance.AddTimer(2.0f, new TimedEvent(EventEnableGrenades));
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			GameWorld.Instance.EndGame(false);
		}

		public void EventStopDamp()
		{
			if (playerState == PLAYER_STATE.PCS_WET) SetState(PLAYER_STATE.PCS_NORMAL);
			dampnessTimer = null;
		}

		public void EventEnableGrenades()
		{			
			grenadesActive = true;
		}

		//Move the player to be within the specified rectangle
		public void KeepWithinBounds(Rectangle bounds)
		{
			if (position.X < bounds.Location.X) position.X = bounds.Location.X;
			if (position.Y < bounds.Location.Y) position.Y = bounds.Location.Y;

			if (position.X + dimensions.X > bounds.Location.X + bounds.Width) position.X = (bounds.Location.X + bounds.Width) - dimensions.X;
			if (position.Y + dimensions.Y > bounds.Location.Y + bounds.Height) position.Y = (bounds.Location.Y + bounds.Height) - dimensions.Y;
		}

		public void SetState(PLAYER_STATE newState)
		{
			ExitState(playerState);
			EnterState(newState);
			playerState = newState;
		}

		private void EnterState(PLAYER_STATE state)
		{
			switch (state)
			{
				case PLAYER_STATE.PCS_NORMAL:
					currentSpeed = normalSpeed;

					sprite = GraphicsManager.Instance.charFine;
					dimensions = normalSize;
					break;
				case PLAYER_STATE.PCS_WET:
					currentSpeed = wetMovementSpeed;

					sprite = GraphicsManager.Instance.charNotFine;
					dimensions = wetSize;

					dampnessTimer = EventManager.Instance.AddTimer(3.0f, new TimedEvent(EventStopDamp));
					break;
				case PLAYER_STATE.PCS_POWEREDUP:
					currentSpeed = normalSpeed;

					sprite = GraphicsManager.Instance.charArmoured;
					dimensions = normalSize;
					break;
			}
		}

		private void ExitState(PLAYER_STATE state)
		{
			switch (state)
			{
				case PLAYER_STATE.PCS_NORMAL:
					break;
				case PLAYER_STATE.PCS_WET:
					break;
				case PLAYER_STATE.PCS_POWEREDUP:
					break;
			}
		}
	}
}
