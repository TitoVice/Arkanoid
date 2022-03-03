using Xamarin.Android;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;
using System;
using System.Collections.Generic;

namespace Game1
{
    public class Ball
    {
        private Vector2 initialVel;
        private Vector2 initialPos;
        private Vector2 maxVel;

        private float plusVelocity = 1.05f;

        public Texture2D ballTexture { get; set; }
        public Vector2 ballPosition { get; set; }
        public Vector2 ballVelocity { get; set; }

        public Ball(Vector2 pos, Vector2 vel)
        {
            ballPosition = pos;
            ballVelocity = vel;
            initialPos = pos;
            initialVel = vel;
            maxVel = initialVel * 2;
        }

        public bool Update(GameTime gameTime, Texture2D playerTexture, Vector2 playerPosition, Vector2 playerVelocity, Rectangle screen)
        {
            bool alive = true;
            bool touched = false;

            ballPosition += ballVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;  //posició de la pilota
            Vector2 auxVel = ballVelocity;
            Vector2 auxPos = ballPosition;

            //xoca contra el jugador

            if ((ballPosition.Y + ballTexture.Height / 2 > playerPosition.Y - playerTexture.Height / 2 && ballPosition.Y + ballTexture.Height / 2 < playerPosition.Y + playerTexture.Height / 2) || (ballPosition.Y - ballTexture.Height / 2 > playerPosition.Y - playerTexture.Height / 2 && ballPosition.Y - ballTexture.Height / 2 < playerPosition.Y + playerTexture.Height / 2))
            {
                //paret dreta
                if (ballPosition.X - ballTexture.Width / 2 <= playerPosition.X + playerTexture.Width / 2 && ballPosition.X + ballTexture.Width / 2 >= playerPosition.X + playerTexture.Width / 2)
                {
                    auxVel.X *= -plusVelocity;
                    ballVelocity = auxVel;
                    touched = true;
                }
                //paret esquerra
                else if (ballPosition.X + ballTexture.Width / 2 >= playerPosition.X - playerTexture.Width / 2 && ballPosition.X - ballTexture.Width / 2 <= playerPosition.X - playerTexture.Width / 2)
                {
                    auxVel.X *= -plusVelocity;
                    ballVelocity = auxVel;
                    touched = true;
                }

            }
            //paret superior
            if (ballPosition.Y >= playerPosition.Y - (playerTexture.Height / 2 + ballTexture.Height / 2) && (ballPosition.X > playerPosition.X - (playerTexture.Width / 2 + ballTexture.Width / 2) && ballPosition.X < playerPosition.X + (playerTexture.Width / 2 + ballTexture.Width / 2)))
            {
                auxVel.Y = VelocityLimit(true);
                if (!touched) { auxVel.X = playerVelocity.X; } //si fos true, vol dir que toca una cantonada

                ballVelocity = auxVel;
            }
            
            //xoca contra les parets
            if (ballPosition.X >= screen.Width - ballTexture.Width / 2)
            {
                auxPos.X = screen.Width - ballTexture.Width / 2;
                auxVel.X *= -plusVelocity;

                ballPosition = auxPos;
                ballVelocity = auxVel;
            }
            else if (ballPosition.X < ballTexture.Width / 2)
            {
                auxPos.X = ballTexture.Width / 2;
                auxVel.X *= -plusVelocity;

                ballPosition = auxPos;
                ballVelocity = auxVel;
            }
            
            if (ballPosition.Y < ballTexture.Height / 2)
            {
                auxPos.Y = ballTexture.Height / 2;
                auxVel.Y = VelocityLimit(false);

                ballPosition = auxPos;
                ballVelocity = auxVel;
            }

            //es "destrueix" perque passa al jugador
            if (ballPosition.Y >= playerPosition.Y + playerTexture.Height/2)
            {
                alive = false;
                Restart(initialPos, initialVel);
            }

            return alive;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, 0f,
                new Vector2(ballTexture.Width / 2, ballTexture.Height / 2),
                Vector2.One, SpriteEffects.None, 0f);
        }

        public void Restart(Vector2 pos, Vector2 vel)
        {
            ballPosition = pos;
            ballVelocity = vel;
        }
        public float VelocityLimit(bool perDalt)
        {
            //limita la velocitat de la pilota a un maxim en l'eix de les Y, perDalt serveix per saber la direccio de la velocitat

            Vector2 auxVel = ballVelocity;

            if (Math.Abs(auxVel.Y * (-plusVelocity)) > Math.Abs(maxVel.Y))
            {
                auxVel.Y = maxVel.Y;
                if (perDalt) { auxVel.Y *= -1; }
            }
            else
            {
                auxVel.Y *= -plusVelocity;
            }
            return auxVel.Y;
        }

    }
}