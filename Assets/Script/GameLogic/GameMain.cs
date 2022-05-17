using Framework.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private BattleManager _core;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Singleton<UIManager>.Create();
        Singleton<UIManager>.Get().Init();
    }
}
