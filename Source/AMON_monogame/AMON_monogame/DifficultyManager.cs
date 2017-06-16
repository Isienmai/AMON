using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMON
{
	//Class to handle the game's difficulty. Currently just provides methods to calculate various time delays based on the elapsed time
	class DifficultyManager
	{
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

		public static float GetCloudDelay(float elapsedTime, Random randNumGen)
		{
			float timeRemaining = 60 - elapsedTime;

			float min = timeRemaining / 20.0f;
			float max = timeRemaining / 10.0f;

			if (min < 1) min = 1;
			if (min < 2) max = 2;

			return randNumGen.Next((int)(min + 0.5f) , (int)(max + 0.5f));
		}
	}
}
