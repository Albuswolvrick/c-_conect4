using Microsoft.VisualBasic;
using System.CodeDom;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class GameForm : Form
    {
        private Rectangle[] boardColumns;
        private int[,] board;
        private int turn;
        private enum GameMode { PlayerVsPlayer, PlayerVsComputer }
        private GameMode currentMode = GameMode.PlayerVsPlayer;
        private GameLogic gameLogic;

        public GameForm()
        {
            InitializeComponent();
            DoubleBuffered = true; // stops flicker and redraw lag

            this.boardColumns = new Rectangle[10];
            this.board = new int[6, 10];
            this.turn = 1;
            this.gameLogic = new GameLogic();

            // Create column hitboxes ONCE (not in Paint)
            for (int j = 0; j < boardColumns.Length; j++)
            {
                this.boardColumns[j] = new Rectangle(32 + 48 * j, 24, 32, 300);
            }

            // Ask once at start
            DialogResult result = MessageBox.Show("Do you want to play against Computer?", "Game Mode", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                currentMode = GameMode.PlayerVsComputer;
        }

        // Override MouseClick so it fires once reliably
        private void GameForm_MouseClick(object sender, MouseEventArgs e)
        {
            int columnIndex = this.ColumnNumber(e.Location);
            if (columnIndex == -1) return;

            if (!gameLogic.Move(columnIndex, turn)) return;

            this.Invalidate();

            if (gameLogic.IsWin)
            {
                string winner = (turn == 1) ? "Red" : "Yellow";
                MessageBox.Show($"Player {winner} wins!", "Game Over");
                this.Close();
                return;
            }

            if (!gameLogic.HasMoreMoves())
            {
                MessageBox.Show("It's a draw!", "Game Over");
                this.Close();
                return;
            }

            turn = (turn == 1) ? 2 : 1;

            if (currentMode == GameMode.PlayerVsComputer && turn == 2)
            {
                int compCol = gameLogic.GetComputerMove();
                gameLogic.Move(compCol, 2);
                this.Invalidate();

                if (gameLogic.IsWin)
                {
                    MessageBox.Show("Computer wins!", "Game Over");
                    this.Close();
                    return;
                }

                if (!gameLogic.HasMoreMoves())
                {
                    MessageBox.Show("It's a draw!", "Game Over");
                    this.Close();
                    return;
                }

                turn = 1;
            }
        }


        // Cleaned logic from your MouseClick
        private void HandleClick(Point location)
        {
            int columnIndex = this.ColumnNumber(location);
            if (columnIndex == -1) return;

            if (!gameLogic.Move(columnIndex, turn)) return;

            this.Invalidate(); // refresh drawing

            if (gameLogic.IsWin)
            {
                string winner = (turn == 1) ? "Red" : "Yellow";
                MessageBox.Show($"Player {winner} wins!", "Game Over");
                this.Close();
                return;
            }

            if (!gameLogic.HasMoreMoves())
            {
                MessageBox.Show("It's a draw!", "Game Over");
                this.Close();
                return;
            }

            // Switch turns
            turn = (turn == 1) ? 2 : 1;

            // --- COMPUTER MOVE ---
            if (currentMode == GameMode.PlayerVsComputer && turn == 2)
            {
                int compCol = gameLogic.GetComputerMove();
                gameLogic.Move(compCol, 2);
                this.Invalidate();

                if (gameLogic.IsWin)
                {
                    MessageBox.Show("Computer wins!", "Game Over");
                    this.Close();
                    return;
                }

                if (!gameLogic.HasMoreMoves())
                {
                    MessageBox.Show("It's a draw!", "Game Over");
                    this.Close();
                    return;
                }

                turn = 1;
            }
        }

        // Drawing only reads from board state (no re-init)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.FillRectangle(Brushes.CornflowerBlue, 24, 24, 340, 300);

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    int cell = gameLogic.Board[row, col];
                    Brush brush = Brushes.White;
                    if (cell == 1) brush = Brushes.Red;
                    else if (cell == 2) brush = Brushes.Yellow;

                    g.FillEllipse(brush, 32 + 48 * col, 32 + 48 * row, 32, 32);
                }
            }
        }

        private int ColumnNumber(Point mouse)
        {
            for (int i = 0; i < this.boardColumns.Length; i++)
            {
                if (this.boardColumns[i].Contains(mouse))
                    return i;
            }
            return -1;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        private int EmptyRow(int col)
        {
            for (int row = 5; row >= 0; row--)
            {
                if (this.board[row, col] == 0)
                    return row;
            }
            return -1;
        }

        private bool AllNumbersEqual(int toCheck, params int[] numbers)
        {
            foreach (int number in numbers)
            {
                if (number != toCheck || number == 0)
                    return false;
            }
            return true;
        }

        private int WinnerPlayer(int playerToCheck)
        {
            // vertical
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            {
                for (int col = 0; col < this.board.GetLength(1); col++)
                {
                    if (this.AllNumbersEqual(playerToCheck,
                        this.board[row, col],
                        this.board[row + 1, col],
                        this.board[row + 2, col],
                        this.board[row + 3, col]))
                        return playerToCheck;
                }
            }

            // horizontal
            for (int row = 0; row < this.board.GetLength(0); row++)
            {
                for (int col = 0; col < this.board.GetLength(1) - 3; col++)
                {
                    if (this.AllNumbersEqual(playerToCheck,
                        this.board[row, col],
                        this.board[row, col + 1],
                        this.board[row, col + 2],
                        this.board[row, col + 3]))
                        return playerToCheck;
                }
            }

            // diagonal ↘
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            {
                for (int col = 0; col < this.board.GetLength(1) - 3; col++)
                {
                    if (this.AllNumbersEqual(playerToCheck,
                        this.board[row, col],
                        this.board[row + 1, col + 1],
                        this.board[row + 2, col + 2],
                        this.board[row + 3, col + 3]))
                        return playerToCheck;
                }
            }

            // diagonal ↙
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            {
                for (int col = 3; col < this.board.GetLength(1); col++)
                {
                    if (this.AllNumbersEqual(playerToCheck,
                        this.board[row, col],
                        this.board[row + 1, col - 1],
                        this.board[row + 2, col - 2],
                        this.board[row + 3, col - 3]))
                        return playerToCheck;
                }
            }

            return -1;
        }
    }
}
