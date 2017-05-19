using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AMON
{
    class BaseClass
    {
        public Rectangle charLocation, scrolling1, scrolling2, planeLocation, CastleLocation, powerUpLocation;
        public Texture2D charImage, charFine, charNotFine, scrolling1Texture, scrolling2Texture, beginMessage, failureMessage, victoryMessage, castleImage, powerUpImage;
        public Color charColor;
        public int terminalVelocity, charVertSpeed, charHorizSpeed, painNoise=1;
        public bool started, failed, won, midwayPlayed;
        //public SpriteFont font1;
        public int playInstance;
        public bool poweredUp;

        public int planeSpriteUsed;

        public Texture2D[] planeImage = new Texture2D[4];

        public float timeElapsed;

        public int cloudNumber = 2;
        int[] cloudTextureIndex = new int[9];
        Rectangle[] testCloud = new Rectangle[9];
        public Texture2D[] cloud = new Texture2D[4];
        

        int timer;
        int bulletTimer;
        public int enemyRocketTimer, painTimer, planeTimer, explosionTimer;

        Game1 gameClass;

        /*public void ResetGame()
        {
            playInstance++;
            planeTimer = 0;
            midwayPlayed = false;
            timeElapsed = 0;
            planeSpriteUsed = 0;
            planeLocation = new Rectangle(-148, 200, 148, 38);
            enemyRocketTimer = 100;
            charLocation = new Rectangle(388, 10, 38, 46);
            charColor = Color.White;
            terminalVelocity = 5;
            charVertSpeed = 2;
            charHorizSpeed = 6;
            scrolling1 = new Rectangle(0, 0, 800, 500);
            scrolling2 = new Rectangle(0, 480, 800, 500);
            started = false;
            failed = false;
            won = false;
            InitialiseClouds();
            for (int index = 0; index <= 3; index++)
            {
                cloudTextureIndex[index] = index;
            }
        }*/

        public void PlayPain()
        {
            switch (painNoise)
            {
                case 0:
                    //gameClass.pain1.Play();
                    break;
                case 1:
                    //gameClass.pain2.Play();
                    break;
                case 2:
                    //gameClass.pain3.Play();
                    break;
                case 3:
                   // gameClass.pain4.Play();
                    break;
            }
        }

        public void UpdatePainTimer()
        {
            if (painTimer > 0) painTimer--;
        }

        public void CyclePain()
        {
            switch (painNoise)
            {
                case 0:
                    painNoise = 1;
                    break;
                case 1:
                    painNoise = 2;
                    break;
                case 2:
                    painNoise = 3;
                    break;
                case 3:
                    painNoise = 4;
                    break;
                case 4:
                    painNoise = 1;
                    break;
            }
        }

        public void InitialiseClouds()
        {
            for (int index = 0; index <= cloudNumber-1; index++)
            {
                testCloud[index] = new Rectangle(0, index*10000, 0, 0);
            }
        }

        public void Initialise(Game1 game)
        {
            /*charImage = new Texture2D[2];
            charFine = new Texture2D[2];
            charNotFine = new Texture2D[2];
            scrolling1Texture = new Texture2D[2];
            scrolling2Texture = new Texture2D[2];
            beginMessage = new Texture2D[2];
            failureMessage = new Texture2D[2];
            victoryMessage = new Texture2D[2];*/


            timer = 0;
            poweredUp = false;
            powerUpLocation = new Rectangle(800,480,15,20);
            CastleLocation = new Rectangle(0, 480, 800, 480);
            playInstance = 0;
            planeTimer = 0;
            midwayPlayed = false;
            timeElapsed = 0;
            planeSpriteUsed = 0;
            planeLocation = new Rectangle(-148,200,148,38);
            enemyRocketTimer = 100;
            charLocation = new Rectangle(388,10,38,46);
            charColor = Color.White;
            terminalVelocity = 5;
            charVertSpeed = 3;
            charHorizSpeed = 7;
            scrolling1 = new Rectangle(0, 0, 800, 500);
            scrolling2 = new Rectangle(0, 480, 800, 500);
            started = false;
            failed = false;
            won = false;
            InitialiseClouds();
            gameClass = game;
            for (int index = 0; index <= 3; index++)
            {
                cloudTextureIndex[index] = index;
            }
        }

        public void CheckTime()
        {
            if ((timeElapsed > 60f)&&!failed)
            {
                won = true;
                MediaPlayer.Stop();
                //gameClass.speech.Play();
                timeElapsed = 0;
            }

            if ((timeElapsed >30f)&&(!midwayPlayed))
            {
                //gameClass.midway.Play();
                midwayPlayed = true;
            }
        }

        public void CheckAll()
        {
            CheckInput();

            if (!failed && !won)
            {
                CheckPowerUpCollision();
                CheckPlaneRocketCollision();
                CheckBombPlaneCollision();
                CheckTime();
                CheckWallCollision();
                CheckCloudCollision();
                CheckCharRocketCollision();
                CheckBombRocketCollision();
                CheckTimer();
                CheckPlaneCollision();
                CheckPlane();
            }
        }

        public void UpdateCastle()
        {
            if ((timeElapsed > 59)&&(CastleLocation.Y>10))
            {
                CastleLocation.Y -= terminalVelocity;
            }
            if (!(CastleLocation.Y > 10)) charLocation.Y += terminalVelocity;
        }

        public void UpdateAll()
        {
            if (started&&!failed&&!won)
            {
                UpdateCastle();
                UpdatePainTimer();
                UpdateBackground();
                UpdateCloud();
                if(timeElapsed>10f) UpdateEnemyRocket();
                UpdatePlane();
            }
            if (bulletTimer<=100) bulletTimer++;
        }

        public void UpdatePlane()
        {
            if(timeElapsed > 25)
            {
                if ((planeSpriteUsed == 0) && (planeTimer == 0)) planeLocation.X += 7;
                if ((planeSpriteUsed == 1) && (planeTimer == 0)) planeLocation.X -= 7;
            }
        }

        public void CheckPlane()
        {
            if (planeLocation.X > 800)
            {
                planeSpriteUsed = 1;
                planeLocation.Y = charLocation.Y;
                planeLocation.X = 800;
                if(timeElapsed<30)planeTimer = 150;
                else
                if(timeElapsed<45)planeTimer = 100;
                else planeTimer = 50;
            }
            if (planeLocation.X < -148)
            {
                planeSpriteUsed = 0;
                planeLocation.Y = charLocation.Y;
                planeLocation.X = -148;
                if (timeElapsed < 30) planeTimer = 150;
                else
                if(timeElapsed<45)planeTimer = 100;
                else planeTimer = 50;
            }

            if (planeTimer != 0) planeTimer--;
        }

        public void UpdateEnemyRocket()
        {
            if (enemyRocketTimer == 0)
            {
                
                if (timeElapsed<35)enemyRocketTimer = 100;
                else
                if (timeElapsed < 45) enemyRocketTimer = 80;
                else
                if (timeElapsed < 55) enemyRocketTimer = 60;
                else enemyRocketTimer = 40;
                LaunchEnemyRocket();
            }
            else enemyRocketTimer--;
        }

        public void LaunchEnemyRocket()
        {
            Bullet bulletInst = new Bullet();
            bulletInst.BulletCreate(new Rectangle(charLocation.X,500,8000,6000), gameClass.rocketTexture);
            gameClass.rocketList.Add(bulletInst);
            bulletInst = null;
        }

        public void CheckCharRocketCollision()
        {
            for (int i = 0; i < gameClass.rocketList.Count; i++)
                {
                    if (CheckCollision(charLocation, new Rectangle((int) gameClass.rocketList[i].bulletPos.X, (int) gameClass.rocketList[i].bulletPos.Y, gameClass.rocketList[i].bulletTexture.Width, gameClass.rocketList[i].bulletTexture.Height)))
                    {
                        if (poweredUp)
                        {
                            poweredUp = false;
                            //gameClass.worseThanMySister.Play();
                            //gameClass.explosionSound.Play();

                            gameClass.explosion.position = new Vector2(charLocation.X, charLocation.Y);

                            explosionTimer = 25;
                            gameClass.rocketList.Remove(gameClass.rocketList[i]);

                        }
                        else
                        {
                            failed = true;
                            MediaPlayer.Stop();
                            //gameClass.pathetic.Play();
                            timeElapsed = -100f;
                            //gameClass.explosionSound.Play();
                        }
                    }
                }            
        }

        public void CheckBombRocketCollision()
        {
            for (int j = 0; j < gameClass.bulletList.Count; j++)
            {
                for (int i = 0; i < gameClass.rocketList.Count; i++)
                {
                    if (CheckCollision(new Rectangle((int)gameClass.bulletList[j].bulletPos.X, (int)gameClass.bulletList[j].bulletPos.Y, gameClass.bulletList[j].bulletTexture.Width, gameClass.bulletList[j].bulletTexture.Height), new Rectangle((int)gameClass.rocketList[i].bulletPos.X, (int)gameClass.rocketList[i].bulletPos.Y, gameClass.rocketList[i].bulletTexture.Width, gameClass.rocketList[i].bulletTexture.Height)))
                    {
                        //gameClass.brilliant.Play();
                        //gameClass.explosionSound.Play();

                        gameClass.explosion.position = new Vector2(gameClass.bulletList[j].bulletPos.X, gameClass.bulletList[j].bulletPos.Y);

                        explosionTimer = 25;
                        gameClass.rocketList.Remove(gameClass.rocketList[i]);
                        gameClass.bulletList.Remove(gameClass.bulletList[j]);
                    }
                }
            }
        }

        public void CheckPlaneRocketCollision()
        {
            for (int i = 0; i < gameClass.rocketList.Count; i++)
            {
                if (CheckCollision(planeLocation, new Rectangle((int)gameClass.rocketList[i].bulletPos.X, (int)gameClass.rocketList[i].bulletPos.Y, gameClass.rocketList[i].bulletTexture.Width, gameClass.rocketList[i].bulletTexture.Height)))
                {
                    //gameClass.brilliant.Play();
                   // gameClass.explosionSound.Play();

                    gameClass.explosion.position = new Vector2(gameClass.rocketList[i].bulletPos.X, gameClass.rocketList[i].bulletPos.Y);
                    if (!poweredUp)
                    {
                        powerUpLocation.X = (int)gameClass.rocketList[i].bulletPos.X;
                        powerUpLocation.Y = (int)gameClass.rocketList[i].bulletPos.Y;
                    }


                    explosionTimer = 25;
                    gameClass.rocketList.Remove(gameClass.rocketList[i]);

                    if (planeSpriteUsed == 0) planeLocation.X = 800;
                    if (planeSpriteUsed == 1) planeLocation.X = 0 - planeLocation.Width;
                }
            }
        }

        public void CheckBombPlaneCollision()
        {
            for (int j = 0; j < gameClass.bulletList.Count; j++)
            {
                if (CheckCollision(new Rectangle((int)gameClass.bulletList[j].bulletPos.X, (int)gameClass.bulletList[j].bulletPos.Y, gameClass.bulletList[j].bulletTexture.Width, gameClass.bulletList[j].bulletTexture.Height),planeLocation))
                {
                    //gameClass.explosionSound.Play();

                    gameClass.explosion.position = new Vector2(gameClass.bulletList[j].bulletPos.X, gameClass.bulletList[j].bulletPos.Y);

                    explosionTimer = 25;
                    if (planeSpriteUsed == 0) planeLocation.X = 800;
                    if (planeSpriteUsed == 1) planeLocation.X = 0-planeLocation.Width;
                    gameClass.bulletList.Remove(gameClass.bulletList[j]);
                }
            }
        }

        public void CheckInput()
        {
            KeyboardState keybState = Keyboard.GetState();
            if (started && !failed)
            {
                if (keybState.IsKeyDown(Keys.Up)) charLocation.Y -= charVertSpeed;
                if (keybState.IsKeyDown(Keys.Down)) charLocation.Y += charVertSpeed;
                if (keybState.IsKeyDown(Keys.Left)) charLocation.X -= charHorizSpeed;
                if (keybState.IsKeyDown(Keys.Right)) charLocation.X += charHorizSpeed;
                if (keybState.IsKeyDown(Keys.Space) && bulletTimer > 100)
                {
                    bulletTimer = 0;
                    Bullet bulletInst = new Bullet();
                    bulletInst.BulletCreate(new Rectangle(charLocation.X+10,charLocation.Y+40,0,0), gameClass.bulletTexture);
                    gameClass.bulletList.Add(bulletInst);
                    bulletInst = null;
                }
            }
            else if ((keybState.IsKeyDown(Keys.Enter))&&!started)
            {
               // gameClass.hateFalling.Play();
                started = true;
                MediaPlayer.Volume = 5;
                MediaPlayer.Play(gameClass.background1);
            }

            if ((keybState.IsKeyDown(Keys.Back)) && ((failed)||(won))) gameClass.Exit();
            if ((keybState.IsKeyDown(Keys.Enter)) && ((failed) || (won))) Initialise(gameClass);
            
            //if ((keybState.IsKeyDown(Keys.Back)) && (won)) ResetGame();
        }

        public void CheckWallCollision()
        {
            if (charLocation.X < 0) charLocation.X = 0;
            if (charLocation.X + charLocation.Width > 800) charLocation.X = (800 - charLocation.Width);
            if (charLocation.Y < 0) charLocation.Y = 0;
            if (charLocation.Y + charLocation.Height > 480) charLocation.Y = (480 - charLocation.Height);
        }

        public bool CheckCollision(Rectangle input1, Rectangle input2)
        {
            if (
                ((((input1.X > input2.X) && (input1.X < input2.X + input2.Width)) && 
                ((input1.Y > input2.Y) && (input1.Y < input2.Y + input2.Height))) || 
                (((input1.X + input1.Width > input2.X) && (input1.X + input1.Width < input2.X + input2.Width)) && 
                ((input1.Y + input1.Height > input2.Y) && (input1.Y + input1.Height < input2.Y + input2.Height))))
                ||
                ((((input1.X + input1.Width > input2.X) && (input1.X < input2.X + input2.Width)) && 
                ((input1.Y + input1.Height > input2.Y) && (input1.Y < input2.Y + input2.Height))) || 
                (((input1.X > input2.X) && (input1.X + input1.Width < input2.X + input2.Width)) && 
                ((input1.Y > input2.Y) && (input1.Y + input1.Height < input2.Y + input2.Height))))
               )
                return true;
            return false;
        }

        public void CheckCloudCollision()
        {
            for (int index = 0; index <= cloudNumber - 1; index++)
            {
                if (CheckCollision(charLocation, testCloud[index]))
                {
                    if (!poweredUp)
                    {
                        if (timer == 0)
                        {
                            timer = 130;
                        }

                        if (timer <= 60) timer = 60;

                        if (painTimer == 0)
                        {
                            PlayPain();
                            CyclePain();
                            painTimer = 20;
                        }
                    }
                }
            }
        }

        public void CheckPlaneCollision()
        {
            if (CheckCollision(charLocation, planeLocation))
            {
                if (poweredUp)
                {
                    poweredUp = false;
                   // gameClass.worseThanMySister.Play();
                  //  gameClass.explosionSound.Play();

                    gameClass.explosion.position = new Vector2(charLocation.X, charLocation.Y);

                    explosionTimer = 25;
                    if (planeSpriteUsed == 0) planeLocation.X = 800;
                    if (planeSpriteUsed == 1) planeLocation.X = 0 - planeLocation.Width;

                }
                else
                {
                    failed = true;
                    MediaPlayer.Stop();
                   // gameClass.pathetic.Play();
                }
            }
        }

        public void CheckPowerUpCollision()
        {
            if (CheckCollision(powerUpLocation, charLocation))
            {
                poweredUp = true;
                charColor = Color.LightBlue * 0.8f;
                powerUpLocation.X = 800;
                powerUpLocation.Y = 480;
                timer = 0;
                charImage = charFine;
            }
        }

        public void RandomizeCloud(int index)
        {
            Random randX, randY, randHeight, randWidth, randImage;
            int baseSeed = (int)DateTime.Now.Ticks;
            
            randHeight = new Random(baseSeed + 3 * (index+1));
            randWidth = new Random(baseSeed + 4 * (index + 1));
            randX = new Random(baseSeed + (index + 1));
            randY = new Random(baseSeed + 2 * (index + 1));
            randImage = new Random(baseSeed*index);

            int newHeight = randHeight.Next(100,300);
            int newWidth = randWidth.Next(150, 500);
            int newX = randX.Next(0, 800-newWidth);
            int newY = randY.Next(480, 700);
            //cloudTextureIndex[index] = randImage.Next(0,3);
            cloudTextureIndex[index]++;
            if (cloudTextureIndex[index] > 3) cloudTextureIndex[index] -= 3;
            testCloud[index] = new Rectangle(newX,newY,newWidth,newHeight);
        }

        public void UpdateCloud()
        {
            for (int index = 0; index <= cloudNumber - 1; index++)
            {
                testCloud[index].Y -= terminalVelocity-2;
                if (testCloud[index].Y < 0-testCloud[index].Height)
                {
                    RandomizeCloud(index);
                }
            }
        }

        public void UpdateBackground()
        {
                scrolling1.Y -= terminalVelocity-3;
                scrolling2.Y -= terminalVelocity - 3;
                if (scrolling1.Y + scrolling1Texture.Height <= 0)
                    scrolling1.Y = scrolling2.Y + scrolling2Texture.Height;
                if (scrolling2.Y + scrolling2Texture.Height <= 0)
                    scrolling2.Y = scrolling1.Y + scrolling1Texture.Height;
        }

        public void CheckTimer()
        {
            if (timer > 0)
            {
                charImage = charNotFine;
                charColor = Color.Aquamarine;
                timer--;
                charLocation.Height = 50;
                charLocation.Width = 26;
                charVertSpeed = 1;
                charHorizSpeed = 4;
                terminalVelocity = 9;
            }
            else
            {
                if (!poweredUp) charColor = Color.White;
                charImage = charFine;
                charLocation.Height = 46;
                charLocation.Width = 38;
                charVertSpeed = 3;
                charHorizSpeed = 7;
                terminalVelocity = 7;
            }    
        }

        public void DrawClouds(SpriteBatch spriteBatch)
        {
            for (int index = 0; index <= cloudNumber - 1; index++)
            {
                spriteBatch.Draw(cloud[cloudTextureIndex[index]], testCloud[index], Color.White);
            }
        }

        public void DrawGrenadeCooldown(SpriteBatch spriteBatch)
        {
            //if((bulletTimer<100)&&(bulletTimer>0))
                //spriteBatch.DrawString(font1,"Wait for bomb: "+Convert.ToString(100-bulletTimer),new Vector2(10,10),Color.White);
            //else
                //spriteBatch.DrawString(font1, "Bomb Ready!", new Vector2(10, 10), Color.White);

            //spriteBatch.DrawString(font1, "Time till impact:" + Convert.ToString((int)(60 - timeElapsed)), new Vector2(550, 10), Color.Red);
        }
    }
}
