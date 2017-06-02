using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace AMON
{
	public class AudioManager
	{
		private SoundEffect brilliant, speech, hateFalling, midway, pathetic, worseThanMySister, explosion, nukeSound;
		private List<SoundEffect> pain;
		private Song background1;

		public AudioManager()
		{
			pain = new List<SoundEffect>();
			SetMediaPlayerVolume(5);
		}

		public void LoadContent(ContentManager Content)
		{
			background1 = Content.Load<Song>("Super Street Fighter 2 - Guile's Stage");
			brilliant = Content.Load<SoundEffect>("Sounds/Brilliant");
			speech = Content.Load<SoundEffect>("Sounds/CrazySpeech");
			hateFalling = Content.Load<SoundEffect>("Sounds/HateFallingShout");
			midway = Content.Load<SoundEffect>("Sounds/HowLongUntilIWin");
			pathetic = Content.Load<SoundEffect>("Sounds/Pathetic");
			worseThanMySister = Content.Load<SoundEffect>("Sounds/WorseThanMySister");
			explosion = Content.Load<SoundEffect>("Sounds/Explosion");
			nukeSound = Content.Load<SoundEffect>("Nukes");


			pain.Add(Content.Load<SoundEffect>("Sounds/Pain"));
			pain.Add(Content.Load<SoundEffect>("Sounds/Pain1"));
			pain.Add(Content.Load<SoundEffect>("Sounds/Pain2"));
			pain.Add(Content.Load<SoundEffect>("Sounds/Pain3"));
		}

		public void SetMediaPlayerVolume(float newVolume)
		{
			MediaPlayer.Volume = newVolume;
		}

		public bool StartBackgroundMusic()
		{
			bool audioLoaded = false;

			if(background1 != null)
			{
				audioLoaded = true;
				MediaPlayer.Play(background1);
			}

			return audioLoaded;
		}

		public void StopBackgroundMusic()
		{
			MediaPlayer.Stop();
		}

		public void PlayBrilliant()
		{
			brilliant.Play();
		}

		public void PlaySpeech()
		{
			speech.Play();
		}

		public void PlayHateFalling()
		{
			hateFalling.Play();
		}

		public void PlayMidway()
		{
			midway.Play();
		}

		public void PlayPathetic()
		{
			pathetic.Play();
		}

		public void PlayWorseThanMySister()
		{
			worseThanMySister.Play();
		}

		public void PlayExplosion()
		{
			explosion.Play();
		}

		public void PlayNukeSound()
		{
			nukeSound.Play();
		}

		public void PlayRandomPain()
		{
			Random tempRand = new Random();
			pain[tempRand.Next(pain.Count)].Play();
		}
	}
}
