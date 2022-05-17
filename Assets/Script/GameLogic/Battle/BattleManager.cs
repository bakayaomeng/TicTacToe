using Framework.Helper;
using Framework.Signal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : IDisposable
{
    private GameModel _gameModel;
    private PieceState[,] _chessBoard;

    private bool _isPlaying;
    private int _boardSize;
    private int _curRound;
    private int _maxRound;

    private Player Player1;
    private Player Player2;

    public BattleManager()
    {
        _boardSize = GameDefine.BoardSize;
        _maxRound = _boardSize * _boardSize;
        _chessBoard = new PieceState[_boardSize,_boardSize];
        
        Singleton<SignalManager>.Get().Subscribe((Signal_Battle__Play)Callback_Battle__Play);
        Singleton<SignalManager>.Get().Subscribe((Signal_Battle__Exec)Callback_Battle__Exec);
        Singleton<SignalManager>.Get().Subscribe((Signal_Battle__RePlay)Callback_Battle__RePlay);
    }

    public void Dispose()
    {
        Singleton<SignalManager>.Get().Unsubscribe((Signal_Battle__Exec)Callback_Battle__Exec);
        Singleton<SignalManager>.Get().Unsubscribe((Signal_Battle__RePlay)Callback_Battle__RePlay);
    }
   
    private void Judge()
    {
        if (_isPlaying == false)
        {
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            if(_chessBoard[i, 0] != PieceState.Empty && _chessBoard[i,0] == _chessBoard[i,1] && _chessBoard[i, 1] == _chessBoard[i, 2])
            {
                Singleton<SignalManager>.Get().Find<Signal_Battle__GameEnd>()?.Invoke(Player1);
            }
            if (_chessBoard[0, i] != PieceState.Empty && _chessBoard[0, i] == _chessBoard[1, i] && _chessBoard[0, i] == _chessBoard[2, i])
            {
                Singleton<SignalManager>.Get().Find<Signal_Battle__GameEnd>()?.Invoke(Player2);
            }
        }

        if(_chessBoard[0, 0] != PieceState.Empty && _chessBoard[0, 0] == _chessBoard[1, 1] && _chessBoard[0, 0] == _chessBoard[2, 2] ||
           _chessBoard[2, 0] != PieceState.Empty && _chessBoard[2, 0] == _chessBoard[1, 1] && _chessBoard[2, 0] == _chessBoard[0, 2])
        {
            Singleton<SignalManager>.Get().Find<Signal_Battle__GameEnd>()?.Invoke(Player1);
        }

        if(_curRound == _maxRound)
        {
            Singleton<SignalManager>.Get().Find<Signal_Battle__GameEnd>()?.Invoke(null);
        }
    }

    private void BattleStart()
    {
        RefershChessBoard();
        _curRound = 0;
        _isPlaying = true;
    }

    private void RefershChessBoard()
    {
        for (int i = 0; i < _boardSize; i++)
        {
            for (int j = 0; j < _boardSize; j++)
            {
                _chessBoard[i, j] = PieceState.Empty;
            }
        }
        Singleton<SignalManager>.Get().Find<Signal_Battle__BoardChange>()?.Invoke(_chessBoard);
    }

    private void Callback_Battle__Exec(int x, int y)
    {
        if(_isPlaying == false)
        {
            return;
        }

        if(_gameModel == GameModel.Pve)
        {
            if ((Player2.Role == Role.Attacker && _curRound % 2 == 0) || 
                (Player2.Role == Role.Defender && _curRound % 2 == 1))
            {
                return;
            }
        }

        if (_chessBoard[x, y] != PieceState.Empty)
        {
            return;
        }

        if(_curRound % 2 == 0)
        {
            _chessBoard[x, y] = PieceState.Defender;
        }
        else
        {
            _chessBoard[x, y] = PieceState.Attacker;
        }

        _curRound++;

        Singleton<SignalManager>.Get().Find<Signal_Battle__BoardChange>()?.Invoke(_chessBoard);

        Judge();
    }

    private void Callback_Battle__RePlay()
    {
        BattleStart();
    }

    private void Callback_Battle__Play(GameModel gameModel)
    {
        _gameModel = gameModel;

        if (gameModel == GameModel.Pvp)
        {
            Player1 = new Player("玩家1", false);
            Player2 = new Player("玩家2", false);
        }
        else if (gameModel == GameModel.Pve)
        {
            Player1 = new Player("玩家", false);
            Player2 = new Player("电脑", true);
        }
        
        BattleStart();

        Debug.Log("Began to play , game model :" + gameModel.ToString());
    }
}
