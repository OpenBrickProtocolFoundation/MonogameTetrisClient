using System;
using System.Linq;

namespace MonogameTetrisClient;

public class Tetrion : IDisposable {
    private readonly IntPtr _tetrion;
    private bool _disposed = false;

    public Tetrion(ulong seed) {
        _tetrion = Api.Tetrion.CreateTetrion(seed);
    }

    public Tetromino? TryGetActiveTetromino() {
        if (!Api.Tetrion.TryGetActiveTetromino(_tetrion, out var tetromino)) {
            return null;
        }

        var minoPositions = tetromino.MinoPositions.Select(p => new Vec2(p.X, p.Y)).ToArray();
        return new Tetromino(minoPositions, (TetrominoType)tetromino.Type);
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
