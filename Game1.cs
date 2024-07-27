using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonogameTetrisClient;

internal struct State {
    public bool Left;
    public bool Right;
    public bool Down;
    public bool Drop;
    public bool RotateCw;
    public bool RotateCcw;
    public bool Hold;
}

public class Game1 : Game {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Tetrion _tetrion = null!;
    private Mutex _tetrionMutex = new();
    private State _state = new();
    private Mutex _stateMutex = new();
    private bool _disposed = false;
    private Texture2D _minoTexture = null!;
    private Thread _simulationThread = null!;
    private CancellationTokenSource _cancellationTokenSource = new();

    private static readonly Dictionary<TetrominoType, Color> Colors = new()
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

        _simulationThread = new Thread(KeepSimulating);
        _simulationThread.IsBackground = true;
        _simulationThread.Start();

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _minoTexture = Content.Load<Texture2D>("mino02");
    }

    private void KeepSimulating() {
        ulong nextTick = 0;
        var stopwatch = Stopwatch.StartNew();
        while (!_cancellationTokenSource.Token.IsCancellationRequested) {
            var elapsedTime = stopwatch.Elapsed.TotalSeconds;
            var simulationTick = (ulong)(elapsedTime / (1.0 / 60.0));

            if (nextTick >= simulationTick) {
                Thread.Sleep(8);
                continue;
            }

            _stateMutex.WaitOne();
            var state = _state;
            _stateMutex.ReleaseMutex();

            _tetrionMutex.WaitOne();
            while (true) {
                nextTick = _tetrion.GetNextFrame();
                if (nextTick >= simulationTick) {
                    break;
                }

                _tetrion.SimulateNextFrame(new KeyState(
                    Left: state.Left,
                    Right: state.Right,
                    Down: state.Down,
                    Drop: state.Drop,
                    RotateCw: state.RotateCw,
                    RotateCcw: state.RotateCcw,
                    Hold: state.Hold
                ));
            }

            _tetrionMutex.ReleaseMutex();
            Thread.Sleep(8);
        }
    }

    protected override void Update(GameTime gameTime) {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        ) {
            _cancellationTokenSource.Cancel();
            _simulationThread.Join();
            Exit();
        }

        var left = Keyboard.GetState().IsKeyDown(Keys.A);
        var right = Keyboard.GetState().IsKeyDown(Keys.D);
        var down = Keyboard.GetState().IsKeyDown(Keys.S);
        var drop = Keyboard.GetState().IsKeyDown(Keys.W);
        var rotateCw = Keyboard.GetState().IsKeyDown(Keys.Right);
        var rotateCcw = Keyboard.GetState().IsKeyDown(Keys.Left);
        var hold = Keyboard.GetState().IsKeyDown(Keys.E);

        _stateMutex.WaitOne();
        _state = new State
        {
            Left = left,
            Right = right,
            Down = down,
            Drop = drop,
            RotateCw = rotateCw,
            RotateCcw = rotateCcw,
            Hold = hold,
        };
        _stateMutex.ReleaseMutex();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        _tetrionMutex.WaitOne();
        var matrix = _tetrion.GetMatrix();
        var activeTetromino = _tetrion.TryGetActiveTetromino();
        _tetrionMutex.ReleaseMutex();

        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        for (var x = 0; x < _tetrion.Width; x++) {
            for (var y = 0; y < _tetrion.Height; y++) {
                if (matrix[x, y] != TetrominoType.Empty) {
                    _spriteBatch.Draw(
                        _minoTexture,
                        new Vector2(x * _minoTexture.Width, y * _minoTexture.Height),
                        null,
                        Colors[matrix[x, y]],
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }

        if (activeTetromino is not null) {
            foreach (var mino in activeTetromino.Value.MinoPositions) {
                _spriteBatch.Draw(
                    _minoTexture,
                    new Vector2(mino.X * _minoTexture.Width, mino.Y * _minoTexture.Height),
                    null,
                    Colors[activeTetromino.Value.Type],
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
