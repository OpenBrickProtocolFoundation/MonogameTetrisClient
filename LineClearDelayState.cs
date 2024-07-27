namespace MonogameTetrisClient;

public record struct LineClearDelayState(int[] Lines, ulong Countdown, ulong Delay);
