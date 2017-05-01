using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AMON
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BaseClass startHere;

        int bombTimer = 0;

        public Animation explosion;

        public Texture2D bulletTexture, rocketTexture;
        public List<Bullet> bulletList, rocketList;

        bool bombPlayed = false;
        bool finalNukePlayed = false;

        public Video video1;
        public VideoPlayer player;
        public Texture2D videoTexture;

        public SoundEffect brilliant, speech, hateFalling, midway, pain1, pain2, pain3, pain4, pathetic, worseThanMySister, explosionSound, explosionSoud;
        public Song background1;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            startHere = new BaseClass();
            startHere.Initialise(this);

            explosion = new Animation(Content.Load<Texture2D>("Explosdi"), new Vector2(96, 32), 32, 32);

            player = new VideoPlayer();

            bulletList = new List<Bullet>();
            rocketList = new List<Bullet>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            video1 = Content.Load<Video>("Nuke");

            background1 = Content.Load<Song>("Super Street Fighter 2 - Guile's Stage");
            brilliant = Content.Load<SoundEffect>("Sounds/Brilliant");
            speech = Content.Load<SoundEffect>("Sounds/CrazySpeech");
            hateFalling = Content.Load<SoundEffect>("Sounds/HateFallingShout");
            midway = Content.Load<SoundEffect>("Sounds/HowLongUntilIWin");
            pain1 = Content.Load<SoundEffect>("Sounds/Pain");
            pain2 = Content.Load<SoundEffect>("Sounds/Pain1");
            pain3 = Content.Load<SoundEffect>("Sounds/Pain2");
            pain4 = Content.Load<SoundEffect>("Sounds/Pain3");
            pathetic = Content.Load<SoundEffect>("Sounds/Pathetic");
            worseThanMySister = Content.Load<SoundEffect>("Sounds/WorseThanMySister");
            explosionSound = Content.Load<SoundEffect>("Sounds/Explosion");
            explosionSoud = Content.Load<SoundEffect>("Nukes");

            startHere.planeImage[0] = Content.Load<Texture2D>("Images/plane");
            startHere.planeImage[1] = Content.Load<Texture2D>("Images/Plane flipped");

            startHere.powerUpImage = Content.Load<Texture2D>("Images/Shield");

            startHere.castleImage = Content.Load<Texture2D>("Castle");

            startHere.font1 = Content.Load<SpriteFont>("SpriteFont1");
            bulletTexture = Content.Load<Texture2D>("Images/Bomb");
            rocketTexture = Content.Load<Texture2D>("Images/Rocket");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            startHere.charFine = Content.Load<Texture2D>("Images/Parachute Midget");

            startHere.charNotFine = Content.Load<Texture2D>("Images/Parachute midget damaged");

            startHere.scrolling1Texture = Content.Load<Texture2D>("Images/Background11.fw");

            startHere.scrolling2Texture = Content.Load<Texture2D>("Images/Background12.fw");

            startHere.beginMessage = Content.Load<Texture2D>("Images/StartupMessage");

            startHere.failureMessage = Content.Load<Texture2D>("Images/FailureMessage");

            startHere.victoryMessage = Content.Load<Texture2D>("Images/VictoryMessage");

            startHere.cloud[0] = Content.Load<Texture2D>("Images/Cloud2");
            startHere.cloud[1] = Content.Load<Texture2D>("Images/Cloud2");
            startHere.cloud[2] = Content.Load<Texture2D>("Images/Cloud4");
            startHere.cloud[3] = Content.Load<Texture2D>("Images/Cloud4");
            
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if ((!startHere.won)&&(bombTimer!=0))
            {
                bombTimer = 0;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if(startHere.started)startHere.timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((!bombPlayed)&&(startHere.won))
            {
                player.IsLooped = true;
                player.Play(video1);
                bombPlayed = true;
            }

            explosion.Update(gameTime); 
            
            startHere.CheckAll();
            startHere.UpdateAll();

            if (bulletList.Count > 0)
            {
                for (int i = 0; i < bulletList.Count; i++)
                {
                    bulletList[i].BulletUpdate(5);
                }
            }

            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].bulletPos.Y > GraphicsDevice.Viewport.Height)
                {
                    bulletList.Remove(bulletList[i]);
                }
            }

            if (rocketList.Count > 0)
            {
                for (int i = 0; i < rocketList.Count; i++)
                {
                    rocketList[i].BulletUpdate(-1*(startHere.terminalVelocity+5));
                }
            }

            for (int i = 0; i < rocketList.Count; i++)
            {
                if (rocketList[i].bulletPos.Y < 0 - rocketList[i].bulletTexture.Height)
                {
                    rocketList.Remove(rocketList[i]);
                }
            }
            //most innefficient botched attempt at bug-fixing ever attempted by mankind.
            if (!startHere.won)
            {
                finalNukePlayed = false;
                bombPlayed = false;
            }


            //update enemies
            //check for end

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Rectangle screen = new Rectangle(GraphicsDevice.Viewport.X,
                GraphicsDevice.Viewport.Y,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (player.State != MediaState.Stopped)
                videoTexture = player.GetTexture();

            spriteBatch.Begin();

            //draw background
            spriteBatch.Draw(startHere.scrolling1Texture, startHere.scrolling1, Color.White);
            spriteBatch.Draw(startHere.scrolling2Texture, startHere.scrolling2, Color.White);

            spriteBatch.Draw(startHere.castleImage, startHere.CastleLocation, Color.White);

            //draw bullets
            if (bulletList.Count > 0)
            {
                for (int i = 0; i < bulletList.Count; i++)
                {
                    bulletList[i].Draw(spriteBatch, bulletTexture, Color.White);
                }
            }

            if (rocketList.Count > 0)
            {
                for (int i = 0; i < rocketList.Count; i++)
                {
                    rocketList[i].Draw(spriteBatch, rocketTexture, Color.White);
                }
            }
            //draw character
            spriteBatch.Draw(startHere.charImage, startHere.charLocation, startHere.charColor);

            spriteBatch.Draw(startHere.planeImage[startHere.planeSpriteUsed], startHere.planeLocation, Color.White);

            startHere.DrawClouds(spriteBatch);

            if (rocketList.Count > 0)
            {
                for (int i = 0; i < rocketList.Count; i++)
                {
                    rocketList[i].Draw(spriteBatch, rocketTexture, Color.Gray*0.2f);
                }
            }

            spriteBatch.Draw(startHere.powerUpImage, startHere.powerUpLocation, Color.White);

            startHere.DrawGrenadeCooldown(spriteBatch);
            if(startHere.explosionTimer>0)
            {
                explosion.Draw(spriteBatch);
                startHere.explosionTimer--;
            }
            //draw enemies


            //draw "titlescreen"
            if (!startHere.started) spriteBatch.Draw(startHere.beginMessage, new Rectangle(0, 0, 800, 480), Color.White);
            if (startHere.failed) spriteBatch.Draw(startHere.failureMessage, new Rectangle(0, 0, 800, 480), Color.White);

            if (startHere.won)
            {
                if ((videoTexture != null) && (bombTimer <= 320))
                {
                    spriteBatch.Draw(videoTexture, screen, Color.White);
                    if (!finalNukePlayed)
                    {
                        explosionSoud.Play();
                        finalNukePlayed = true;
                    }
                        bombTimer++;
                }

                if (bombTimer >= 300) spriteBatch.Draw(startHere.victoryMessage, new Rectangle(0, 0, 800, 480), Color.White);
            }

            spriteBatch.End();
            

            base.Draw(gameTime);
        }
    }
}
