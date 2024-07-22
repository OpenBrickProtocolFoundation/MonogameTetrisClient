using System.Runtime.InteropServices;

namespace MonogameTetrisClient.Api;

/* struct ObpfTetromino {
    ObpfVec2 mino_positions[4];
    ObpfTetrominoType type;
};
 */
[StructLayout(LayoutKind.Sequential)]
internal struct Tetromino {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public Vec2[] MinoPositions;

    public TetrominoType Type;
}