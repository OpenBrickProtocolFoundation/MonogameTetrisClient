using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameTetrisClient;

public class ObpfGame : Game {
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private readonly SceneStack _sceneStack = new();
    private Assets _assets = null!;
    private readonly string? _server;
    private readonly ushort? _port;

    public ObpfGame(string server, ushort port) : this() {
        _server = server;
        _port = port;
    }

    public ObpfGame() {
        InactiveSleepTime = new TimeSpan(0);
        _graphics = new GraphicsDeviceManager(this);
        _graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 704 + 3 * 10 * 32;
        _graphics.PreferredBackBufferHeight = 640;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _assets = new Assets(GraphicsDevice, Content);

        // LoadContent() is called after Initialize (WTF?!), so we have to create the initial scene here
        _sceneStack.PushScene(new Scenes.SingleplayerScene(_assets, _server, _port));

        base.LoadContent();
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
