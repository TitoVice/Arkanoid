using Xamarin.Android;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Devices.Sensors;
using System;


namespace Game1
{
    public class Enemy
    {
        public Texture2D enemyTexture1 { get; set; }
        public Texture2D enemyTexture2 { get; set; }
        public Texture2D enemyTexture3 { get; set; }
        
        public Vector2 enemyPosition { get; set; }

        private Texture2D actualTexture;
        private int level;
        private bool touch;
        private float pastTime;
        private float rest;

        public Enemy(Vector2 pos, int lev)//introduir el nivell per aqui
        {
            enemyPosition = pos;
            level = lev;
            touch = false;

            pastTime = 0.0f;
            rest = 0.1f;
        }

        public void getTextures(Texture2D texture1, Texture2D texture2, Texture2D texture3)
        {
            enemyTexture1 = texture1;
            enemyTexture2 = texture2;
            enemyTexture3 = texture3;

            ChangeTexture();
        }

        public int Touched(Texture2D textBall, Vector2 posBall, float seconds)
        {
            //Returns: 0(toca paret lateral i no es destrueix), 2(toca paret lateral i es destrueix), 1(toca paret vertical i no es destrueix), 3(toca paret vertical i es destrueix), 4(toca cantonada i no es destrueix), 5(toca cantonada i es destrueix)

            int sol = -1;

            if (pastTime >= rest)
            {
                touch = false;
                pastTime = 0.0f;
            }

            if (touch) { pastTime += seconds; return sol; } //comenco a contar quan ha estat tocat

            //deteccio parets laterals
            if ((posBall.Y + textBall.Height / 2 > enemyPosition.Y - actualTexture.Height / 2 && posBall.Y + textBall.Height / 2 < enemyPosition.Y + actualTexture.Height / 2) || (posBall.Y - textBall.Height / 2 > enemyPosition.Y - actualTexture.Height / 2 && posBall.Y - textBall.Height / 2 < enemyPosition.Y + actualTexture.Height / 2))
            {
                //paret dreta
                if (posBall.X - textBall.Width / 2 <= enemyPosition.X + actualTexture.Width / 2 && posBall.X + textBall.Width / 2 >= enemyPosition.X + actualTexture.Width / 2)
                {
                    level -= 1;
                    touch = true;

                    if (level <= 0) { sol = 2; }    // poso <= pel cas de que detecti colisio lateral i vertical alhora, en el qual es podria posar a -1 i no detectar que s'ha de destruir
                    else { sol = 0; ChangeTexture(); }
                }
                //paret esquerra
                else if (posBall.X + textBall.Width / 2 >= enemyPosition.X - actualTexture.Width / 2 && posBall.X - textBall.Width / 2 <= enemyPosition.X - actualTexture.Width / 2)
                {
                    level -= 1;
                    touch = true;

                    if (level <= 0) { sol = 2; }
                    else { sol = 0; ChangeTexture(); }
                }

            }

            //deteccio parets verticals
            if ( (posBall.X + textBall.Width / 2 > enemyPosition.X - actualTexture.Height / 2 && posBall.X + textBall.Width / 2 < enemyPosition.X + actualTexture.Width / 2) || (posBall.X - textBall.Width / 2 > enemyPosition.X - actualTexture.Width / 2 && posBall.X - textBall.Width / 2 < enemyPosition.X + actualTexture.Width / 2))
            {
                //paret inferior
                if (posBall.Y - textBall.Height / 2 <= enemyPosition.Y + actualTexture.Height / 2 && posBall.Y + textBall.Height / 2 >= enemyPosition.Y + actualTexture.Height / 2)
                {

                    if(sol == -1) { level -= 1; } //si ja ha estat tocat no li baixem un nivell
                    touch = true;

                    if (level <= 0) 
                    {
                        if (sol != -1) { sol = 5; }
                        else { sol = 3; } 
                    }
                    else 
                    {
                        if (sol != -1) { sol = 4; }
                        else { sol = 1; }
                        ChangeTexture(); 
                    }
                }
                //paret superior
                else if (posBall.Y + textBall.Height / 2 >= enemyPosition.Y - actualTexture.Height / 2 && posBall.Y - textBall.Height / 2 <= enemyPosition.Y - actualTexture.Height / 2)
                {
                    if (sol == -1) { level -= 1; }
                    touch = true;

                    if (level <= 0)
                    {
                        if (sol != -1) { sol = 5; }
                        else { sol = 3; }
                    }
                    else
                    {
                        if (sol != -1) { sol = 4; }
                        else { sol = 1; }
                        ChangeTexture();
                    }
                }
            }

            return sol;
        }

        
        private void ChangeTexture()
        {
            //canvia la textura del bloc

            switch (level)
            {
                case 1:
                    actualTexture = enemyTexture1;
                    break;
                case 2:
                    actualTexture = enemyTexture2;
                    break;
                case 3:
                    actualTexture = enemyTexture3;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(actualTexture, enemyPosition, null, Color.White, 0f,
                new Vector2(actualTexture.Width / 2, actualTexture.Height / 2),
                Vector2.One, SpriteEffects.None, 0f);
        }

    }
}