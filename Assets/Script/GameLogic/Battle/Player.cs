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

    private int[,,] WinPath = new int[8,3,2] { 
        { { 0,0}, { 0,1}, { 0,2} },
        { { 1,0}, { 1,1}, { 1,2} },
        { { 2,0}, { 2,1}, { 2,2} },
        { { 0,0}, { 1,0}, { 2,0} },
        { { 0,1}, { 1,1}, { 2,1} },
        { { 0,2}, { 1,2}, { 2,2} },
        { { 0,0}, { 1,1}, { 2,2} },
        { { 0,2}, { 1,1}, { 2,0} }, 
    };

    public Player(string name, bool isAI)
    {
        _isAI = isAI;
        _name = name;

        Singleton<SignalManager>.Get().Subscribe((Signal_UI__OnClickPiece)Callback_UI__OnClickPiece);
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
    /// AI解算 逻辑判定优先级
    /// 1.判断自己是否可以获胜 是则直接落子取胜
    /// 2.判断对方是否可以获胜 是则直接落子防守
    /// 3.根据综合策略落子（优先达成2*2等于必胜，优先2连等于强制对方落子）
    /// </summary>
    private void AiSolver()
    {
        var btMgr = Singleton<BattleManager>.Get();
        var board = btMgr.ChessBoard;

        PieceState self = (PieceState)Role;

        //判断自己是否可以获胜 是则直接落子取胜
        for (int i = 0; i < 8; i++)
        {
            int weight = 0;
            int winX = 0;
            int wenY = 0;
            for (int j = 0; j < 3; j++)
            {
                int value = GetWeight(board, WinPath[i, j, 0], WinPath[i, j, 1]);
                weight += value;
                if(weight == 0)
                {
                    winX = i;
                    wenY = j;
                }
            }    
            if(weight == 2)
            {
                btMgr.RoundExec(this, winX, wenY);
                btMgr.RoundEnd();
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
                int value = GetWeight(board, WinPath[i, j, 0], WinPath[i, j, 1]);
                weight += value;
                if (weight == 0)
                {
                    winX = i;
                    wenY = j;
                }
            }
            if (weight == -2)
            {
                btMgr.RoundExec(this, winX, wenY);
                btMgr.RoundEnd();
                return;
            }
        }

        ////判断自己是否可以获胜 是则直接落子取胜
        //for (int i = 0; i < 3; i++)
        //{
        //    int weight = GetWeight(board, i, 0) + GetWeight(board, i, 1) + GetWeight(board, i, 2);
        //    if(weight == 2)
        //    {
        //        for (int j = 0; j < 3; j++)
        //        {
        //            if(GetWeight(board, i, j) == 0)
        //            {
        //                btMgr.RoundExec(this,i,j);
        //                btMgr.RoundEnd();
        //                return;
        //            }
        //        }
        //    }

        //    weight = GetWeight(board, 0, i) + GetWeight(board, 1, i) + GetWeight(board, 2, i);
        //    if (weight == 2)
        //    {
        //        for (int j = 0; j < 3; j++)
        //        {
        //            if (GetWeight(board, j, i) == 0)
        //            {
        //                btMgr.RoundExec(this, j, i);
        //                btMgr.RoundEnd();
        //                return;
        //            }
        //        }
        //    }
        //}

        ////判断对方是否可以获胜 是则直接落子防守
        //for (int i = 0; i < 3; i++)
        //{
        //    int weight = GetWeight(board, i, 0) + GetWeight(board, i, 1) + GetWeight(board, i, 2);
        //    if (weight == -2)
        //    {
        //        for (int j = 0; j < 3; j++)
        //        {
        //            if (GetWeight(board, i, j) == 0)
        //            {
        //                btMgr.RoundExec(this, i, j);
        //                btMgr.RoundEnd();
        //                return;
        //            }
        //        }
        //    }

        //    weight = GetWeight(board, 0, i) + GetWeight(board, 1, i) + GetWeight(board, 2, i);
        //    if (weight == -2)
        //    {
        //        for (int j = 0; j < 3; j++)
        //        {
        //            if (GetWeight(board, j, i) == 0)
        //            {
        //                btMgr.RoundExec(this, j, i);
        //                btMgr.RoundEnd();
        //                return;
        //            }
        //        }
        //    }
        //}

        //根据策略落子
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if(board[i,j] == PieceState.Empty)
                {
                    btMgr.RoundExec(this,i,j);
                    btMgr.RoundEnd();
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
