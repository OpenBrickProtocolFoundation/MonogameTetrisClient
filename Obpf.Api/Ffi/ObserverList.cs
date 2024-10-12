using System.Runtime.InteropServices;

namespace Obpf.Api.Ffi {
    /* struct ObpfObserverList {
        size_t num_observers;
        struct ObpfTetrion** observers;
    }; */
    [StructLayout(LayoutKind.Sequential)]
    internal struct ObserverList {
        public UIntPtr NumObservers;
        public IntPtr Observers;
    }
}
