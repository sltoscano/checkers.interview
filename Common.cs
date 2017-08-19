// Copyright: Steven Toscano 2009

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers
{
    struct Position
    {
        public const int Empty = -1;
    }
    
    enum Move
    {
        Left,
        Right,
        None
    }
    enum MoveType
    {
        Legal,
        Illegal,
        LegalJump
    }
    enum TurnStatus
    {
        NoPiecesWithLegalMoves,
        CompletedAllLegalMoves,
        ForfeitGame
    }
    enum AdvancementDirection
    {
        Up,
        Down
    }
    enum PieceType
    {
        Red,
        Black
    }

    class Piece
    {
        public Piece(int pos, AdvancementDirection direction, PieceType type)
        {
            m_type = type;
            m_position = pos;
            m_direction = direction;
        }
        private int m_position;
        private AdvancementDirection m_direction;
        PieceType m_type;

        public PieceType Type
        {
            get { return m_type; }
        }
        public int Position
        {
            get { return m_position; }
            set { m_position = value; }
        }
        public AdvancementDirection Direction
        {
            get { return m_direction; }
        }
        public void ChangeDirection()
        {
            if (m_direction == AdvancementDirection.Down)
                m_direction = AdvancementDirection.Up;
            else
                m_direction = AdvancementDirection.Down;
        }
    }
}
