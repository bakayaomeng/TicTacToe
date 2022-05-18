using Framework.Helper;
using Framework.Signal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : UIBase
{
    public Button BtnPvp;
    public Button BtnPve;
    public Button BtnExit;

    private void Awake()
    {
        _name = UIDefine.UIMain;

        Singleton<UIManager>.Get().Add(this);

        BtnPvp.onClick.AddListener(BtnPvpOnClick);
        BtnPve.onClick.AddListener(BtnPveOnClick);
        BtnExit.onClick.AddListener(BtnExitOnClick);
    }

    private void OnDestroy()
    {
        
    }

    public override void Hide()
    {

    }

    public override void Show()
    {

    }

    private void BtnPvpOnClick()
    {
        Hide();
        Singleton<UIManager>.Get().Get(UIDefine.UIBattle).Show();
        Singleton<SignalManager>.Get().Find<Signal_Battle__Play>()?.Invoke( GameModel.Pvp);
    }

    private void BtnPveOnClick()
    {
        Hide();
        Singleton<UIManager>.Get().Get(UIDefine.UIBattle).Show();
        Singleton<SignalManager>.Get().Find<Signal_Battle__Play>()?.Invoke(GameModel.Pve);
    }

    private void BtnExitOnClick()
    {
        Application.Quit();
    }
}
