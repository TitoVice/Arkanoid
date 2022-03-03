using Xamarin.Android;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;
using System;
using System.Collections.Generic;

namespace Game1
{
    public class Player
    {

        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }

        public SpriteFont font { get; set; }
        private Vector2 fontPosition = new Vector2(-1000, -1000);
        private int score;

        public Player(Vector2 pos)
        {
            velocity = Vector2.Zero;
            position = pos;
            score = 0;
        }
        public void Update(GameTime gameTime, Vector3 acceleration, Rectangle screen)
        {
            velocity *= 0.99f;  //fricció
            velocity += new Vector2(-acceleration.X, 0) * 25f;

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;  //posició del jugador

            Vector2 auxVel = velocity;
            Vector2 auxPos = position;

            // mirem si el jugador es surt de la pantalla, si ho fa fem que reboti al límit de la pantalla
            if (position.X >= screen.Width - texture.Width / 2)
            {
                auxPos.X = screen.Width - texture.Width / 2;
                auxVel.X *= -0.5f;

                position = auxPos;
                velocity = auxVel;
            }
            else if (position.X < texture.Width / 2)
            {
                auxPos.X = texture.Width / 2;
                auxVel.X *= -0.5f;

                position = auxPos;
                velocity = auxVel;
            }

            FontReposition();
        }

        public void changeScore(int points)
        {
            score += points;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0f,
                new Vector2(texture.Width / 2, texture.Height / 2),
                Vector2.One, SpriteEffects.None, 0f);

            spriteBatch.DrawString(font, score.ToString(), fontPosition, Color.White);
        }

        public void Restart(Vector2 pos, bool lost)
        {
            position = pos;
            if (lost) 
            { 
                score = 0;
                fontPosition = new Vector2(-1000, -1000);
            }
            else
            {
                FontReposition();
            }
        }

        private void FontReposition()
        {
            //Recoloca el text de la puntuació

            Vector2 sizeFont = font.MeasureString(score.ToString());

            fontPosition.X = position.X - sizeFont.X / 2;
            fontPosition.Y = position.Y + texture.Height / 2 + sizeFont.Y / 2;
        }

    }
}