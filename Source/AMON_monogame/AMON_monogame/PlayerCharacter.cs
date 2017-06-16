using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AMON
{
	enum PLAYER_STATE
	{
		PCS_NORMAL,
		PCS_WET,
		PCS_POWEREDUP
	}

	class PlayerCharacter : PhysicalObject
	{
		PLAYER_STATE playerState;
		Vector2 currentSpeed, normalSpeed, wetMovementSpeed;
		Vector2 normalSize, wetSize;
		public float grenadeTimer;

		float wetTimer;

		public PlayerCharacter(Vector2 spawnLocation) : base(spawnLocation, GraphicsManager.Instance.charFine)
		{
			normalSpeed = new Vector2(700, 300);
			wetMovementSpeed = new Vector2(400, 100);

			normalSize = new Vector2(38, 46);
			wetSize = new Vector2(26, 50);

			playerState = PLAYER_STATE.PCS_NORMAL;
			currentSpeed = normalSpeed;
			dimensions = normalSize;

			grenadeTimer = 0;
			wetTimer = 0;
		}

		protected override void SpecifyCollidableTypes()
		{
			collidableTypes.Add(typeof(Missile));
			collidableTypes.Add(typeof(Plane));
			collidableTypes.Add(typeof(Cloud));
			collidableTypes.Add(typeof(Powerup));
		}

		public override void ReactToCollision(PhysicalObject other)
		{
			if (other is Cloud)
			{
				switch (playerState)
				{
					case PLAYER_STATE.PCS_NORMAL:
						AudioManager.Instance.PlayRandomPain();
						SetState(PLAYER_STATE.PCS_WET);
						break;
					case PLAYER_STATE.PCS_WET:
						//Keep the timer at max whilst inside a cloud
						wetTimer = 3;
						break;
					case PLAYER_STATE.PCS_POWEREDUP:
						break;
				}
			}
		}

		public override void ReactToCollisionEntry(PhysicalObject other)
		{
			//Look into IDictionary and the Visitor pattern as possible alternatives to these if/else statements
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
		}

		//WARNING: This method is never called right now. Its parent Tick is called instead because it's in a list of <parent object>
		public override void Tick(float deltaTime)
		{
			if (grenadeTimer > 0) grenadeTimer -= deltaTime;
			if (wetTimer > 0) wetTimer -= deltaTime;

			if (wetTimer <= 0 && playerState == PLAYER_STATE.PCS_WET) SetState(PLAYER_STATE.PCS_NORMAL);

			base.Tick(deltaTime);
		}

		/// <summary>
		/// Update the object's velocity to control the player's movement.
		/// </summary>
		/// <param name="horizontalMovement"> Negative = left, Positive = right, Zero = no motion</param>
		/// <param name="verticalMovement"> Negative = up, Positive = down, Zero = no motion</param>
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
			if(grenadeTimer <= 0)
			{
				grenadeTimer = 2;
				GameWorld.Instance.AddObject(new Grenade(this.GetCentre()));
			}
		}

		public override void Destroy()
		{
			GameWorld.Instance.EndGame(false);
			base.Destroy();
		}

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

					wetTimer = 3;
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
