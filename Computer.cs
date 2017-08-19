// Copyright: Steven Toscano 2009

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers
{
    struct MoveResult
    {
        public Piece piece;
        public Move move;
        public MoveType type;
    };
    class ComputerPlayer : Player
    {
        public ComputerPlayer(PieceType type, string name, WaitForInput waitInput)
            : base(type, name, waitInput)
        {
        }

        public override TurnStatus MakeMove(Board board)
        {
            if (m_pieces.Count == 0)
                return TurnStatus.NoPiecesWithLegalMoves;

            /*
            // Randomize piece order
            Random rand = new Random();
            for (int i = 0; i < m_pieces.Count; i++)
            {
                int index = rand.Next() % m_pieces.Count;
                Piece tmp = m_pieces[i];
                m_pieces[i] = m_pieces[index];
                m_pieces[index] = tmp;
            }
            */

            List<MoveResult> legalMoves = new List<MoveResult>();

            // Pick the first legal move found in pieces
            Piece pieceToMove = null;
            foreach (var piece in m_pieces)
            {
                MoveType rightMove = board.TryMove(piece, Move.Right);
                MoveType leftMove = board.TryMove(piece, Move.Left);
                /*
                if (rightMove != MoveType.Illegal || leftMove != MoveType.Illegal)
                {
                    pieceToMove = piece;
                    break;
                }
                */
                if (rightMove != MoveType.Illegal)
                {
                    MoveResult m;
                    m.move = Move.Right;
                    m.type = rightMove;
                    m.piece = piece;
                    legalMoves.Add(m);
                }
                if (leftMove != MoveType.Illegal)
                {
                    MoveResult m;
                    m.move = Move.Left;
                    m.type = leftMove;
                    m.piece = piece;
                    legalMoves.Add(m);
                }
            }

            foreach (var legalMove in legalMoves)
            {
                if (legalMove.type == MoveType.LegalJump)
                    pieceToMove = legalMove.piece;
            }
            if (pieceToMove == null)
            {
                // If there are no legal jumps the pick the first legal move
                if (legalMoves.Count != 0)
                    pieceToMove = legalMoves[0].piece;
            }

            if (legalMoves.Count == 0 || pieceToMove == null)
            {
                Program.PrintBoard(board);
                Console.WriteLine("No legal moves for {0}.", this);
                return TurnStatus.NoPiecesWithLegalMoves;
            }
            else
            {
                return MakeAllLegalMoves(pieceToMove, board, false);
            }
        }

        private TurnStatus MakeAllLegalMoves(Piece pieceToMove, Board board, bool fMultiJump)
        {
            Move moveToMake = Move.None;

            if (fMultiJump)
            {
                // Pick the first legal jump found if any
                foreach (Move move in Enum.GetValues(typeof(Move)))
                {
                    if (board.TryMove(pieceToMove, move) == MoveType.LegalJump)
                    {
                        moveToMake = move;
                        break;
                    }
                    pieceToMove.ChangeDirection();
                    if (board.TryMove(pieceToMove, move) == MoveType.LegalJump)
                    {
                        moveToMake = move;
                        break;
                    }
                    pieceToMove.ChangeDirection();
                }
            }
            else
            {
                // Pick a random legal move if there are any
                MoveType rightMove = board.TryMove(pieceToMove, Move.Right);
                MoveType leftMove = board.TryMove(pieceToMove, Move.Left);
                if (rightMove != MoveType.Illegal && leftMove != MoveType.Illegal)
                {
                    Random rand = new Random();
                    moveToMake = (rand.Next() % 2 == 0) ?
                        Move.Right : Move.Left;
                }
                else if (rightMove != MoveType.Illegal || leftMove != MoveType.Illegal)
                {
                    moveToMake = rightMove != MoveType.Illegal ?
                        Move.Right : Move.Left;
                }
                else
                {
                    return TurnStatus.CompletedAllLegalMoves;
                }
            }

            if (moveToMake == Move.None)
                return TurnStatus.CompletedAllLegalMoves;

            Program.PrintBoard(board);
            Console.WriteLine("{0} going to move {1} piece at pos {2} to {3}.", this, pieceToMove.Direction, pieceToMove.Position, moveToMake);
            Console.WriteLine("Hit enter to continue.");
            m_WaitForInput();

            MoveType moveMade = board.PerformLegalMove(this, pieceToMove.Position, moveToMake);

            Program.PrintBoard(board);
            Console.Write("{0} moved {1} piece to pos {2} to {3}", this, pieceToMove.Direction, pieceToMove.Position, moveToMake);
            if (moveMade == MoveType.LegalJump)
                Console.Write(" and captured a piece");
            Console.Write(".\n");

            // Fixup the direction of the piece if it was changed for a multi-jump
            if (pieceToMove.Direction != m_direction)
                pieceToMove.ChangeDirection();

            if (moveMade == MoveType.LegalJump)
            {
                return MakeAllLegalMoves(pieceToMove, board, true);
            }
            else
            {
                return TurnStatus.CompletedAllLegalMoves;
            }
        }
    }
}
