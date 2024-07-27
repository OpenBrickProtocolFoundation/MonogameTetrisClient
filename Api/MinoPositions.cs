using System.Runtime.InteropServices;

namespace MonogameTetrisClient.Api;

/*
 * typedef struct {
        ObpfVec2 positions[4];
    } ObpfMinoPositions;
 */
[StructLayout(LayoutKind.Sequential)]
internal struct MinoPositions {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public Vec2[] Positions;
}
