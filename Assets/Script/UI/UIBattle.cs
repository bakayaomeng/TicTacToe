using Framework.Helper;
using Framework.Signal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattle : UIBase
{
    public UIPlayerInfo Player_1;
    public UIPlayerInfo Player_2;

    private void Awake()
    {
        _name = UIDefine.UIBattle;
        Hide();
        Singleton<UIManager>.Get().Add(this);

        Singleton<SignalManager>.Get().Subscribe((Signal_Battle__GameStart)Callback_Battle__GameStart);
    }

    private void Callback_Battle__GameStart()
    {
        Player_1.Init(Singleton<BattleManager>.Get().Player1);
        Player_2.Init(Singleton<BattleManager>.Get().Player2);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        Singleton<BattleManager>.Destroy();
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        Singleton<BattleManager>.Create();
    }
}
