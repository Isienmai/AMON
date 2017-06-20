using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMON
{
	/// <summary>
	/// Class to control the game's difficulty curve
	/// </summary>
	class DifficultyManager
	{
		//Control missile difficulty curve
		public static float GetMissileDelay(float elapsedTime)
		{
			if (elapsedTime < 35)
			{
				return 3.0f;
			}
			else if (elapsedTime < 45)
			{
				return 2.0f;
			}
			else if (elapsedTime < 55)
			{
				return 1.0f;
			}
			else
			{
				return 0.5f;
			}
		}

		//Control plane difficulty curve
		public static float GetPlaneDelay(float elapsedTime)
		{
			if (elapsedTime < 35)
			{
				return 8.0f;
			}
			else if (elapsedTime < 45)
			{
				return 5.0f;
			}
			else if (elapsedTime < 55)
			{
				return 2.5f;
			}
			else
			{
				return 1.0f; ;
			}
		}

		//Control cloud difficulty curve
		public static float GetCloudDelay(float elapsedTime, Random randNumGen)
		{
			float timeRemaining = 60 - elapsedTime;

			//Use remaining time to define the range of possible cloud delays
			float min = timeRemaining / 20.0f;
			float max = timeRemaining / 10.0f;

			//Avoid invalid ranges
			if (min < 1) min = 1;
			if (min < 2) max = 2;

			//Randomly generate the time delay on the next cloud spawn using the specified range
			return randNumGen.Next((int)(min + 0.5f) , (int)(max + 0.5f));
		}
	}
}
