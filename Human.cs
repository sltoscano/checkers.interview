// Copyright: Steven Toscano 2009

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers
{
    class HumanPlayer : Player
    {
        public HumanPlayer(PieceType type, string name, WaitForInput waitInput)
            : base(type, name, waitInput)
        {
        }

        public override TurnStatus MakeMove(Board board)
        {
            m_board = board;
            return MakeAllLegalMoves(false);
        }

        private TurnStatus MakeAllLegalMoves(bool fMultiJump)
        {
            bool legalMovesLeft = false;
            List<Piece> pieces;

            // If the last move was a jump, only legal moves
            // are allowed from that same piece
            if (fMultiJump)
            {
                pieces = new List<Piece>();
                pieces.Add(m_lastMovedPiece);
            }
            else
            {
                pieces = m_pieces;
            }

            // Check for any legal moves
            foreach (var piece in pieces)
            {
                if (legalMovesLeft)
                    break;

                foreach (Move move in Enum.GetValues(typeof(Move)))
                {
                    if (fMultiJump)
                    {
                        // It is legal to continue doing only jumps
                        MoveType result = m_board.TryMove(piece, move);
                        if (result == MoveType.LegalJump)
                        {
                            legalMovesLeft = true;
                            break;
                        }
                        piece.ChangeDirection();
                        result = m_board.TryMove(piece, move);
                        piece.ChangeDirection();
                        if (result == MoveType.LegalJump)
                        {
                            legalMovesLeft = true;
                            break;
                        }
                    }
                    else
                    {
                        if (m_board.TryMove(piece, move) != MoveType.Illegal)
                        {
                            legalMovesLeft = true;
                            break;
                        }
                    }
                }
            }

            if (!legalMovesLeft)
            {
                Console.WriteLine("You have no more legal moves");
                return TurnStatus.NoPiecesWithLegalMoves;
            }

            MoveType resultingMoveType;
            TurnStatus turn = InputLoop(fMultiJump, out resultingMoveType);

            if (turn == TurnStatus.CompletedAllLegalMoves)
            {
                if (resultingMoveType == MoveType.LegalJump)
                {
                    // Recurse and check if there are more legal jumps
                    MakeAllLegalMoves(true);
                }
            }
            else
            {
                Console.WriteLine(", hit enter to continue.");
                Console.ReadLine();
            }
            return turn;
        }

        private TurnStatus InputLoop(bool fMultiJump, out MoveType resultingMoveType)
        {
            m_lastMovedPiece = null;
            resultingMoveType = MoveType.Illegal;
            TurnStatus turn = TurnStatus.NoPiecesWithLegalMoves;

            Console.Write("Your turn");

            Piece pieceToMove = null;
            bool legalSelectionMade = false;
            while (!legalSelectionMade)
            {
                Console.WriteLine(", hit enter to continue.");
                Console.ReadLine();
                Console.Clear();
                Program.PrintBoard(m_board);

                if (fMultiJump)
                    Console.WriteLine("You have more legal moves possible");

                Console.WriteLine("{0}, type in the column and row of the piece position to move", this);
                Console.Write("For example '{1},{2}' is bottom rightmost, or 'q' to quit: ", this, m_board.RowSize-1, m_board.RowSize-1);
                string input = Console.ReadLine();
                if (input == "q")
                {
                    Console.Write("Quitting");
                    turn = TurnStatus.ForfeitGame;
                    legalSelectionMade = true;
                    continue;
                }
                string[] position = input.Split(',');
                if (position.Length != 2)
                {
                    Console.Write("Input error");
                    continue;
                }
                int iCol = Int32.Parse(position[0]);
                int iRow = Int32.Parse(position[1]);
                int iPos = (iRow * m_board.RowSize) + iCol;
                if (iPos >= m_board.Size || iRow < 0 || iCol < 0)
                {
                    Console.Write("Not a valid position on the board");
                    continue;
                }
                pieceToMove = m_board.GetPiece(iPos);
                if (pieceToMove == null)
                {
                    Console.Write("There is no piece at that position");
                    continue;
                }
                if (pieceToMove.Type != m_type)
                {
                    Console.Write("That is not your piece");
                    continue;
                }
                Console.WriteLine("Type in the number for your move selection:");
                Console.Write("up_left(1),up_right(2),down_left(3),down_right(4) or 'q' to quit: ");
                input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        resultingMoveType = DoMove(fMultiJump, pieceToMove, AdvancementDirection.Up, Move.Left);                        
                        break;
                    case "2":
                        resultingMoveType = DoMove(fMultiJump, pieceToMove, AdvancementDirection.Up, Move.Right);
                        break;
                    case "3":
                        resultingMoveType = DoMove(fMultiJump, pieceToMove, AdvancementDirection.Down, Move.Left);
                        break;
                    case "4":
                        resultingMoveType = DoMove(fMultiJump, pieceToMove, AdvancementDirection.Down, Move.Right);
                        break;
                    case "q":
                        turn = TurnStatus.ForfeitGame;
                        legalSelectionMade = true;
                        break;
                    default:
                        Console.WriteLine("Not a valid selection");
                        break;
                }

                if (resultingMoveType == MoveType.Illegal)
                {
                    Console.Write("Invalid move");
                }
                else
                {
                    turn = TurnStatus.CompletedAllLegalMoves;
                    legalSelectionMade = true;
                }
            }

            if (turn == TurnStatus.CompletedAllLegalMoves)
                m_lastMovedPiece = pieceToMove;
            return turn;
        }

        private MoveType DoMove(bool fMultiJump, Piece pieceToMove, AdvancementDirection direction, Move move)
        {
            MoveType resultingMoveType = MoveType.Illegal;

            if (fMultiJump)
            {
                // If the user wants to switch direction only allow in a multi-jump
                if (pieceToMove.Direction != direction)
                    pieceToMove.ChangeDirection();
            }
            if (m_direction != direction && !fMultiJump)
            {
                return MoveType.Illegal;
            }
            else if (m_board.TryMove(pieceToMove, move) == MoveType.Illegal)
            {
                return MoveType.Illegal;
            }

            resultingMoveType = m_board.PerformLegalMove(this, pieceToMove.Position, move);

            // Fixup direction if switched for a multi-jump
            if (pieceToMove.Direction != m_direction)
                pieceToMove.ChangeDirection();

            return resultingMoveType;
        }

        private Piece m_lastMovedPiece;
        private Board m_board;
    }
}
