using System.Runtime.InteropServices;

namespace Obpf.Api.Ffi;

/*
 * typedef struct {
        uint8_t bitmask;
    } ObpfKeyState;
 */
[StructLayout(LayoutKind.Sequential)]
internal struct KeyState {
    public byte Bitmask;
}
