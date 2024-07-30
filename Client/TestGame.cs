using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameTetrisClient;

public class TestGame : Game {
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private readonly SceneStack _sceneStack = new();
    private Assets _assets = null!;

    public TestGame() {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        var minoTexture = Content.Load<Texture2D>("mino02");
        var tetrionTexture = Content.Load<Texture2D>("tetrion");
        var whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
        whiteTexture.SetData(new[] { Color.White });
        var font = Content.Load<SpriteFont>("font");
        _assets = new Assets(minoTexture, tetrionTexture, whiteTexture, font);

        base.LoadContent();
    }

    protected override void Initialize() {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 704;
        _graphics.PreferredBackBufferHeight = 640;
        _graphics.ApplyChanges();

        _sceneStack.PushScene(new Scenes.SingleplayerScene());

        base.Initialize();
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _sceneStack.Draw(gameTime, _spriteBatch, _assets);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    protected override void Update(GameTime gameTime) {
        _sceneStack.Update(gameTime, _assets);
        if (_sceneStack.IsEmpty) {
            Exit();
        }

        base.Update(gameTime);
    }
}
