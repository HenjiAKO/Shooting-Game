using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace ShootingGalley
{
    public enum GameState
    { 
        MainMenu,
        DifficultySelect,
        Playing,
        GameOver
    }
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Nightmare
    }



    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentGameState = GameState.MainMenu;
        private MouseState previousMouse;
        private Texture2D crosshairTexture;
        private Vector2 mousePosition;
        private Difficulty currentDifficulty;
        private Random random = new Random();




        Texture2D targetSprite;
        Texture2D crosshairSprite;
        Texture2D backgroundSprite;
        SpriteFont gameFont;
        Texture2D playButton;
        Texture2D quitButtom;
        Texture2D easyButton;
        Texture2D normalButton;
        Texture2D hardButton;
        Texture2D nightmareButton;
        Texture2D backButton;

        Vector2 targetPosition = new Vector2(300, 300);
        const int targetRadius = 45;

        MouseState mState;
        bool mRelease = true;
        int score = 0;

        double targetTimer;
        double targetSpawnTime;


        private void StartGame(Difficulty difficulty)
        {
            currentDifficulty = difficulty;
            score = 0;
            targetPosition = new Vector2(300, 300);

            switch (difficulty)
            {
                case Difficulty.Easy:
                    targetSpawnTime = 3.0;
                    break;
                case Difficulty.Normal:
                    targetSpawnTime = 2.0;
                    break;
                case Difficulty.Hard:
                    targetSpawnTime = 0.9;
                    break;
                case Difficulty.Nightmare:
                    targetSpawnTime = 0.5;
                    break;
            }

            targetTimer = targetSpawnTime;
            currentGameState = GameState.Playing;
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = false;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            targetSprite = Content.Load<Texture2D>("target");
            crosshairSprite = Content.Load<Texture2D>("crosshairs");
            backgroundSprite = Content.Load<Texture2D>("sky");
            gameFont = Content.Load<SpriteFont>("galleryFont");
            playButton = Content.Load<Texture2D>("button_play");
            quitButtom = Content.Load<Texture2D>("button_quit");
            easyButton = Content.Load<Texture2D>("button_easy");
            normalButton = Content.Load<Texture2D>("button_normal");
            hardButton = Content.Load<Texture2D>("button_hard");
            nightmareButton = Content.Load<Texture2D>("button_nightmare");
            backButton = Content.Load<Texture2D>("button_back");


        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            mousePosition = new Vector2(mouse.X, mouse.Y);


            if (currentGameState == GameState.MainMenu)
            {
                Rectangle playButtonRect = new Rectangle(300, 200, playButton.Width, playButton.Height);
                Rectangle quitButtonRect = new Rectangle(300, 300, quitButtom.Width, quitButtom.Height);

                if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
                {
                    if (playButtonRect.Contains(mousePosition))
                        currentGameState = GameState.DifficultySelect;

                    if (quitButtonRect.Contains(mousePosition))
                        Exit();
                }
            }
            else if (currentGameState == GameState.DifficultySelect)
            {
                Rectangle easyRect = new Rectangle(200, 200, easyButton.Width, easyButton.Height);
                Rectangle normalRect = new Rectangle(400, 200, normalButton.Width, normalButton.Height);
                Rectangle hardRect = new Rectangle(200, 300, hardButton.Width, hardButton.Height);
                Rectangle nightmareRect = new Rectangle(400, 300, nightmareButton.Width, nightmareButton.Height);
                Rectangle backRect = new Rectangle(20, 20, backButton.Width, backButton.Height);

                if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
                {
                    if (easyRect.Contains(mousePosition))
                        StartGame(Difficulty.Easy);

                    if (normalRect.Contains(mousePosition))
                        StartGame(Difficulty.Normal);

                    if (hardRect.Contains(mousePosition))
                        StartGame(Difficulty.Hard);

                    if (nightmareRect.Contains(mousePosition))
                        StartGame(Difficulty.Nightmare);

                    if (backRect.Contains(mousePosition))
                        currentGameState = GameState.MainMenu;
                }
            }
            else if (currentGameState == GameState.Playing)
            {
                targetTimer -= gameTime.ElapsedGameTime.TotalSeconds;

                if (targetTimer <= 0)
                { 
                    targetPosition = new Vector2(
                        random.Next(targetRadius, _graphics.PreferredBackBufferWidth - targetRadius),
                        random.Next(targetRadius, _graphics.PreferredBackBufferHeight - targetRadius)
                    );
                    targetTimer = targetSpawnTime;
                }


                // Handle shooting
                if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
                {
                    float dx = mouse.X - targetPosition.X;
                    float dy = mouse.Y - targetPosition.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (distance <= targetRadius) // hit detected
                    {
                        score++;

                        targetPosition = new Vector2(
                            random.Next(targetRadius, _graphics.PreferredBackBufferWidth - targetRadius),
                            random.Next(targetRadius, _graphics.PreferredBackBufferHeight - targetRadius)
                         );
                        targetTimer = targetSpawnTime; // reset timer
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    currentGameState = GameState.DifficultySelect;
            }
            // save for next frame
            previousMouse = mouse;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            float buttonScale = 0.5f;
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    _spriteBatch.Draw(backgroundSprite, GraphicsDevice.Viewport.Bounds, Color.White);
                    _spriteBatch.Draw(playButton, new Vector2(300, 200), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
                    _spriteBatch.Draw(quitButtom, new Vector2(300, 300), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
                    break;

                case GameState.DifficultySelect:
                    _spriteBatch.Draw(backgroundSprite, GraphicsDevice.Viewport.Bounds, Color.White);
                    _spriteBatch.Draw(easyButton, new Vector2(200, 200), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
                    _spriteBatch.Draw(normalButton, new Vector2(400, 200), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
                    _spriteBatch.Draw(hardButton, new Vector2(200, 300), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
                    _spriteBatch.Draw(nightmareButton, new Vector2(400, 300), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
                    _spriteBatch.Draw(backButton, new Vector2(20, 20), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
                    break;


                case GameState.Playing:
                    _spriteBatch.DrawString(gameFont, "Score: " + score, new Vector2(20, 20), Color.White);

                    // draw target at its position
                    _spriteBatch.Draw(targetSprite, targetPosition - new Vector2(targetSprite.Width / 2, targetSprite.Height / 2), Color.White);
                    break;

            }

            // Always draw crosshair last
            // Always draw crosshair last
            _spriteBatch.Draw(crosshairSprite, mousePosition - new Vector2(crosshairSprite.Width / 2, crosshairSprite.Height / 2), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
