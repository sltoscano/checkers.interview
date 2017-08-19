// Copyright: Steven Toscano 2009

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Checkers
{
    enum GameState
    {
        Starting,
        Playing,
        Over
    }

    class Game
    {
        Board m_board;
        GameState m_state;
        Player m_player1;
        Player m_player2;
        int m_numberOfPieces;

        public Game(Player player1, Player player2)
        {
            m_player1 = player1;
            m_player2 = player2;
        }
        public Board SetupBoard(int rowSize, int numberOfPieces)
        {
            Debug.Assert(rowSize > 2);
            m_board = new Board(rowSize);
            m_state = GameState.Starting;
            m_numberOfPieces = numberOfPieces;
            return m_board;
        }
        public GameState State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public TurnStatus DoTurn(byte turn, TurnStatus lastTurnStatus)
        {
            m_state = GameState.Playing;

            //turn ^= 1;
            if (m_player1.CountOfPieces == 0)
            {
                this.State = GameState.Over;
            }
            else if (m_player2.CountOfPieces == 0)
            {
                this.State = GameState.Over;
            }
            else
            {
                Player currentPlayer = (turn == 1) ? m_player1 : m_player2;
                TurnStatus turnStatus = currentPlayer.MakeMove(m_board);
                if (turnStatus == TurnStatus.NoPiecesWithLegalMoves &&
                    lastTurnStatus == TurnStatus.NoPiecesWithLegalMoves)
                {
                    this.State = GameState.Over;
                }
                lastTurnStatus = turnStatus;
            }

            if (lastTurnStatus == TurnStatus.ForfeitGame)
            {
                this.State = GameState.Over;
            }

            return lastTurnStatus;
        }

        public void Start()
        {
            m_state = GameState.Starting;

            m_player1.PlacePieces(m_numberOfPieces, m_board);
            m_player2.PlacePieces(m_numberOfPieces, m_board);

            m_player1.Opponent = m_player2;
            m_player2.Opponent = m_player1;
        }
    }
}
