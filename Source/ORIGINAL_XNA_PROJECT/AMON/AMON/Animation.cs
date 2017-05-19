using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AMON
{
    public class Animation
    {
        Texture2D texture;
        Rectangle rectangle;
        public Vector2 position;
        public Vector2 origin;
        Vector2 velocity;

        int currentFrame;
        int frameHeight;
        int frameWidth;

        float timer;
        float interval = 100;
        
        public Animation(Texture2D newTexture, Vector2 newPosition, int newFrameHeight, int newFrameWidth)
        {
            texture = newTexture;
            position = newPosition;
            frameHeight = newFrameHeight;
            frameWidth = newFrameWidth;
        }

        public void Update(GameTime gameTime)
        {
            rectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            //position = position + velocity;
            //position = new Vector2(100, 100);
            
            AnimateRight(gameTime);
            velocity.X = 3;

        }
        public void AnimateRight(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
            if (timer > interval)
            {
                currentFrame++;
                timer = 0;
                if (currentFrame > 2)
                    currentFrame = 0;
            }
        }

     

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, origin, 1.0f, SpriteEffects.None, 0); 
        }
    }
}
