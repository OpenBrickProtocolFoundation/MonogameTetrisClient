namespace MonogameTetrisClient.Api;

public readonly record struct Stats(ulong Score, uint LinesCleared, uint Level);
