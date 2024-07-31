namespace Obpf.Api.Ffi {
    /*
typedef enum {
    OBPF_ROTATION_NORTH = 0,
    OBPF_ROTATION_EAST,
    OBPF_ROTATION_SOUTH,
    OBPF_ROTATION_WEST,
    OBPF_ROTATION_LAST_ROTATION = OBPF_ROTATION_WEST,
} ObpfRotation;
*/
    internal enum Rotation {
        North = 0,
        East,
        South,
        West,
        LastRotation = West,
    }
}
