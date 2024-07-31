using System.Runtime.InteropServices;

namespace Obpf.Api.Ffi;

internal class Common {
    public const string DllPath = @"C:\dev\cpp\obpf-simulator\cmake-build-msvc-debug\bin\obpf\obpf_d.dll";

    /* ObpfKeyState obpf_key_state_create(
        bool left,
        bool right,
        bool down,
        bool drop,
        bool rotate_clockwise,
        bool rotate_counter_clockwise,
        bool hold
    );
    */
    [DllImport(DllPath, EntryPoint = "obpf_key_state_create")]
    public static extern KeyState CreateKeyState(
        bool left,
        bool right,
        bool down,
        bool drop,
        bool rotateClockwise,
        bool rotateCounterClockwise,
        bool hold
    );
}
