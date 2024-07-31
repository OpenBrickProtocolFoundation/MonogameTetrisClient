namespace Obpf.Api.Ffi;

/* typedef enum {
    OBPF_ACTION_ROTATE_CW,
    OBPF_ACTION_ROTATE_CCW,
    OBPF_ACTION_HARD_DROP,
    OBPF_ACTION_TOUCH,
} ObpfAction;
*/
internal enum Action {
    RotateCw,
    RotateCcw,
    HardDrop,
    Touch,
}
