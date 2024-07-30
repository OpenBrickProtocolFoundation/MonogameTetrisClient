using System;
using System.Linq;

namespace MonogameTetrisClient;

public class Tetrion : IDisposable {
    private readonly IntPtr _tetrion;
    private bool _disposed = false;
    public int Width { get; init; }
    public int Height { get; init; }
    public int NumInvisibleLines { get; init; }
    private TetrominoType[,] _matrixCache;
    private TetrominoType[] _previewCache;

    public Tetrion(ulong seed) {
        _tetrion = Api.Tetrion.CreateTetrion(seed);
        Width = Api.Tetrion.GetWidth();
        Height = Api.Tetrion.GetHeight();
        NumInvisibleLines = Api.Tetrion.GetNumInvisibleLines();
        _matrixCache = new TetrominoType[Width, Height];
        _previewCache = new TetrominoType[6];
    }

    public Stats GetStats() {
        var ffiStats = Api.Tetrion.GetStats(_tetrion);
        return new Stats(ffiStats.Score, ffiStats.LinesCleared, ffiStats.Level);
    }

    public LineClearDelayState GetLineClearDelayState() {
        var ffiState = Api.Tetrion.GetLineClearDelayState(_tetrion);
        var clearedLines = new int[ffiState.Count];
        for (var i = 0; i < ffiState.Count; i++) {
            clearedLines[i] = (int)ffiState.Lines[i];
        }

        return new LineClearDelayState(clearedLines, ffiState.Countdown, ffiState.Delay);
    }

    public Tetromino? TryGetActiveTetromino() {
        if (!Api.Tetrion.TryGetActiveTetromino(_tetrion, out var tetromino)) {
            return null;
        }

        var minoPositions = tetromino.MinoPositions.Select(p => new Vec2(p.X, p.Y)).ToArray();
        return new Tetromino(minoPositions, (TetrominoType)tetromino.Type);
    }

    public Tetromino? TryGetGhostTetromino() {
        if (!Api.Tetrion.TryGetGhostTetromino(_tetrion, out var tetromino)) {
            return null;
        }

        var minoPositions = tetromino.MinoPositions.Select(p => new Vec2(p.X, p.Y)).ToArray();
        return new Tetromino(minoPositions, (TetrominoType)tetromino.Type);
    }

    public TetrominoType GetHoldPiece() {
        return (TetrominoType)Api.Tetrion.GetHoldPiece(_tetrion);
    }

    public TetrominoType[] GetPreviewPieces() {
        var ffiPreviewPieces = Api.Tetrion.GetPreviewPieces(_tetrion);
        for (var i = 0; i < ffiPreviewPieces.Types.Length; i++) {
            _previewCache[i] = (TetrominoType)ffiPreviewPieces.Types[i];
        }

        return _previewCache;
    }

    public ulong GetNextFrame() {
        return Api.Tetrion.GetNextFrame(_tetrion);
    }

    public void SimulateNextFrame(KeyState keyState) {
        var ffiKeyState = Api.Common.CreateKeyState(
            keyState.Left,
            keyState.Right,
            keyState.Down,
            keyState.Drop,
            keyState.RotateCw,
            keyState.RotateCcw,
            keyState.Hold
        );
        Api.Tetrion.SimulateNextFrame(_tetrion, ffiKeyState);
    }

    public TetrominoType[,] GetMatrix() {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                _matrixCache[x, y] = (TetrominoType)Api.Tetrion.GetMatrixValue(
                    _tetrion,
                    new Api.Vec2 { X = (byte)x, Y = (byte)y }
                );
            }
        }

        return _matrixCache;
    }

    public static Vec2[] GetMinoPositions(TetrominoType type, Rotation rotation) {
        var ffiMinoPositions = Api.Tetrion.GetMinoPositions((Api.TetrominoType)type, (Api.Rotation)rotation);
        return ffiMinoPositions.Positions.Select(p => new Vec2(p.X, p.Y)).ToArray();
    }

    private void ReleaseUnmanagedResources() {
        Api.Tetrion.DestroyTetrion(_tetrion);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        ReleaseUnmanagedResources();
        if (disposing) {
            // TODO release managed resources here
        }

        _disposed = true;
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    ~Tetrion() {
        Dispose(disposing: false);
    }
}
