using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMON
{
	public delegate void TimedEvent();

	//Struct to store a timer and a function to call once the timer expires
	class EventTimer
	{
		private float initialTimer;
		private float timer;
		private TimedEvent expirationEvent;

		public EventTimer(float timerDuration, TimedEvent callbackFunction)
		{
			initialTimer = timerDuration;
			timer = initialTimer;
			expirationEvent = callbackFunction;
		}

		//Add an equality comparison
		public void Tick(float dt)
		{
			timer -= dt;
		}

		public void ResetTimer()
		{
			timer = initialTimer;
		}

		public bool TimerExpired()
		{
			return timer <= 0;
		}

		public float Timer
		{
			get 
			{
				return timer;
			}
		}

		public void ExecuteCallbackFunction()
		{
			expirationEvent();
		}
	}

	class EventManager
	{
		private static EventManager instance;

		private List<EventTimer> currentTimers;

		public static EventManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new EventManager();
				}

				return instance;
			}
		}

		private EventManager()
		{
			currentTimers = new List<EventTimer>();
		}

		public void Tick(float dt)
		{
			List<EventTimer> timersToRemove = new List<EventTimer>();
			for(int i = 0; i < currentTimers.Count; ++i)
			{
				currentTimers[i].Tick(dt);
				if(currentTimers[i].TimerExpired())
				{
					//Store the timer to remove FIRST as the callback function may empty the currentTimers list
					timersToRemove.Add(currentTimers[i]);
					currentTimers[i].ExecuteCallbackFunction();
				}
			}

			for(int i = 0; i < timersToRemove.Count; ++i)
			{
				currentTimers.Remove(timersToRemove[i]);
			}
		}

		public void Reset()
		{
			currentTimers.Clear();
		}

		public EventTimer AddTimer(float duration, TimedEvent callbackFunction)
		{
			if (duration == 0) duration = 0.01f;
			EventTimer newTimer = new EventTimer(duration, callbackFunction);
			currentTimers.Add(newTimer);

			return newTimer;
		}
	}
}
