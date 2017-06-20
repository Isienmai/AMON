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
	/// <summary>
	/// A singleton class for storing and playing any audio clips/background music.
	/// </summary>
	public class AudioManager
	{
		private static AudioManager instance;

		public static AudioManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new AudioManager();
				}

				return instance;
			}
		}

		/// <summary>
		/// Enum used to identify all audio clips stored in the audioClips list
		/// </summary>
		public enum AUDIOCLIPS
		{
			BRILLIANT = 0,
			TAUNT,
			HATEFALLING,
			HOWLONG,
			PATHETIC,
			WORSETHANSISTER,
			EXPLOSION,
			NUKE,
			COUNT
		}

		//List of individual audio clips
		private List<SoundEffect> audioClips;
		//List of pain noises
		private List<SoundEffect> pain;
		//Backround music
		private Song background1;

		private AudioManager()
		{
			pain = new List<SoundEffect>();
			SetMediaPlayerVolume(5);
		}

		public void LoadContent(ContentManager Content)
		{
			audioClips = new List<SoundEffect>(new SoundEffect[(int)AUDIOCLIPS.COUNT]);
			background1 = Content.Load<Song>("Super Street Fighter 2 - Guile's Stage");

			audioClips[(int)AUDIOCLIPS.BRILLIANT] = Content.Load<SoundEffect>("Sounds/Brilliant");
			audioClips[(int)AUDIOCLIPS.TAUNT] = Content.Load<SoundEffect>("Sounds/CrazySpeech");
			audioClips[(int)AUDIOCLIPS.HATEFALLING] = Content.Load<SoundEffect>("Sounds/HateFallingShout");
			audioClips[(int)AUDIOCLIPS.HOWLONG] = Content.Load<SoundEffect>("Sounds/HowLongUntilIWin");
			audioClips[(int)AUDIOCLIPS.PATHETIC] = Content.Load<SoundEffect>("Sounds/Pathetic");
			audioClips[(int)AUDIOCLIPS.WORSETHANSISTER] = Content.Load<SoundEffect>("Sounds/WorseThanMySister");
			audioClips[(int)AUDIOCLIPS.EXPLOSION] = Content.Load<SoundEffect>("Sounds/Explosion");
			audioClips[(int)AUDIOCLIPS.NUKE] = Content.Load<SoundEffect>("Nukes");

			
			pain.Add(Content.Load<SoundEffect>("Sounds/Pain1"));
			pain.Add(Content.Load<SoundEffect>("Sounds/Pain2"));
			pain.Add(Content.Load<SoundEffect>("Sounds/Pain3"));
		}

		public void SetMediaPlayerVolume(float newVolume)
		{
			MediaPlayer.Volume = newVolume;
		}

		public void StartBackgroundMusic()
		{
			MediaPlayer.Play(background1);
		}

		public void StopBackgroundMusic()
		{
			MediaPlayer.Stop();
		}

		public void PlayAudioClip(AUDIOCLIPS clipName)
		{
			audioClips[(int)clipName].Play();
		}

		public void PlayRandomPain()
		{
			Random tempRand = new Random();
			pain[tempRand.Next(pain.Count)].Play();
		}
	}
}
