using System;
using System.Runtime.InteropServices;

namespace MonogameTetrisClient.Api;

internal class Tetrion {
    // struct ObpfTetrion* obpf_create_tetrion(uint64_t seed);
    [DllImport(Common.DllPath, EntryPoint = "obpf_create_tetrion")]
    public static extern IntPtr CreateTetrion(ulong seed);

    // void obpf_destroy_tetrion(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_destroy_tetrion")]
    public static extern void DestroyTetrion(IntPtr tetrion);

    // bool obpf_tetrion_try_get_active_tetromino(
    //     struct ObpfTetrion const* tetrion,
    //     struct ObpfTetromino* out_tetromino
    // );
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_try_get_active_tetromino")]
    public static extern bool TryGetActiveTetromino(IntPtr tetrion, out Tetromino tetromino);

    // uint64_t obpf_tetrion_get_next_frame(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_get_next_frame")]
    public static extern ulong GetNextFrame(IntPtr tetrion);

    // void obpf_tetrion_simulate_next_frame(struct ObpfTetrion* tetrion, ObpfKeyState key_state);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_simulate_next_frame")]
    public static extern void SimulateNextFrame(IntPtr tetrion, KeyState keyState);
}