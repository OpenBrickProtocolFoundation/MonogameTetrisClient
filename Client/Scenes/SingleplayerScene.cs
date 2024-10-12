using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Obpf.Api;
using Action = Obpf.Api.Action;
using KeyState = Obpf.Api.KeyState;

namespace MonogameTetrisClient.Scenes;

internal struct State {
    public bool Left;
    public bool Right;
    public bool Down;
    public bool Drop;
    public bool RotateCw;
    public bool RotateCcw;
    public bool Hold;
}

public sealed class SingleplayerScene : Scene, IDisposable {
    private Tetrion _tetrion = null!;
    private Mutex _tetrionMutex = new();
    private State _state = new();
    private Mutex _stateMutex = new();
    private Thread _simulationThread = null!;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private double _elapsedTime = 0.0;
    private double _lastFpsMeasureTime = 0.0;
    private uint _framesSinceLastFpsMeasure = 0u;
    private string _fpsString = "-";
    private const int AllClearDuration = 15;
    private Synchronized<int> _allClearCountdown = new(0);

    public SingleplayerScene(Assets assets) : base(assets) { }

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

    private static readonly Dictionary<TetrominoType, Color> GhostColors = new()
    {
        { TetrominoType.Empty, Color.Black },
        { TetrominoType.I, new Color(0, 80, 80) },
        { TetrominoType.J, new Color(0, 0, 80) },
        { TetrominoType.L, new Color(80, 50, 0) },
        { TetrominoType.O, new Color(80, 80, 0) },
        { TetrominoType.S, new Color(0, 80, 0) },
        { TetrominoType.T, new Color(50, 0, 80) },
        { TetrominoType.Z, new Color(80, 0, 0) },
    };

    public override void Initialize() {
        // _tetrion = new Tetrion((ulong)Random.Shared.Next());
        _tetrion = new Tetrion("127.0.0.1", 12345);
        _tetrion.SetActionHandler(HandleAction);
        _simulationThread = new Thread(KeepSimulating)
        {
            IsBackground = true,
        };
        _simulationThread.Start();
    }

