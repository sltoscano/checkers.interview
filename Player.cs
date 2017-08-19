// Copyright: Steven Toscano 2009

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Checkers
{
    public delegate void WaitForInput();

    abstract class Player
    {
        protected List<Piece> m_pieces;
        protected List<Piece> m_capturedPieces;
        protected readonly PieceType m_type;
        protected readonly AdvancementDirection m_direction;
        protected readonly string m_name;
        protected Player m_opponent;
        protected WaitForInput m_WaitForInput;
        

        protected Player(PieceType type, string name, WaitForInput waitInput)
        {
            m_name = name;
            m_type = type;
            m_WaitForInput = waitInput;
            if (m_type == PieceType.Red)
                m_direction = AdvancementDirection.Down;
            else if (m_type == PieceType.Black)
                m_direction = AdvancementDirection.Up;
            m_pieces = new List<Piece>();
            m_capturedPieces = new List<Piece>();
        }
        public void LosePiece(Piece piece)
        {
            Debug.Assert(piece.Type == m_type);
            m_pieces.Remove(piece);
        }
        public void CapturePiece(Piece piece)
        {
            Debug.Assert(piece.Type != m_type);
            m_capturedPieces.Add(piece);
        }
        public void PlacePieces(int numberOfPlayerPieces, Board board)
        {
            Debug.Assert(numberOfPlayerPieces > 1);
            Debug.Assert(numberOfPlayerPieces*2 < board.Size);

            // Start placing pieces depending on orientation
            // for an 8x8 board top player starts at 1 and bottom 
            // player starts at (8*8) - (12*2) = 40

            int startFillPosition;
            if (m_direction == AdvancementDirection.Down)
                startFillPosition = 0;
            else
                startFillPosition = board.Size - (numberOfPlayerPieces * 2);

            int countPlacedPieces = 0;  
            for (int i = startFillPosition; i < board.Size; i++)
            {
                if (board.Square(i).color == SquareColor.White)
                {
                    Piece piece = new Piece(i, m_direction, m_type);
                    m_pieces.Add(piece);
                    board.PlacePiece(i, piece);
                    countPlacedPieces++;
                    if (countPlacedPieces >= numberOfPlayerPieces)
                        break;
                }
            }
        }
        public string Name
        {
            get { return m_name; }
        }
        public Player Opponent
        {
            get { return m_opponent; }
            set { m_opponent = value; }
        }
        public int CountOfPieces
        {
            get { return m_pieces.Count; }
        }
        public int CountOfPiecesCaptured
        {
            get { return m_capturedPieces.Count; }
        }
        public PieceType Type
        {
            get { return m_type; }
        }
        public override string ToString()
        {
 	         return this.Name;
        }

        abstract public TurnStatus MakeMove(Board board);
    }

    

}
