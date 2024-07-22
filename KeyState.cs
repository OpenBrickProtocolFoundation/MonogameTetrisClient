namespace MonogameTetrisClient;

public readonly record struct KeyState(
    bool Left,
    bool Right,
    bool Down,
    bool Drop,
    bool RotateCw,
    bool RotateCcw,
    bool Hold
);