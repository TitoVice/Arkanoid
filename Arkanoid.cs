using Xamarin.Android;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;
using System;
using System.Collections.Generic;

namespace Game1
{
    public class Arkanoid : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private bool gameStarted = false;

        //accelerometre
        Accelerometer acc = new Accelerometer();
        Vector3 acceleration;

        //jugador
        Texture2D playerTexture;
        Vector2 playerPosition;
        Player player;

        //bola
        Texture2D ballTexture;
        Vector2 initialBallPos;
        Vector2 initialBallVel;
        Ball ball;

        //textures enemics/blocs
        Texture2D enemy1Texture;
        Texture2D enemy2Texture;
        Texture2D enemy3Texture;

        //enemics/blocs
        List<Enemy> enemies;
        int enemyNumber;

        //touch screen
        TouchCollection touchCollection;

        //font
        SpriteFont font;

        //vida
        int Lifes;
        int startingLifes = 3;
        Vector2 lifePosition;

        public Arkanoid()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            playerPosition = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height - (GraphicsDevice.Viewport.Bounds.Height / 5)); //posició jugador
            player = new Player(playerPosition);

            initialBallPos = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);
            initialBallVel = new Vector2(0.0f, 500.0f);
            ball = new Ball(initialBallPos, initialBallVel);

            enemyNumber = 0;

            Lifes = startingLifes;

            acc.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("jugador");
            ballTexture = Content.Load<Texture2D>("pilota2");
            enemy1Texture = Content.Load<Texture2D>("enemic1");
            enemy2Texture = Content.Load<Texture2D>("enemic2");
            enemy3Texture = Content.Load<Texture2D>("enemic3");
            font = Content.Load<SpriteFont>("Score");
            
            player.font = font;
            player.texture = playerTexture;

            ball.ballTexture = ballTexture;

            lifePosition = new Vector2(GraphicsDevice.Viewport.Bounds.Width - ballTexture.Width, GraphicsDevice.Viewport.Bounds.Height - ballTexture.Height);

            CreateBlocks();
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameStarted)
            {
                //mirar si la pilota xoca amb algun bloc
                for (int i = 0; i < enemies.Count; i++)
                {
                    int sol = enemies[i].Touched(ball.ballTexture, ball.ballPosition, (float)gameTime.ElapsedGameTime.TotalSeconds);

                    ColisioBola(sol, i);
                    
                    //només es mira la colisio amb un bloc per frame (evita bugs)
                    if (sol != -1) { break; }
                }

                //s'acaba el joc
                if(enemies.Count == 0)
                {
                    Restart(false);
                    return;
                }

                acc.CurrentValueChanged += (sender, args) =>
                {
                    acceleration = 0.8f * args.SensorReading.Acceleration + 0.2f * acceleration; //agafem el valor de la inclinació del mòbil
                };

                player.Update(gameTime, acceleration, GraphicsDevice.Viewport.Bounds);

                //mira xocs de la pilota amb el jugador i les parets
                if(!ball.Update(gameTime, player.texture, player.position, player.velocity, GraphicsDevice.Viewport.Bounds))
                {
                    //si ha passat al juagdor (ha mort)
                    Lifes -= 1;
                    if (Lifes == 0)
                    {
                        Restart(true);
                        return;
                    }
                }

            }
            else
            {
                //es mira si s'ha tocat la pantalla, de ser així el joc comença
                touchCollection = TouchPanel.GetState();

                foreach (var touch in touchCollection)
                {
                    if (touch.State == TouchLocationState.Pressed)
                    {
                        gameStarted = true;
                        break;
                    }
                }
            }
            
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            //sprite jugador
            player.Draw(_spriteBatch);

            //sprite bola
            ball.Draw(_spriteBatch);

            //sprites blocs
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(_spriteBatch);
            }

            //vides
            for (int j = 0; j < Lifes; j++)
            {
                Vector2 auxPosLife = lifePosition;
                auxPosLife.X -= (j * ballTexture.Width);

                _spriteBatch.Draw(ballTexture, auxPosLife, null, Color.White, 0f,
                    new Vector2(ballTexture.Width / 2, ballTexture.Height / 2),
                    Vector2.One, SpriteEffects.None, 0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private void Restart(bool lost)
        {
            gameStarted = false;

            player.Restart(playerPosition, lost);

            ball.Restart(initialBallPos, initialBallVel);

            Lifes = startingLifes;

            enemies.Clear();
            enemyNumber = 0;
            CreateBlocks();

            for (int i = 0; i < enemyNumber; i++)
            {
                enemies[i].getTextures(enemy1Texture, enemy2Texture, enemy3Texture);
            }
        }
        private void CreateBlocks()
        {
            //creacio blocs
            enemies = new List<Enemy>();

            Random r = new Random();
            int xEnemy = (GraphicsDevice.Viewport.Bounds.Width / enemy1Texture.Width); //per adaptar-se a la mida de la pantalla
            int yEnemy = 6;

            for (int i = 0; i < yEnemy; i++)
            {
                int x = r.Next(1, xEnemy);

                int pos = GraphicsDevice.Viewport.Bounds.Width / (x + 1);

                for (int j = 0; j < x; j++)
                {
                    Enemy en = new Enemy( new Vector2( pos * (j + 1), (enemy1Texture.Height*2 * (i + 1) ) + GraphicsDevice.Viewport.Bounds.Height / 15), (Math.Abs(i-yEnemy+1)/2)+1 );
                    en.getTextures(enemy1Texture, enemy2Texture, enemy3Texture);
                    enemies.Add(en);
                    enemyNumber += 1;
                }
            }
        }

        private void ColisioBola(int sol, int i)
        {
            //es fan els canvis de direccio en la bola si colisiona amb un bloc

            Vector2 aux = ball.ballVelocity;
            bool perDalt = false;
            if (ball.ballVelocity.Y > 0) { perDalt = true; }

            switch (sol)
            {   //toca dreta o esquerra i no es destrueix
                case 0:
                    aux.X = ball.ballVelocity.X * -1;
                    ball.ballVelocity = aux;
                    player.changeScore(5);
                    break;
                //toca dalt o baix i no es destrueix
                case 1:
                    aux.Y = ball.VelocityLimit(perDalt);
                    ball.ballVelocity = aux;
                    player.changeScore(5);
                    break;
                //toca dreta o esquerra i es destrueix
                case 2:
                    aux.X = ball.ballVelocity.X * -1;
                    ball.ballVelocity = aux;
                    enemies.RemoveAt(i);
                    player.changeScore(10);
                    break;
                //toca dalt o baix i es destrueix
                case 3:
                    aux.Y = ball.VelocityLimit(perDalt);
                    ball.ballVelocity = aux;
                    enemies.RemoveAt(i);
                    player.changeScore(10);
                    break;
                //toca cantonada i no es destrueix
                case 4:
                    aux.Y = ball.VelocityLimit(perDalt);
                    aux.X = ball.ballVelocity.X * -1;
                    ball.ballVelocity = aux;
                    player.changeScore(5);
                    break;
                //toca cantonada i es destrueix
                case 5:
                    aux.Y = ball.VelocityLimit(perDalt);
                    aux.X = ball.ballVelocity.X * -1;
                    ball.ballVelocity = aux;
                    enemies.RemoveAt(i);
                    player.changeScore(5);
                    break;
            }
        }

    }
}
