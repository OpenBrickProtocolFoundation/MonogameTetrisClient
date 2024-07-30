using System.Runtime.InteropServices;

namespace MonogameTetrisClient.Api;

/* typedef struct {
    uint64_t score;
    uint32_t lines_cleared;
    uint32_t level;
} ObpfStats;
*/
[StructLayout(LayoutKind.Sequential)]
internal struct Stats {
    public ulong Score;
    public uint LinesCleared;
    public uint Level;
}
