using System;
using System.Collections.Generic;
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
    private Texture2D _minoTexture = null!;
    private Tetromino? _activeTetromino = null;

    private static Dictionary<TetrominoType, Color> _colors = new Dictionary<TetrominoType, Color>
    {
        { TetrominoType.Empty, Color.Black },
        { TetrominoType.I, new Color(0, 240, 240) },
        { TetrominoType.J, new Color(0, 0, 240) },
        { TetrominoType.L, new Color(240, 160, 0) },
        { TetrominoType.O, new Color(240, 240, 0) },
        { TetrominoType.S, new Color(0, 240, 0) },
        { TetrominoType.T, new Color(160, 0, 240) },
        { TetrominoType.Z, new Color(240, 0, 0) },
    };

    public Game1() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 900;
        _graphics.ApplyChanges();

        _tetrion = new Tetrion((ulong)Random.Shared.Next());

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _minoTexture = Content.Load<Texture2D>("mino02");
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

        _activeTetromino = _tetrion.TryGetActiveTetromino();
        if (_activeTetromino is not null) {
            Console.WriteLine(
                $"Active tetromino: {_activeTetromino.Value.Type} @ ({_activeTetromino.Value.MinoPositions[0].X}, {_activeTetromino.Value.MinoPositions[0].Y})"
            );
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        var matrix = _tetrion.GetMatrix();
        for (var x = 0; x < _tetrion.Width; x++) {
            for (var y = 0; y < _tetrion.Height; y++) {
                if (matrix[x, y] != TetrominoType.Empty) {
                    _spriteBatch.Draw(
                        _minoTexture,
                        new Vector2(x * _minoTexture.Width, y * _minoTexture.Height),
                        null,
                        _colors[matrix[x, y]],
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }

        if (_activeTetromino is not null) {
            foreach (var mino in _activeTetromino.Value.MinoPositions) {
                _spriteBatch.Draw(
                    _minoTexture,
                    new Vector2(mino.X * _minoTexture.Width, mino.Y * _minoTexture.Height),
                    null,
                    _colors[_activeTetromino.Value.Type],
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        _spriteBatch.End();

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
