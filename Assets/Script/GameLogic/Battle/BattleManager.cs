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

    private Player Attacker;
    private Player Defender;

    public int CurRound => _curRound;
    public GameModel GameModel => _gameModel;
    public PieceState[,] ChessBoard => _chessBoard;

    public BattleManager()
    {
        _boardSize = GameDefine.BoardSize;
        _maxRound = _boardSize * _boardSize;
        _chessBoard = new PieceState[_boardSize,_boardSize];
        
        Singleton<SignalManager>.Get().Subscribe((Signal_Battle__Play)Callback_Battle__Play);
        Singleton<SignalManager>.Get().Subscribe((Signal_Battle__RePlay)Callback_Battle__RePlay);
    }

    public void Dispose()
    {
        Singleton<SignalManager>.Get().Unsubscribe((Signal_Battle__RePlay)Callback_Battle__RePlay);
    }

    public bool RoundExec(Player player, int x, int y)
    {
        if (_isPlaying == false || player == null || _chessBoard[x, y] != PieceState.Empty)
        {
            return false;
        }

        _chessBoard[x, y] = (PieceState)player.Role;
        Singleton<SignalManager>.Get().Find<Signal_Battle__BoardChange>()?.Invoke(_chessBoard);

        //胜负判定 
        if (Judge(player))
        {
            //游戏结束
            BattleEnd();
        }
        return true;
    }

    public void RoundEnd()
    {
        if (!_isPlaying)
        {
            return;
        }

        _curRound++;
        if (_curRound % 2 == 0)
        {
            Attacker.AllocationRound();
        }
        else
        {
            Defender.AllocationRound();
        }
    }

    private bool Judge(Player player)
    {
        if (_isPlaying == false)
        {
            return false;
        }

        for (int i = 0; i < 3; i++)
        {
            if(_chessBoard[i, 0] != PieceState.Empty && _chessBoard[i, 0] == (PieceState)player.Role &&
                _chessBoard[i,0] == _chessBoard[i,1] && _chessBoard[i, 1] == _chessBoard[i, 2])
            {
                Singleton<SignalManager>.Get().Find<Signal_Battle__GameEnd>()?.Invoke(player);
                return true;
            }
            if (_chessBoard[0, i] != PieceState.Empty && _chessBoard[0, i] == (PieceState)player.Role &&
                _chessBoard[0, i] == _chessBoard[1, i] && _chessBoard[0, i] == _chessBoard[2, i])
            {
                Singleton<SignalManager>.Get().Find<Signal_Battle__GameEnd>()?.Invoke(player);
                return true;
            }
        }

        if(_chessBoard[0, 0] != PieceState.Empty && _chessBoard[0, 0] == (PieceState)player.Role &&
            _chessBoard[0, 0] == _chessBoard[1, 1] && _chessBoard[0, 0] == _chessBoard[2, 2] ||
           _chessBoard[2, 0] != PieceState.Empty && _chessBoard[2, 0] == (PieceState)player.Role &&
           _chessBoard[2, 0] == _chessBoard[1, 1] && _chessBoard[2, 0] == _chessBoard[0, 2])
        {
            Singleton<SignalManager>.Get().Find<Signal_Battle__GameEnd>()?.Invoke(player);
            return true;
        }

        if(_curRound == _maxRound - 1)
        {
            Singleton<SignalManager>.Get().Find<Signal_Battle__GameEnd>()?.Invoke(null);
            return true;
        }

        return false;
    }

    private void BattleStart()
    {
        RefershChessBoard();
        _curRound = 0;
        _isPlaying = true;

        if(_gameModel == GameModel.Pvp)
        {
            Player1.BattleStart(Role.Attacker);
            Player2.BattleStart(Role.Defender);
        }
        else
        {
            int val = UnityEngine.Random.Range(0,100);
            if(val > 50)
            {
                Attacker = Player1;
                Defender = Player2;
                Player1.BattleStart(Role.Attacker);
                Player2.BattleStart(Role.Defender);
            }
            else
            {
                Attacker = Player2;
                Defender = Player1;
                Player1.BattleStart(Role.Defender);
                Player2.BattleStart(Role.Attacker);
            }
        }

        Attacker.AllocationRound();
    }

    private void BattleEnd()
    {
        _isPlaying = false;
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
