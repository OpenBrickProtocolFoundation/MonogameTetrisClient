using System.Runtime.InteropServices;

namespace MonogameTetrisClient.Api;

/* typedef struct {
        ObpfTetrominoType types[6];
    } ObpfPreviewPieces;
*/
[StructLayout(LayoutKind.Sequential)]
internal struct PreviewPieces {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public TetrominoType[] Types;
}
