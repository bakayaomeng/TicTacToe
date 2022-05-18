using Framework.Helper;
using Framework.Signal;
using System;

public class Player : IDisposable
{
    private string _name;
    private bool _isAI;
    private bool _isManual;

    public Role Role;
    public string Name => _name;

    private int[,,] _winPath = new int[8,3,2] { 
        { { 0,0}, { 0,1}, { 0,2} },
        { { 1,0}, { 1,1}, { 1,2} },
        { { 2,0}, { 2,1}, { 2,2} },
        { { 0,0}, { 1,0}, { 2,0} },
        { { 0,1}, { 1,1}, { 2,1} },
        { { 0,2}, { 1,2}, { 2,2} },
        { { 0,0}, { 1,1}, { 2,2} },
        { { 0,2}, { 1,1}, { 2,0} }, 
    };

    public Player()
    {
        Singleton<SignalManager>.Get().Subscribe((Signal_UI__OnClickPiece)Callback_UI__OnClickPiece);
    }

    public void Init(string name, bool isAI)
    {
        _isAI = isAI;
        _name = name;
    }

    public void Dispose()
    {
        Singleton<SignalManager>.Get().Unsubscribe((Signal_UI__OnClickPiece)Callback_UI__OnClickPiece);
    }

    public void AllocationRound()
    {
        if(!_isAI)
        {
            _isManual = true;
        }
        else
        {
            AiSolver();
        }
    }

    public void BattleStart(Role role)
    {
        Role = role;
    }

    /// <summary>
    /// AI出棋策略优先级
    /// 1.判断自己是否可以获胜 是则直接落子取胜
    /// 2.判断对方是否可以获胜 是则直接落子防守
    /// 3.根据综合策略落子（优先达成2*2等于必胜，优先2连等于强制对方落子）
    /// 4.保底填空
    /// </summary>
    private void AiSolver()
    {
        var BattleMgr = Singleton<BattleManager>.Get();
        var board = BattleMgr.ChessBoard;

        //第一手随机出棋
        if(BattleMgr.CurRound == 0)
        {
            int x = UnityEngine.Random.Range(0, 300) /100;
            int y = UnityEngine.Random.Range(0, 300) / 100;
            if(BattleMgr.RoundExec(this, x, y))
            {
                BattleMgr.RoundEnd();
                return;
            }
        }

        //判断自己是否可以获胜 是则直接落子取胜
        for (int i = 0; i < 8; i++)
        {
            int weight = 0;
            int winX = 0;
            int wenY = 0;
            for (int j = 0; j < 3; j++)
            {
                int value = GetWeight(board, _winPath[i, j, 0], _winPath[i, j, 1]);
                weight += value;
                if(value == 0)
                {
                    winX = _winPath[i, j, 0];
                    wenY = _winPath[i, j, 1];
                }
            }    
            if(weight == 2 && BattleMgr.RoundExec(this, winX, wenY))
            {
                BattleMgr.RoundEnd();
                return;
            }
        }

        //判断对方是否可以获胜 是则直接落子防守
        for (int i = 0; i < 8; i++)
        {
            int weight = 0;
            int winX = 0;
            int wenY = 0;
            for (int j = 0; j < 3; j++)
            {
                int value = GetWeight(board, _winPath[i, j, 0], _winPath[i, j, 1]);
                weight += value;
                if (value == 0)
                {
                    winX = _winPath[i, j, 0];
                    wenY = _winPath[i, j, 1];
                }
            }
            if (weight == -2 && BattleMgr.RoundExec(this, winX, wenY))
            {
                BattleMgr.RoundEnd();
                return;
            }
        }

        //寻找组成2连的位置
        for (int i = 0; i < 8; i++)
        {
            int weight = 0;
            int winX = 0;
            int wenY = 0;
            for (int j = 0; j < 3; j++)
            {
                int value = GetWeight(board, _winPath[i, j, 0], _winPath[i, j, 1]);
                weight += value;
                if (value == 0)
                {
                    winX = _winPath[i, j, 0];
                    wenY = _winPath[i, j, 1];
                }
            }
            if (weight == 1)
            {
                if(BattleMgr.RoundExec(this, winX, wenY))
                {
                    BattleMgr.RoundEnd();
                    return;
                }
            }
        }

        //保底
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == PieceState.Empty && BattleMgr.RoundExec(this, i, j))
                {
                    BattleMgr.RoundEnd();
                    return;
                }
            }
        }
    }

    private int GetWeight(PieceState[,] ChessBoard , int x , int y)
    {
        if(ChessBoard[x,y] == PieceState.Empty)
        {
            return 0;
        }
        if (ChessBoard[x, y] == (PieceState)Role)
        {
            return 1;
        }
        else
        {
            return -1;
        }      
    }

    private void Callback_UI__OnClickPiece(int x, int y)
    {
        var battleMgr = Singleton<BattleManager>.Get();
        if(!_isAI && _isManual)
        {
            if (battleMgr.RoundExec(this, x, y))
            {
                _isManual = false;
                battleMgr.RoundEnd();
            }
        }
    }
}
