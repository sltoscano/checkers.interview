// Copyright: Steven Toscano 2009

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Checkers
{
    enum SquareColor
    {
        White,
        Black
    }
    struct Square
    {
        public bool fOccupied;
        public Piece piece;
        public SquareColor color;
    }

    class Board
    {
        public Board(int rowSize)
        {
            m_rowSize = rowSize;
            m_board = new Square[rowSize * rowSize];

            // Initialize the board
            byte toggleColorStart = 0;
            for (int row = 0; row < rowSize; row++)
            {
                toggleColorStart ^= 1;
                for (int col = 0; col < rowSize; col++)
                {
                    int boardPos = (row * rowSize) + col;
                    m_board[boardPos].fOccupied = false;
                    m_board[boardPos].piece = null;
                    if (toggleColorStart == 1)
                    {
                        m_board[boardPos].color = (col % 2 == 0) ? SquareColor.Black :
                            SquareColor.White;
                    }
                    else
                    {
                        m_board[boardPos].color = (col % 2 == 0) ? SquareColor.White :
                            SquareColor.Black;
                    }
                }
            }
        }

        private bool IsMoveGoingOutsideBoard(Piece piece, Move move)
        {
            return IsMoveGoingOutsideBoard(piece.Position, piece.Direction, move);
        }

        private bool IsMoveGoingOutsideBoard(int currentPos, AdvancementDirection direction, Move move)
        {
            if (move == Move.None)
                return true;

            if (direction == AdvancementDirection.Down)
            {
                if (currentPos >= (m_rowSize * m_rowSize) - m_rowSize)
                {
                    // At the bottom of the board, no move legal
                    return true;
                }
                if (currentPos % m_rowSize == 0)
                {
                    if (move == Move.Right)
                    {
                        // At the left edge of the board, right move not legal
                        return true;
                    }
                }
                else if (currentPos % m_rowSize == (m_rowSize - 1))
                {
                    if (move == Move.Left)
                    {
                        // At the right edge of the board, left move not legal
                        return true;
                    }
                }
            }
            else
            {
                if (currentPos < m_rowSize)
                {
                    // At the top of the board, no move legal
                    return true;
                }
                if (currentPos % m_rowSize == 0)
                {
                    if (move == Move.Left)
                    {
                        // At the left edge of the board, left move not legal
                        return true;
                    }
                }
                if (currentPos % m_rowSize == (m_rowSize - 1))
                {
                    if (move == Move.Right)
                    {
                        // At the right edge of the board, right move not legal
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsJumpPossible(Piece piece, Move move)
        {
            if (IsMoveGoingOutsideBoard(piece, move))
                return false;

            int newPos = GetNewPositionIfMoved(piece, move);

            if (m_board[newPos].piece.Type == piece.Type)
                return false;

            if (IsMoveGoingOutsideBoard(newPos, piece.Direction, move))
                return false;

            int posAfterJump = GetNewPositionIfMoved(piece.Direction, newPos, move);

            if (m_board[posAfterJump].fOccupied)
                return false;

            return true;
        }

        private int GetNewPositionIfMoved(Piece piece, Move move)
        {
            return GetNewPositionIfMoved(piece.Direction, piece.Position, move);
        }

        private int GetNewPositionIfMoved(AdvancementDirection direction, int position, Move move)
        {
            int iHorizontalOffset = (direction == AdvancementDirection.Down) ?
                m_rowSize : -m_rowSize;
            int iVerticalOffset = (direction == AdvancementDirection.Down) ?
                (move == Move.Right) ? -1 : 1 :
                (move == Move.Left) ? -1 : 1;

            return position + iHorizontalOffset + iVerticalOffset;
        }

        public MoveType TryMove(Piece pieceToMove, Move move)
        {
            if (IsMoveGoingOutsideBoard(pieceToMove, move))
                return MoveType.Illegal;

            int newPos = GetNewPositionIfMoved(pieceToMove, move);
            if (m_board[newPos].fOccupied)
            {
                // If opponent piece the check if a jump is legal
                if (IsJumpPossible(pieceToMove, move))
                {
                    return MoveType.LegalJump;
                }
                else
                {
                    return MoveType.Illegal;
                }
            }
            else
            {
                return MoveType.Legal;
            }
        }

        public MoveType PerformLegalMove(Player player, int fromPosition, Move move)
        {
            Piece piece = RemovePiece(fromPosition);

            int newPos = GetNewPositionIfMoved(piece.Direction, fromPosition, move);

            if (m_board[newPos].fOccupied)
            {
                if (m_board[newPos].piece.Type == piece.Type)
                {
                    Debug.Assert(false, "Making this jump is NOT legal, call TryMove first");
                    return MoveType.Illegal;
                }
                else
                {
                    Piece jumpedPiece = RemovePiece(newPos);
                    player.Opponent.LosePiece(jumpedPiece);

                    int newJumpPos = GetNewPositionIfMoved(piece.Direction, newPos, move);
                    PlacePiece(newJumpPos, piece);
                    player.CapturePiece(jumpedPiece);

                    return MoveType.LegalJump;
                }
            }
            else
            {
                PlacePiece(newPos, piece);
                return MoveType.Legal;
            }
        }

        public void PlacePiece(int pos, Piece piece)
        {
            Debug.Assert(pos < m_board.Length);
            Debug.Assert(m_board[pos].fOccupied == false);
            Debug.Assert(m_board[pos].piece == null);
            piece.Position = pos;
            m_board[pos].piece = piece;
            m_board[pos].fOccupied = true;
        }
        public Piece RemovePiece(int pos)
        {
            Debug.Assert(pos < m_board.Length);
            Debug.Assert(m_board[pos].fOccupied == true);
            Piece pieceToMove = m_board[pos].piece;
            m_board[pos].fOccupied = false;
            m_board[pos].piece = null;
            pieceToMove.Position = Position.Empty;
            return pieceToMove;
        }
        public Piece GetPiece(int pos)
        {
            return m_board[pos].piece;
        }
        public Square Square(int pos)
        {
            return m_board[pos];
        }
        public int Size
        {
            get { return m_board.Length; }
        }
        public int RowSize
        {
            get { return m_rowSize; }
        }

        private Square[] m_board;
        private int m_rowSize;
    }
}
