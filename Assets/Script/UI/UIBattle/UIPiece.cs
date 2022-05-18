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
    public Image ImageX;
    public Image ImageO;

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
                ImageX.gameObject.SetActive(false);
                ImageO.gameObject.SetActive(false);
                break;
            case PieceState.Attacker:
                ImageX.gameObject.SetActive(false);
                ImageO.gameObject.SetActive(true);
                break;
            case PieceState.Defender:
                ImageX.gameObject.SetActive(true);
                ImageO.gameObject.SetActive(false);
                break;
        }
    }

    private void OnClick()
    {
        Singleton<SignalManager>.Get().Find<Signal_UI__OnClickPiece>()?.Invoke(_indexX, _indexY);
    }
}
