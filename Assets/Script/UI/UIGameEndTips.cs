using Framework.Helper;
using Framework.Signal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameEndTips : UIBase
{
    public Button BtnRestart;
    public Button BtnReturn;
    public Text TipsText;

    public void Awake()
    {
        BtnRestart.onClick.AddListener(Restart);
        BtnReturn.onClick.AddListener(Return);

        Singleton<SignalManager>.Get().Subscribe((Signal_Battle__GameEnd)Callback_Battle__GameEnd);
        Hide();
    }

    private void Restart()
    {
        Hide();
        Singleton<SignalManager>.Get().Find<Signal_Battle__RePlay>()?.Invoke();
    }

    private void Return()
    {
        Hide();
        Singleton<UIManager>.Get().Get(UIDefine.UIMain).Show();
        Singleton<UIManager>.Get().Get(UIDefine.UIBattle).Hide();
    }

    private void Callback_Battle__GameEnd(Player victory)
    {
        this.gameObject.SetActive(true);

        if(victory == null)
        {
            TipsText.text = "平局！";
        }
        else
        {
            TipsText.text = victory.Name + "获胜！";
        }
    }

    public override void Show()
    {
        this.gameObject.SetActive(true);
    }

    public override void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
