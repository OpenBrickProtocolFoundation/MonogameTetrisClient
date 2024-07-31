using System.Runtime.InteropServices;

namespace Obpf.Api.Ffi {
    /* typedef struct {
    uint8_t x;
    uint8_t y;
} ObpfVec2;
*/
    [StructLayout(LayoutKind.Sequential)]
    internal struct Vec2 {
        public byte X;
        public byte Y;
    }
}
