
﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TicTacToe
{
    static bool CheckForWin(char[] b, char p) =>
        (b[1]==p && b[2]==p && b[3]==p) ||
        (b[4]==p && b[5]==p && b[6]==p) ||
        (b[7]==p && b[8]==p && b[9]==p) ||
        (b[1]==p && b[4]==p && b[7]==p) ||
        (b[2]==p && b[5]==p && b[8]==p) ||
        (b[3]==p && b[6]==p && b[9]==p) ||
        (b[3]==p && b[5]==p && b[7]==p) ||
        (b[1]==p && b[5]==p && b[9]==p);

    static bool IsFree(char[] b, int i) => b[i] == ' ';

    static bool CheckForTie(char[] b)
    {
        for (int i=1;i<=9;i++) if (IsFree(b,i)) return false;
        return true;
    }

    // 업로드하신 로직과 동일한 간단 AI
    static int GetComputerMove(char[] board)
    {
        // 이길 수 있으면 승리 수
        for (int i=1;i<=9;i++)
        {
            if (IsFree(board,i))
            {
                board[i]='O';
                if (CheckForWin(board,'O')) { board[i]=' '; return i; }
                board[i]=' ';
            }
        }
        // 막아야 하면 방어 수
        for (int i=1;i<=9;i++)
        {
            if (IsFree(board,i))
            {
                board[i]='X';
                if (CheckForWin(board,'X')) { board[i]=' '; return i; }
                board[i]=' ';
            }
        }
        // 코너
        for (int i=1;i<=9;i+=2) if (i!=5 && IsFree(board,i)) return i;
        // 센터
        if (IsFree(board,5)) return 5;
        // 엣지
        for (int i=2;i<=9;i+=2) if (IsFree(board,i)) return i;

        return 1;
    }

    static string BoardToWire(char[] b)
    {
        var sb = new StringBuilder(9);
        for (int i=1;i<=9;i++)
            sb.Append(b[i] == ' ' ? '-' : b[i]);
        return sb.ToString();
    }

    static void SendBoard(StreamWriter w, char[] b) =>
        w.WriteLine("BOARD " + BoardToWire(b));

    static void HandleClient(TcpClient client)
    {
        using var ns = client.GetStream();
        using var reader = new StreamReader(ns, Encoding.UTF8);
        using var writer = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };

        Console.WriteLine($"[SERVER] Client connected from {client.Client.RemoteEndPoint}");

        // 초기화
        char[] board = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' }; // 0번 미사용
        char user = 'X';
        char ai   = 'O';

        writer.WriteLine("INFO Welcome to TicTacToe Server");
        writer.WriteLine($"INFO You are '{user}'. Enter moves 1-9.");
        SendBoard(writer, board);

        bool gameOver = false;

        while (!gameOver)
        {
            // 1) 사용자 차례
            writer.WriteLine("YOUR_MOVE");

            while (true)
            {
                string? line = reader.ReadLine();
                if (line == null) { Console.WriteLine("[SERVER] Client disconnected."); return; }

                var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && parts[0].Equals("MOVE", StringComparison.OrdinalIgnoreCase)
                    && int.TryParse(parts[1], out int loc))
                {
                    if (loc < 1 || loc > 9) { writer.WriteLine("INVALID OutOfRange"); continue; }
                    if (!IsFree(board, loc)) { writer.WriteLine("INVALID Occupied"); continue; }

                    board[loc] = user;
                    SendBoard(writer, board);

                    if (CheckForWin(board, user))
                    {
                        writer.WriteLine("RESULT X");
                        writer.WriteLine("BYE");
                        gameOver = true;
                    }
                    else if (CheckForTie(board))
                    {
                        writer.WriteLine("RESULT TIE");
                        writer.WriteLine("BYE");
                        gameOver = true;
                    }
                    break;
                }
                else
                {
                    writer.WriteLine("INVALID Format"); // 예: MOVE 5
                }
            }

            if (gameOver) break;

            // 2) 서버(AI) 차례
            int aiMove = GetComputerMove(board);
            board[aiMove] = ai;
            writer.WriteLine($"OPPONENT_MOVE {aiMove}");
            SendBoard(writer, board);

            if (CheckForWin(board, ai))
            {
                writer.WriteLine("RESULT O");
                writer.WriteLine("BYE");
                gameOver = true;
            }
            else if (CheckForTie(board))
            {
                writer.WriteLine("RESULT TIE");
                writer.WriteLine("BYE");
                gameOver = true;
            }
        }

        Console.WriteLine("[SERVER] Game finished. Closing connection.");
    }

    public static void Main(string[] args)
    {
        int port = args.Length > 0 && int.TryParse(args[0], out var p) ? p : 5000;
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"[SERVER] Listening on 0.0.0.0:{port} ...");

        while (true)
        {
            var client = listener.AcceptTcpClient();
            // 단순화를 위해 동기 1-클라이언트 처리 (필요하면 Task.Run으로 병렬화 가능)
            HandleClient(client);
            client.Close();
        }
    }
}
