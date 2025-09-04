using System;

class TicTacToe
{
    // 게임판을 콘솔에 그립니다.
    static void DrawBoard(char[] board)
    {
        Console.Clear(); // 콘솔 화면을 지우고 새로 그립니다.
        Console.WriteLine("------------");
        Console.WriteLine(" {0} | {1} | {2} ", board[1], board[2], board[3]);
        Console.WriteLine("------------");
        Console.WriteLine(" {0} | {1} | {2} ", board[4], board[5], board[6]);
        Console.WriteLine("------------");
        Console.WriteLine(" {0} | {1} | {2} ", board[7], board[8], board[9]);
        Console.WriteLine("------------");
    }

    // 주어진 플레이어의 승리 여부를 확인합니다.
    static bool CheckForWin(char[] board, char player)
    {
        return (board[1] == player && board[2] == player && board[3] == player) ||
               (board[4] == player && board[5] == player && board[6] == player) ||
               (board[7] == player && board[8] == player && board[9] == player) ||
               (board[1] == player && board[4] == player && board[7] == player) ||
               (board[2] == player && board[5] == player && board[8] == player) ||
               (board[3] == player && board[6] == player && board[9] == player) ||
               (board[3] == player && board[5] == player && board[7] == player) ||
               (board[1] == player && board[5] == player && board[9] == player);
    }

    // 특정 칸이 비어있는지 확인합니다.
    static bool IsFree(char[] board, int loc)
    {
        return loc >= 1 && loc <= 9 && board[loc] == ' ';
    }

    // 무승부인지 확인합니다.
    static bool CheckForTie(char[] board)
    {
        for (int i = 1; i < 10; i++)
        {
            if (board[i] == ' ')
                return false;
        }
        return true;
    }

    // 사용자 입력을 안전하게 받습니다.
    static int GetPlayerMove(char[] board, char player)
    {
        int loc = 0;
        while (true)
        {
            Console.Write($"플레이어 '{player}' 차례입니다. 1에서 9 사이의 숫자를 입력하세요: ");
            string input = Console.ReadLine();
            
            if (int.TryParse(input, out loc) && IsFree(board, loc))
            {
                return loc;
            }
            Console.WriteLine("잘못된 입력입니다. 빈 칸의 1~9 숫자를 다시 입력하세요.");
        }
    }

    static void Main()
    {
        Console.WriteLine("2인용 틱택토 게임에 오신 것을 환영합니다!");
        char[] board = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        bool playing = true;
        char turn = 'X'; // 'X' 플레이어가 먼저 시작

        DrawBoard(board);

        while (playing)
        {
            int loc = GetPlayerMove(board, turn);

            board[loc] = turn;
            DrawBoard(board);

            if (CheckForWin(board, turn))
            {
                Console.WriteLine($"플레이어 '{turn}'가 승리했습니다!");
                playing = false;
            }
            else if (CheckForTie(board))
            {
                Console.WriteLine("무승부입니다!");
                playing = false;
            }
            else
            {
                // 차례 변경
                turn = (turn == 'X') ? 'O' : 'X';
            }
        }
    }
}
