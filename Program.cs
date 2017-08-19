// Copyright: Steven Toscano 2009

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers
{
    class Program
    {
        public static string sm_mode;

        static int Main(string[] args)
        {
            sm_mode = "A";
            bool validInput = false;

            while (!validInput)
            {
                Console.Clear();
                Console.WriteLine("Please choose game play mode");
                Console.Write("'P' to play with the computer 'A' for computer auto-play, 'q' to quit: ");
                sm_mode = Console.ReadLine();
                switch (sm_mode)
                {
                    case "P":
                        validInput = true;
                        break;
                    case "A":
                        validInput = true;
                        break;
                    case "q":
                        Console.WriteLine("Goodbye.");
                        return 0;
                    default:
                        Console.WriteLine("Invalid input, press enter to continue.");
                        Console.ReadLine();
                        break;
                }
            }

            Player player1;
            Player player2;
 
            if (sm_mode == "P")
            {

                Console.Write("Please type in the name for your player: ");
                string name = Console.ReadLine();

                PieceType pieceType = PieceType.Black;
                PieceType pieceTypeOtherPlayer = PieceType.Red;

                validInput = false;
                while (!validInput)
                {
                    Console.Clear();
                    Console.Write("Please choose piece type for your player '0' or 'X', 'q' to quit: ");
                    string type = Console.ReadLine();
                    switch (type)
                    {
                        case "0":
                            pieceType = PieceType.Black;
                            pieceTypeOtherPlayer = PieceType.Red;
                            validInput = true;
                            break;
                        case "X":
                            pieceType = PieceType.Red;
                            pieceTypeOtherPlayer = PieceType.Black;
                            validInput = true;
                            break;
                        case "q":
                            Console.WriteLine("Goodbye.");
                            return 0;
                        default:
                            Console.WriteLine("Invalid input, press enter to continue.");
                            Console.ReadLine();
                            break;
                    }
                }

                player1 = new HumanPlayer(pieceType, name, WaitForInput);
                player2 = new ComputerPlayer(pieceTypeOtherPlayer, "George", WaitForInput);
            }
            else
            {
                player1 = new ComputerPlayer(PieceType.Red, "Fred", WaitForInput);
                player2 = new ComputerPlayer(PieceType.Black, "George", WaitForInput);
            }

            Console.WriteLine("Hello {0}, hit enter to start game.", player1);
            WaitForInput();

            Game checkers = new Game(player1, player2);

            int boardRowSize = 8; // 8x8 board
            int numberOfPieces = 12;

            Board gameBoard = checkers.SetupBoard(boardRowSize, numberOfPieces);

            checkers.Start();

            PrintBoard(gameBoard);

            byte turn = 0;
            TurnStatus lastTurnStatus = TurnStatus.NoPiecesWithLegalMoves;
            while (checkers.State != GameState.Over)
            {
                turn ^= 1;
                lastTurnStatus = checkers.DoTurn(turn, lastTurnStatus);

                Console.WriteLine("Hit enter for next turn.");
                WaitForInput();
            }

            if (player1.CountOfPieces == 0)
            {
                Console.WriteLine("{0} lost, {1} wins, with {2} pieces captured.", player1, player2, player2.CountOfPiecesCaptured);
                return 1;
            }
            if (player2.CountOfPieces == 0)
            {
                Console.WriteLine("{0} lost, {1} wins, with {2} pieces captured.", player2, player1, player1.CountOfPiecesCaptured);
                return 2;
            }
            if (lastTurnStatus == TurnStatus.ForfeitGame)
            {
                Console.WriteLine("Game is over.");
                return 3;
            }

            Console.WriteLine("No more legal moves, stalemate.");
            return 4;
        }

        public static void WaitForInput()
        {
            if (sm_mode == "A")
            {
                System.Threading.Thread.Sleep(50);
            }
            else
            {
                Console.ReadLine();
            }
        }

        public static void PrintBoard(Board gameBoard)
        {
            Console.Clear();
            for (int i = 0; i < gameBoard.Size; i++)
            {
                if (i % gameBoard.RowSize == 0)
                {
                    Console.Write("\n");
                }
                if (gameBoard.Square(i).fOccupied)
                {
                    if (gameBoard.Square(i).piece.Type == PieceType.Red)
                        Console.Write("X", gameBoard.Square(i).piece.Position);
                    else
                        Console.Write("0", gameBoard.Square(i).piece.Position);
                }
                else
                {
                    Console.Write(".");
                }
                Console.Write(" ");
            }
            Console.Write("\n");
        }
    }
}
