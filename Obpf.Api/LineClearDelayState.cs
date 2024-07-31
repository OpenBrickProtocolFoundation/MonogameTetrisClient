namespace Obpf.Api;

public record struct LineClearDelayState(int[] Lines, ulong Countdown, ulong Delay);
