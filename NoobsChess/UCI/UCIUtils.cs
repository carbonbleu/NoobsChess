using NoobsChess.Io;
using NoobsChess.Search;
using NoobsEngine;
using NoobsEngine.Enums;
using NoobsEngine.Fen;

namespace NoobsChess.UCI;

public class UCIUtils {
    public static void UCILoop() {
        #pragma warning disable CA1416 // Validate platform compatibility
        Console.WriteLine("id name {0}", NoobsDefs.EngineName);
        Console.WriteLine("id author Carbonbleu / Clueless-Skywatcher");
        Console.WriteLine("uciok");

        ChessBoard board = new ChessBoard();
        SearchInfo info = new SearchInfo();

        while (true) {
            Console.Out.Flush();
            String? line = Console.ReadLine();
            if (line == null) {
                continue;
            }

            if (line[0] == '\n') {
                continue;
            } 

            if (line.Equals("isready")) {
                Console.WriteLine("readyok");
                continue;
            }
            else if (line.StartsWith("position")) {
                ParsePosition(line, board);
            }
            else if (line.StartsWith("ucinewgame")) {
                ParsePosition("position startpos", board);
            }
            else if (line.StartsWith("go")) {
                ParseGo(line, info, board);
            }
            else if (line.Equals("quit")) {
                if (info.SearchThread != null) {
                    info.SearchThread.Interrupt();
                }
                info.IsQuit = true;
                break;
            }
            else if (line.Equals("stop")) {
                if (info.SearchThread != null) {
                    info.SearchThread.Interrupt();
                }
                info.IsStopped = true;
            }
            else if (line.Equals("uci")) {
                Console.WriteLine("id name {0}", NoobsDefs.EngineName);
                Console.WriteLine("id author Carbonbleu / Clueless-Skywatcher");
                Console.WriteLine("uciok");
            }

            if (info.IsQuit) break;
        }

        board.PVTable.Clear();
    }

    public static void ParsePosition(String line, ChessBoard position) {
        String command = line["position ".Length..];
        if (command.StartsWith("startpos")) {
            ParseFen(NoobsDefs.StartingFEN, position);
        }
        else if (command.StartsWith("fen")) {
            String fen = command.Substring("fen ".Length);
            if (fen.Length == 0) {
                ParseFen(NoobsDefs.StartingFEN, position);
            }
            else {
                ParseFen(fen, position);
            }
        }
        // position startpos moves e2e4 e7e5
        int movesIdx = line.IndexOf("moves");

        if (movesIdx != -1) {
            movesIdx += 6;
            String[] moves = line.Substring(movesIdx).Split(" ");
            foreach (String moveStr in moves) {
                Move move = MoveParse.ParseMove(moveStr, position);
                if (move.Equals(NoobsDefs.NoMove)) {
                    break;
                }
                position.MakeMove(move);
                position.Ply = 0;
            }
        }
    }

    public static void ParseGo(String line, SearchInfo info, ChessBoard position) {
        // go depth 6 wtime 180000 btime 100000 binc 1000 winc 1000 movetime 1000 movestogo 40
        int movesToGo = 30;
        int depth = -1;
        int moveTime = -1;
        int time = -1;
        int inc = 0;

        info.IsTimeSet = false;

        String[] commands = line.Split(" ");

        for (int i = 1; i < commands.Length; i++) {
            switch (commands[i]) {
                case "btime":
                    if (position.SideToMove == (int) Players.Black) {
                        time = int.Parse(commands[i + 1]);
                    }
                    break;
                case "wtime":
                    if (position.SideToMove == (int) Players.White) {
                        time = int.Parse(commands[i + 1]);
                    }
                    break;
                case "binc":
                    if (position.SideToMove == (int) Players.Black) {
                        inc = int.Parse(commands[i + 1]);
                    }
                    break;
                case "winc":
                    if (position.SideToMove == (int) Players.White) {
                        inc = int.Parse(commands[i + 1]);
                    }
                    break;
                case "depth" :
                    depth = int.Parse(commands[i + 1]);
                    break;
                case "movetime":
                    moveTime = int.Parse(commands[i + 1]);
                    break;
                case "movestogo":
                    movesToGo = int.Parse(commands[i + 1]);
                    break;
            }
        }

        if (moveTime != -1) {
            time = moveTime;
            movesToGo = 1;
        }

        info.StartTime = NoobsUtils.GetTimeInMs();
        info.DepthLimit = depth;
        
        if (time != -1) {
            info.IsTimeSet = true;
            time /= movesToGo;
            time -= 50;
            info.StopTime = info.StartTime + time + inc;
        }

        if (depth == -1) {
            info.DepthLimit = NoobsDefs.MaxDepth;
        }

        Console.WriteLine("time {0} start {1} stop {2} depth {3} timeset {4}", time, info.StartTime, info.StopTime, info.DepthLimit, info.IsTimeSet);

        info.SearchThread = new Thread(() => position.SearchPosition(info));
        info.SearchThread.Start();
    }

    public static void ParseFen(String fen, ChessBoard board) {
        FenUtils.ParseFen(fen, board);
    }
}