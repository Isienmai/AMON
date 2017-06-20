using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMON
{
	//Callback function definition
	public delegate void TimedEvent();

	/// <summary>
	/// Represents a single event, with an internal timer and a callback function
	/// </summary>
	class EventTimer
	{
		//The specified time delay
		private float initialTimer;
		//Time remaining before the event fires
		private float timer;
		//Delegate function to call when the timer expires
		private TimedEvent expirationEvent;

		public EventTimer(float timerDuration, TimedEvent callbackFunction)
		{
			initialTimer = timerDuration;
			timer = initialTimer;
			expirationEvent = callbackFunction;
		}
		
		//Update the timer
		public void Tick(float dt)
		{
			timer -= dt;

			//call the delegate and destroy this if the timer has run out
			if (timer <= 0)
			{
				expirationEvent();
				Destroy();
			}
		}

		//Allow the timer to be reset to it's initial value
		public void ResetTimer()
		{
			timer = initialTimer;
		}

		public float Timer
		{
			get 
			{
				return timer;
			}
		}

		//Remove the timer from the EventManager's timer list
		//NOTE: this may cause problems if the timer continues to be referenced from elsewhere
		public void Destroy()
		{
			EventManager.Instance.RemoveTimer(this);
		}
	}

	/// <summary>
	/// A singleton class to manage all timed events.
	/// </summary>
	class EventManager
	{
		private static EventManager instance;

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


		//Timed events currently being managed
		private List<EventTimer> currentTimers;

		private EventManager()
		{
			currentTimers = new List<EventTimer>();
		}

		public void Tick(float dt)
		{
			//Loop through a copy of the currentTimers list and tick each timer
			foreach(EventTimer timer in currentTimers.ToArray())
			{
				timer.Tick(dt);
			}
		}

		public void Reset()
		{
			currentTimers.Clear();
		}

		public EventTimer AddTimer(float duration, TimedEvent callbackFunction)
		{
			EventTimer newTimer = new EventTimer(duration, callbackFunction);
			currentTimers.Add(newTimer);

			return newTimer;
		}

		public void RemoveTimer(EventTimer timerToRemove)
		{
			currentTimers.Remove(timerToRemove);
		}
	}
}
