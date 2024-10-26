using System;
using MonogameTetrisClient;

if (args.Length == 0) {
    using var singleplayerGame = new ObpfGame();
    singleplayerGame.Run();

    return 0;
}

if (args.Length != 2) {
    Console.Error.WriteLine("Usage: MonogameTetrisClient <server> <port>");
    return 1;
}

var server = args[0];
var port = ushort.Parse(args[1]);

using var multiplayerGame = new ObpfGame(server, port);
multiplayerGame.Run();

return 0;