    private void HandleAction(Obpf.Api.Action action) {
        switch (action) {
            case Action.RotateCw:
            case Action.RotateCcw:
                break;
            case Action.HardDrop:
                Assets.SwiffSound.Play();
                break;
            case Action.Touch:
                Assets.ClickSound.Play();
                break;
            case Action.Clear1:
                Assets.Clear1Sound.Play();
                break;
            case Action.Clear2:
                Assets.Clear2Sound.Play();
                break;
            case Action.Clear3:
                Assets.Clear3Sound.Play();
                break;
            case Action.Clear4:
                Assets.Clear4Sound.Play();
                break;
            case Action.AllClear:
                _allClearCountdown.Access((ref int value) => value = AllClearDuration);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }

    public override UpdateResult Update(GameTime gameTime, ISceneManager sceneManager) {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        ) {
            _cancellationTokenSource.Cancel();
            _simulationThread.Join();
            sceneManager.PopCurrentScene();
            return UpdateResult.KeepUpdating;
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

        return UpdateResult.KeepUpdating;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        _tetrionMutex.WaitOne();
        var matrix = _tetrion.GetMatrix();
        var activeTetromino = _tetrion.TryGetActiveTetromino();
        var ghostTetromino = _tetrion.TryGetGhostTetromino();
        var holdPieceType = _tetrion.GetHoldPiece();
        var lineClearDelayState = _tetrion.GetLineClearDelayState();
        var previewPieces = _tetrion.GetPreviewPieces();
        var stats = _tetrion.GetStats();
        var isGameOver = _tetrion.IsGameOver();
        var nextFrame = _tetrion.GetNextFrame();
        var framesUntilGameStart = _tetrion.GetFramesUntilGameStart();

        var observerMatrices = new List<TetrominoType[,]>();
        foreach (var observer in _tetrion.Observers.Observers) {
            observerMatrices.Add(observer.GetMatrix());
        }
        _tetrionMutex.ReleaseMutex();

        DrawTetrionBackground(spriteBatch);

        if (holdPieceType != TetrominoType.Empty) {
            var holdPiecePositions = Tetrion.GetMinoPositions(holdPieceType, Rotation.North);
            foreach (var position in holdPiecePositions) {
                DrawMino(position + new Vec2(1, 2), Colors[holdPieceType], spriteBatch);
            }
        }

        for (int i = 0; i < previewPieces.Length; ++i) {
            var previewPiecePositions = Tetrion.GetMinoPositions(previewPieces[i], Rotation.North);
            foreach (var position in previewPiecePositions) {
                DrawMino(position + new Vec2(17, 2 + i * 3), Colors[previewPieces[i]], spriteBatch);
            }
        }

        var drawOffset = new Vec2(6, -_tetrion.NumInvisibleLines);

        for (var x = 0; x < _tetrion.Width; x++) {
            for (var y = 0; y < _tetrion.Height; y++) {
                if (matrix[x, y] != TetrominoType.Empty) {
                    DrawMino(new Vec2(x, y) + drawOffset, Colors[matrix[x, y]], spriteBatch);
                }
            }
        }

        var observerIndex = 0;
        foreach (var observerMatrix in observerMatrices) {
            for (var x = 0; x < _tetrion.Width; x++) {
                for (var y = 0; y < _tetrion.Height; y++) {
                    if (observerMatrix[x, y] != TetrominoType.Empty) {
                        DrawMino(new Vec2(x + 22, y) + drawOffset with { X = observerIndex * 10 }, Colors[observerMatrix[x, y]], spriteBatch);
                    }
                }
            }

            ++observerIndex;
        }

        if (ghostTetromino is not null) {
            DrawTetromino(ghostTetromino.Value, GhostColors, spriteBatch, Assets, drawOffset);
        }

        if (activeTetromino is not null) {
            DrawTetromino(activeTetromino.Value, Colors, spriteBatch, Assets, drawOffset);
        }

        if (lineClearDelayState.Countdown > 0) {
            var relativeVisibility = (double)lineClearDelayState.Countdown / (double)lineClearDelayState.Delay;
            var color = new Color(
                (byte)Math.Round(255.0 * relativeVisibility),
                (byte)Math.Round(255.0 * relativeVisibility),
                (byte)Math.Round(255.0 * relativeVisibility)
            );
            foreach (var line in lineClearDelayState.Lines) {
                spriteBatch.Draw(
                    Assets.WhiteTexture,
                    new Rectangle(
                        6 * Assets.MinoTexture.Width,
                        (line - 2) * Assets.MinoTexture.Height,
                        Assets.MinoTexture.Width * _tetrion.Width,
                        Assets.MinoTexture.Height
                    ),
                    color
                );
            }
        }

        var allClearRatio = _allClearCountdown.Access((ref int value) => (float)value) / (float)AllClearDuration;
        if (allClearRatio > 0.0) {
            spriteBatch.Draw(
                Assets.WhiteTexture,
                new Rectangle(
                    6 * Assets.MinoTexture.Width,
                    0,
                    Assets.MinoTexture.Width * _tetrion.Width,
                    Assets.MinoTexture.Height * _tetrion.Height
                ),
                Color.White * (float)allClearRatio
            );
        }

        var elapsedSeconds = (double)nextFrame / 60.0;
        var timeString = elapsedSeconds < 60.0
            ? TimeSpan.FromSeconds(elapsedSeconds).ToString(@"ss\.fff")
            : TimeSpan.FromSeconds(elapsedSeconds).ToString(@"mm\:ss\.f");

        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        var timeSinceLastFpsMeasure = _elapsedTime - _lastFpsMeasureTime;
        if (timeSinceLastFpsMeasure >= 1.0) {
            _fpsString = $"{Math.Round((_framesSinceLastFpsMeasure / timeSinceLastFpsMeasure))}";
            _lastFpsMeasureTime = _elapsedTime;
            _framesSinceLastFpsMeasure = 0;
        } else {
            ++_framesSinceLastFpsMeasure;
        }

        var statsString =
            @$"Score:
{stats.Score}

Level:
{stats.Level}

Lines:
{stats.LinesCleared}

Time:
{timeString}

FPS:
{_fpsString}";

        spriteBatch.DrawString(
            Assets.Font,
            statsString,
            new Vector2(Assets.MinoTexture.Width, Assets.MinoTexture.Height * 6),
            Color.Black,
            0,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0.5f
        );

        if (isGameOver) {
            spriteBatch.Draw(
                Assets.WhiteTexture,
                new Rectangle(
                    0, //6 * _minoTexture.Width,
                    0,
                    Assets.MinoTexture.Width * (_tetrion.Width + 12),
                    Assets.MinoTexture.Height * _tetrion.Height
                ),
                Color.Black * 0.5f
            );

            RenderTextCentered(spriteBatch, "Game Over");
        }

        if (framesUntilGameStart > 0) {
            var remainingTime = framesUntilGameStart / 60.0;
            RenderTextCentered(spriteBatch, $"Starting in {remainingTime:0.0} s...");
        }
    }

    private void RenderTextCentered(SpriteBatch spriteBatch, string text) {
        var windowWidth = spriteBatch.GraphicsDevice.Viewport.Width;
        var windowHeight = spriteBatch.GraphicsDevice.Viewport.Height;

        var textSize = Assets.Font.MeasureString("Game Over");
        spriteBatch.DrawString(
            Assets.Font,
            text,
            new Vector2(windowWidth / 2f, windowHeight / 2f),
            Color.White,
            0,
            textSize / 2f,
            1f,
            SpriteEffects.None,
            0.5f
        );
    }

    private void DrawTetrionBackground(SpriteBatch spriteBatch) {
        spriteBatch.Draw(
            Assets.TetrionTexture,
            new Vector2(0f, 0f),
            null,
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0f
        );
    }

    private void DrawTetromino(
        Tetromino activeTetromino, Dictionary<TetrominoType, Color> colors, SpriteBatch spriteBatch, Assets assets,
        Vec2 offset = default
    ) {
        foreach (var minoPosition in activeTetromino.MinoPositions) {
            DrawMino(minoPosition + offset, colors[activeTetromino.Type], spriteBatch);
        }
    }

    private void DrawMino(Vec2 position, Color color, SpriteBatch spriteBatch) {
        spriteBatch.Draw(
            Assets.MinoTexture,
            new Vector2(position.X * Assets.MinoTexture.Width, position.Y * Assets.MinoTexture.Height),
            null,
            color,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0f
        );
    }

    private void KeepSimulating() {
        ulong nextTick = 0;
        var stopwatch = Stopwatch.StartNew();
        while (!_cancellationTokenSource.Token.IsCancellationRequested) {
            var elapsedTime = stopwatch.Elapsed.TotalSeconds;
            var simulationTick = (ulong)(elapsedTime / (1.0 / 60.0));

            if (nextTick >= simulationTick) {
                Thread.Yield();
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
                _allClearCountdown.Access((ref int value) => {
                    if (value > 0) {
                        --value;
                    }
                });
            }

            _tetrionMutex.ReleaseMutex();
            Thread.Yield();
        }
    }

    public void Dispose() {
        _tetrion.Dispose();
    }

    ~SingleplayerScene() {
        Dispose();
    }
}
