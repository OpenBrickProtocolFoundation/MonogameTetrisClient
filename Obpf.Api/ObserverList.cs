using System.Runtime.InteropServices;

namespace Obpf.Api;

public class ObserverList : IDisposable {
    private readonly Ffi.ObserverList _observerList;
    public Tetrion[] Observers { get; init; }

    internal ObserverList(Ffi.ObserverList observerList) {
        _observerList = observerList;
        Observers = new Tetrion[_observerList.NumObservers.ToUInt32()];
        for (var i = 0; i < Observers.Length; ++i) {
            Observers[i] = new Tetrion(Marshal.ReadIntPtr(_observerList.Observers, i * IntPtr.Size));
        }
    }

    private void ReleaseUnmanagedResources() {
        Ffi.Tetrion.DestroyObservers(_observerList);
    }

    public void Dispose() {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~ObserverList() {
        ReleaseUnmanagedResources();
    }
}
