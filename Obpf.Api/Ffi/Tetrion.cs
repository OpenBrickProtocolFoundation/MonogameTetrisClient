using System.Runtime.InteropServices;

namespace Obpf.Api.Ffi;

internal class Tetrion {
    // OBPF_EXPORT ObpfMinoPositions obpf_tetromino_get_mino_positions(ObpfTetrominoType type, ObpfRotation rotation);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetromino_get_mino_positions")]
    public static extern MinoPositions GetMinoPositions(TetrominoType type, Rotation rotation);

    // struct ObpfTetrion* obpf_create_tetrion(uint64_t seed);
    [DllImport(Common.DllPath, EntryPoint = "obpf_create_tetrion")]
    public static extern IntPtr CreateTetrion(ulong seed);

    // struct ObpfTetrion* obpf_create_multiplayer_tetrion(char const* const host, uint16_t const port)
    [DllImport(Common.DllPath, CharSet = CharSet.Ansi, EntryPoint = "obpf_create_multiplayer_tetrion")]
    public static extern IntPtr CreateMultiplayerTetrion(string host, ushort port);

    // struct ObpfObserverList obpf_tetrion_get_observers(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_get_observers")]
    public static extern ObserverList GetObservers(IntPtr tetrion);

    // void obpf_destroy_observers(struct ObpfObserverList observers);
    [DllImport(Common.DllPath, EntryPoint = "obpf_destroy_observers")]
    public static extern void DestroyObservers(ObserverList observers);

    // struct ObpfTetrion* obpf_clone_tetrion(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_clone_tetrion")]
    public static extern IntPtr CloneTetrion(IntPtr tetrion);

    // typedef void (*ObpfActionHandler)(ObpfAction action, void* user_data);
    public delegate void ActionHandler(Action action, IntPtr userData);

    /* void obpf_tetrion_set_action_handler(
        struct ObpfTetrion* tetrion,
        ObpfActionHandler handler,
        void* user_data
    ); */
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_set_action_handler")]
    public static extern void SetActionHandler(IntPtr tetrion, ActionHandler? handler, IntPtr userData);

    // ObpfStats obpf_tetrion_get_stats(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_get_stats")]
    public static extern Stats GetStats(IntPtr tetrion);

    // bool obpf_tetrion_is_game_over(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_is_game_over")]
    public static extern bool IsGameOver(IntPtr tetrion);

    // OBPF_EXPORT ObpfLineClearDelayState obpf_tetrion_get_line_clear_delay_state(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_get_line_clear_delay_state")]
    public static extern LineClearDelayState GetLineClearDelayState(IntPtr tetrion);

    // void obpf_destroy_tetrion(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_destroy_tetrion")]
    public static extern void DestroyTetrion(IntPtr tetrion);

    // bool obpf_tetrion_try_get_active_tetromino(
    //     struct ObpfTetrion const* tetrion,
    //     struct ObpfTetromino* out_tetromino
    // );
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_try_get_active_tetromino")]
    public static extern bool TryGetActiveTetromino(IntPtr tetrion, out Tetromino tetromino);

    // bool obpf_tetrion_try_get_ghost_tetromino(
    //     struct ObpfTetrion const* tetrion,
    //     struct ObpfTetromino* out_tetromino
    // );
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_try_get_ghost_tetromino")]
    public static extern bool TryGetGhostTetromino(IntPtr tetrion, out Tetromino tetromino);

    // ObpfPreviewPieces obpf_tetrion_get_preview_pieces(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_get_preview_pieces")]
    public static extern PreviewPieces GetPreviewPieces(IntPtr tetrion);

    // ObpfTetrominoType obpf_tetrion_get_hold_piece(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_get_hold_piece")]
    public static extern TetrominoType GetHoldPiece(IntPtr tetrion);

    // uint64_t obpf_tetrion_get_next_frame(struct ObpfTetrion const* tetrion);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_get_next_frame")]
    public static extern ulong GetNextFrame(IntPtr tetrion);

    // void obpf_tetrion_simulate_next_frame(struct ObpfTetrion* tetrion, ObpfKeyState key_state);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_simulate_next_frame")]
    public static extern void SimulateNextFrame(IntPtr tetrion, KeyState keyState);

    // uint8_t obpf_tetrion_width(void);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_width")]
    public static extern byte GetWidth();

    // uint8_t obpf_tetrion_height(void);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_height")]
    public static extern byte GetHeight();

    // uint8_t obpf_tetrion_num_invisible_lines(void);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_num_invisible_lines")]
    public static extern byte GetNumInvisibleLines();

    // OBPF_EXPORT ObpfTetrominoType obpf_tetrion_matrix_get(const struct ObpfTetrion* tetrion, ObpfVec2 position);
    [DllImport(Common.DllPath, EntryPoint = "obpf_tetrion_matrix_get")]
    public static extern TetrominoType GetMatrixValue(IntPtr tetrion, Vec2 position);
}
