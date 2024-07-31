using System.Runtime.InteropServices;

namespace Obpf.Api.Ffi;

/* typedef struct {
        ObpfTetrominoType types[6];
    } ObpfPreviewPieces;
*/
[StructLayout(LayoutKind.Sequential)]
internal struct PreviewPieces {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public TetrominoType[] Types;
}
