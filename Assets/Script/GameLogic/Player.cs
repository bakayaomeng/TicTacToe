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
    /// AI����������ȼ�
    /// 1.�ж��Լ��Ƿ���Ի�ʤ ����ֱ������ȡʤ
    /// 2.�ж϶Է��Ƿ���Ի�ʤ ����ֱ�����ӷ���
    /// 3.�����ۺϲ������ӣ����ȴ��2*2���ڱ�ʤ������2������ǿ�ƶԷ����ӣ�
    /// 4.�������
    /// </summary>
    private void AiSolver()
    {
        var BattleMgr = Singleton<BattleManager>.Get();
        var board = BattleMgr.ChessBoard;

        //��һ���������
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

        //�ж��Լ��Ƿ���Ի�ʤ ����ֱ������ȡʤ
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

        //�ж϶Է��Ƿ���Ի�ʤ ����ֱ�����ӷ���
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

        //Ѱ�����2����λ��
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

        //����
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
