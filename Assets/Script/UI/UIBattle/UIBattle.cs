using Framework.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattle : UIBase
{
    private void Awake()
    {
        _name = UIDefine.UIBattle;
        Hide();
        Singleton<UIManager>.Get().Add(this);
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
