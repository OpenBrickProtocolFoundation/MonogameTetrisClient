using System.Runtime.InteropServices;

namespace Obpf.Api.Ffi;

/* struct ObpfGarbageEvent {
        uint8_t num_lines;
        uint64_t remaining_frames;
    }; */
[StructLayout(LayoutKind.Sequential)]
internal struct GarbageEvent {
    public byte NumLines;
    public ulong RemainingFrames;
}
