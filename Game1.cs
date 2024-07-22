using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonogameTetrisClient;

public class Game1 : Game {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Tetrion _tetrion = null!;
    private bool _disposed = false;
    private double _elapsedTime = 0.0;

    public Game1() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        _tetrion = new Tetrion((ulong)Random.Shared.Next());

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }

        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

        var simulationTick = (ulong)(_elapsedTime / (1.0 / 60.0));

        while (_tetrion.GetNextFrame() < simulationTick) {
            _tetrion.SimulateNextFrame(new KeyState(
                Left: Keyboard.GetState().IsKeyDown(Keys.A),
                Right: Keyboard.GetState().IsKeyDown(Keys.D),
                Down: Keyboard.GetState().IsKeyDown(Keys.S),
                Drop: Keyboard.GetState().IsKeyDown(Keys.W),
                RotateCw: Keyboard.GetState().IsKeyDown(Keys.Right),
                RotateCcw: Keyboard.GetState().IsKeyDown(Keys.Left),
                Hold: Keyboard.GetState().IsKeyDown(Keys.E)
            ));
        }

        var activeTetromino = _tetrion.TryGetActiveTetromino();
        if (activeTetromino is not null) {
            Console.WriteLine(
                $"Active tetromino: {activeTetromino.Value.Type} @ ({activeTetromino.Value.MinoPositions[0].X}, {activeTetromino.Value.MinoPositions[0].Y})"
            );
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        if (disposing) {
            _graphics.Dispose();
            _spriteBatch.Dispose();
            _tetrion.Dispose();
        }

        _disposed = true;

        base.Dispose(disposing);
    }
}
