using Framework.Helper;
using Framework.Signal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPiece : MonoBehaviour
{  
    public Image Image;
    public Button Button;
    public RectTransform RectTrans;

    private int _indexX;
    private int _indexY;

    public void Init(int x , int y)
    {
        _indexX = x;
        _indexY = y;

        Button.onClick.AddListener(OnClick);
    }

    public void SetState(PieceState state)
    {
        switch (state)
        {
            case PieceState.Empty:
                Image.color = Color.white;
                break;
            case PieceState.Attacker:
                Image.color = Color.red;
                break;
            case PieceState.Defender:
                Image.color = Color.green;
                break;
        }
    }

    private void OnClick()
    {
        Singleton<SignalManager>.Get().Find<Signal_Battle__Exec>()?.Invoke(_indexX, _indexY);
    }
}
