namespace Obpf.Api.Ffi;

/* typedef enum {
        OBPF_ACTION_ROTATE_CW,
        OBPF_ACTION_ROTATE_CCW,
        OBPF_ACTION_HARD_DROP,
        OBPF_ACTION_TOUCH,
        OBPF_ACTION_CLEAR1,
        OBPF_ACTION_CLEAR2,
        OBPF_ACTION_CLEAR3,
        OBPF_ACTION_CLEAR4,
        OBPF_ACTION_ALL_CLEAR,
    } ObpfAction;
*/
internal enum Action {
    RotateCw,
    RotateCcw,
    HardDrop,
    Touch,
    Clear1,
    Clear2,
    Clear3,
    Clear4,
    AllClear,
}
