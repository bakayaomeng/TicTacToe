using Framework.Helper;
using Framework.Signal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBoard : MonoBehaviour
{
    public UIPiece[,] Pieces;

    private void Awake()
    {
        Pieces = new UIPiece[GameDefine.BoardSize, GameDefine.BoardSize];
        UIPiece piecePrefab = Resources.Load<UIPiece>(UIDefine.PiecePath);

        for (int i = 0; i < GameDefine.BoardSize; i++)
        {
            for (int j = 0; j < GameDefine.BoardSize; j++)
            {
                var piece = GameObject.Instantiate(piecePrefab);
                piece.name = "UIPiece_" + (i * GameDefine.BoardSize + j + 1);
                piece.transform.SetParent(this.transform);
                piece.RectTrans.anchoredPosition = new Vector2(j * 300, -i * 300);

                UIPiece uiPiece = piece.GetComponent<UIPiece>();
                uiPiece.Init(i,j);
                Pieces[i, j] = uiPiece;
            }
        }

        Singleton<SignalManager>.Get().Subscribe((Signal_Battle__BoardChange)Callback_Battle__BoardChange);
    }

    private void Callback_Battle__BoardChange(PieceState[,] chessBoard)
    {
        if(chessBoard == null || Pieces.Length != chessBoard.Length)
        {
            return;
        }

        for (int i = 0; i < GameDefine.BoardSize; i++)
        {
            for (int j = 0; j < GameDefine.BoardSize; j++)
            {
                Pieces[i, j].SetState(chessBoard[i, j]);
            }
        }
    }
}
