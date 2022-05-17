using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : IDisposable
{
    private Transform UICanvas;
    private Dictionary<string, UIBase> _dicUI = new Dictionary<string, UIBase>();

    public void Init()
    {
        UICanvas = GameObject.Instantiate(Resources.Load<GameObject>(UIDefine.CanvasPath)).transform;
    }

    public void Add(UIBase uI)
    {
        if (!_dicUI.ContainsKey(uI.Name))
        {
            _dicUI[uI.Name] = uI;
        }
    }

    public void Close(UIBase uI)
    {
        if (_dicUI.ContainsKey(uI.Name))
        {
            _dicUI.Remove(uI.Name);
        }
    }

    public UIBase Get(string name)
    {
        if(_dicUI.ContainsKey(name))
        {
            return _dicUI[name];
        }
        return null;
    }

    public void Dispose()
    {
        
    }
}
