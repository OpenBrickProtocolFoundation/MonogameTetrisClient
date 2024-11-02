namespace Obpf.Api;

public record struct GarbageEvent(byte NumLines, ulong RemainingFrames);
