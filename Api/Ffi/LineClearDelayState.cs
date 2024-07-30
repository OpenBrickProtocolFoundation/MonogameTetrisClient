using System.Runtime.InteropServices;

namespace MonogameTetrisClient.Api.Ffi;

/*
typedef struct {
        uint8_t count;
        uint8_t lines[4];
        uint64_t countdown;
        uint64_t delay;
    } ObpfLineClearDelayState;
*/
[StructLayout(LayoutKind.Sequential)]
internal struct LineClearDelayState {
    public byte Count;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Lines;
    public ulong Countdown;
    public ulong Delay;
}
