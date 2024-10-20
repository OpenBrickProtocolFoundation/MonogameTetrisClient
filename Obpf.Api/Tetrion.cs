using System.Runtime.InteropServices;
using Obpf.Api.Ffi;

namespace Obpf.Api;

public class Tetrion : ICloneable, IDisposable {
    private readonly IntPtr _tetrion;
    private bool _disposed = false;
    public int Width { get; init; }
    public int Height { get; init; }
    public int NumInvisibleLines { get; init; }
    public ObserverList Observers { get; init; }
    private TetrominoType[,] _matrixCache;
    private TetrominoType[] _previewCache;

    public delegate void ActionHandler(Action action);

    /* The following static field is required to prevent the delegate from being garbage collected. */
    private static Ffi.Tetrion.ActionHandler s_handleActionDelegateInstance = HandleAction;
    private ActionHandler? _actionHandler = null;
    private GCHandle? _userDataHandle = null;

    internal Tetrion(IntPtr tetrion) {
        _tetrion = tetrion;
        Observers = new ObserverList(Ffi.Tetrion.GetObservers(_tetrion));
        Width = Ffi.Tetrion.GetWidth();
        Height = Ffi.Tetrion.GetHeight();
        NumInvisibleLines = Ffi.Tetrion.GetNumInvisibleLines();
        _matrixCache = new TetrominoType[Width, Height];
        _previewCache = new TetrominoType[6];
    }

    public Tetrion(ulong seed) : this(Ffi.Tetrion.CreateTetrion(seed)) { }

    public Tetrion(string host, ushort port) : this(Ffi.Tetrion.CreateMultiplayerTetrion(host, port)) { }

    private static void HandleAction(Ffi.Action action, IntPtr userData) {
        // todo: null checks
        var handle = GCHandle.FromIntPtr(userData);
        var tetrion = (Tetrion)handle.Target;
        tetrion._actionHandler?.Invoke((Action)action);
    }

    public void SetActionHandler(ActionHandler? handler) {
        if (_userDataHandle != null) {
            _userDataHandle.Value.Free();
            _userDataHandle = null;
        }

        _actionHandler = handler;
        if (handler == null) {
            Ffi.Tetrion.SetActionHandler(_tetrion, null, IntPtr.Zero);
            return;
        }

        _userDataHandle = GCHandle.Alloc(this);
        Ffi.Tetrion.SetActionHandler(_tetrion, s_handleActionDelegateInstance, GCHandle.ToIntPtr(_userDataHandle.Value));
    }

    public bool IsConnected() {
        return Ffi.Tetrion.IsConnected(_tetrion);
    }

    public ulong GetFramesUntilGameStart() {
        return Ffi.Tetrion.FramesUntilGameStart(_tetrion);
    }

    public Stats GetStats() {
        var ffiStats = Ffi.Tetrion.GetStats(_tetrion);
        return new Stats(ffiStats.Score, ffiStats.LinesCleared, ffiStats.Level);
    }

    public bool IsGameOver() {
        return Ffi.Tetrion.IsGameOver(_tetrion);
    }

    public LineClearDelayState GetLineClearDelayState() {
        var ffiState = Ffi.Tetrion.GetLineClearDelayState(_tetrion);
        var clearedLines = new int[ffiState.Count];
        for (var i = 0; i < ffiState.Count; i++) {
            clearedLines[i] = (int)ffiState.Lines[i];
        }

        return new LineClearDelayState(clearedLines, ffiState.Countdown, ffiState.Delay);
    }

    public Tetromino? TryGetActiveTetromino() {
        if (!Ffi.Tetrion.TryGetActiveTetromino(_tetrion, out var tetromino)) {
            return null;
        }

        var minoPositions = tetromino.MinoPositions.Select(p => new Vec2(p.X, p.Y)).ToArray();
        return new Tetromino(minoPositions, (TetrominoType)tetromino.Type);
    }

    public Tetromino? TryGetGhostTetromino() {
        if (!Ffi.Tetrion.TryGetGhostTetromino(_tetrion, out var tetromino)) {
            return null;
        }

        var minoPositions = tetromino.MinoPositions.Select(p => new Vec2(p.X, p.Y)).ToArray();
        return new Tetromino(minoPositions, (TetrominoType)tetromino.Type);
    }

    public TetrominoType GetHoldPiece() {
        return (TetrominoType)Ffi.Tetrion.GetHoldPiece(_tetrion);
    }

    public TetrominoType[] GetPreviewPieces() {
        var ffiPreviewPieces = Ffi.Tetrion.GetPreviewPieces(_tetrion);
        for (var i = 0; i < ffiPreviewPieces.Types.Length; i++) {
            _previewCache[i] = (TetrominoType)ffiPreviewPieces.Types[i];
        }

        return _previewCache;
    }

    public ulong GetNextFrame() {
        return Ffi.Tetrion.GetNextFrame(_tetrion);
    }

    public void SimulateNextFrame(KeyState keyState) {
        var ffiKeyState = Common.CreateKeyState(
            keyState.Left,
            keyState.Right,
            keyState.Down,
            keyState.Drop,
            keyState.RotateCw,
            keyState.RotateCcw,
            keyState.Hold
        );
        Ffi.Tetrion.SimulateNextFrame(_tetrion, ffiKeyState);
    }

    public TetrominoType[,] GetMatrix() {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                _matrixCache[x, y] = (TetrominoType)Ffi.Tetrion.GetMatrixValue(
                    _tetrion,
                    new Ffi.Vec2 { X = (byte)x, Y = (byte)y }
                );
            }
        }

        return _matrixCache;
    }

    public static Vec2[] GetMinoPositions(TetrominoType type, Rotation rotation) {
        var ffiMinoPositions = Ffi.Tetrion.GetMinoPositions((Ffi.TetrominoType)type, (Ffi.Rotation)rotation);
        return ffiMinoPositions.Positions.Select(p => new Vec2(p.X, p.Y)).ToArray();
    }

    private void ReleaseUnmanagedResources() {
        Ffi.Tetrion.DestroyTetrion(_tetrion);
        _userDataHandle?.Free();
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        ReleaseUnmanagedResources();
        if (disposing) {
            // TODO release managed resources here
            Observers.Dispose();
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

    public object Clone() {
        return new Tetrion(Ffi.Tetrion.CloneTetrion(_tetrion));
    }
}
