using System.Runtime.InteropServices;

namespace MonogameTetrisClient.Api;

/*
 * typedef struct {
        uint8_t bitmask;
    } ObpfKeyState;
 */
[StructLayout(LayoutKind.Sequential)]
internal struct KeyState {
    public byte Bitmask;
}
