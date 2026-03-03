using System;

namespace WinFormsApp1
{
    public class GameLogic
    {
        private int[,] board;
        private bool win;
        private const int ROWS = 6;
        private const int COLS = 7;

        public const int EMPTY = 0;
        public const int PLAYER = 1;
        public const int COMPUTER = 2;

        public bool IsWin => win;

        public GameLogic()
        {
            board = new int[ROWS, COLS];
            win = false;
        }

        public int[,] Board => board;

        public bool Move(int col, int player)
        {
            for (int row = ROWS - 1; row >= 0; row--)
            {
                if (board[row, col] == EMPTY)
                {
                    board[row, col] = player;
                    CheckForWin(row, col, player);
                    return true;
                }
            }
            return false;
        }

        private void CheckForWin(int row, int col, int player)
        {
            if (Count(row, col, 1, 0, player) + Count(row, col, -1, 0, player) > 2) win = true; // Horizontal
            else if (Count(row, col, 0, 1, player) + Count(row, col, 0, -1, player) > 2) win = true; // Vertical
            else if (Count(row, col, 1, 1, player) + Count(row, col, -1, -1, player) > 2) win = true; // Diagonal /
            else if (Count(row, col, 1, -1, player) + Count(row, col, -1, 1, player) > 2) win = true; // Diagonal \
        }

        private int Count(int row, int col, int dRow, int dCol, int player)
        {
            int count = 0;
            int r = row + dRow;
            int c = col + dCol;
            while (r >= 0 && r < ROWS && c >= 0 && c < COLS && board[r, c] == player)
            {
                count++;
                r += dRow;
                c += dCol;
            }
            return count;
        }

        public int GetComputerMove()
        {
            // Simple AI: check if any column can make computer win
            for (int col = 0; col < COLS; col++)
            {
                int row = GetEmptyRow(col);
                if (row != -1)
                {
                    board[row, col] = COMPUTER;
                    bool wouldWin = WouldWin(COMPUTER);
                    board[row, col] = EMPTY;
                    if (wouldWin)
                        return col;
                }
            }

            // Then check if any column would let player win next turn → block it
            for (int col = 0; col < COLS; col++)
            {
                int row = GetEmptyRow(col);
                if (row != -1)
                {
                    board[row, col] = PLAYER;
                    bool wouldWin = WouldWin(PLAYER);
                    board[row, col] = EMPTY;
                    if (wouldWin)
                        return col;
                }
            }

            // Otherwise, random valid column
            Random rdm = new Random();
            int choice;
            do
            {
                choice = rdm.Next(0, COLS);
            } while (GetEmptyRow(choice) == -1);

            return choice;
        }

        private bool WouldWin(int player)
        {
            // simple scan for any win in board state
            for (int r = 0; r < ROWS; r++)
            {
                for (int c = 0; c < COLS; c++)
                {
                    if (board[r, c] == player)
                    {
                        if (Count(r, c, 1, 0, player) + Count(r, c, -1, 0, player) > 2) return true;
                        if (Count(r, c, 0, 1, player) + Count(r, c, 0, -1, player) > 2) return true;
                        if (Count(r, c, 1, 1, player) + Count(r, c, -1, -1, player) > 2) return true;
                        if (Count(r, c, 1, -1, player) + Count(r, c, -1, 1, player) > 2) return true;
                    }
                }
            }
            return false;
        }

        private int GetEmptyRow(int col)
        {
            for (int row = ROWS - 1; row >= 0; row--)
            {
                if (board[row, col] == EMPTY)
                    return row;
            }
            return -1;
        }

        public bool HasMoreMoves()
        {
            for (int c = 0; c < COLS; c++)
                if (board[0, c] == EMPTY)
                    return true;
            return false;
        }
    }
}
