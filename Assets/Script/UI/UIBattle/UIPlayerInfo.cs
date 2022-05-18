using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
    public Text TextName;
    public Text TextDescription;
    public Text TextState;

    public Image ImgX;
    public Image ImgO;

    private Player _player;

    void Start()
    {
        
    }

    private void Update()
    {
        if(_player != null)
        {
            TextName.text = _player.Name;
            TextDescription.text = _player.Role == Role.Attacker ? "(先手)" : "(后手)";
            if(_player.Role == Role.Attacker)
            {
                ImgX.gameObject.SetActive(false);
                ImgO.gameObject.SetActive(true);
            }
            else
            {
                ImgX.gameObject.SetActive(true);
                ImgO.gameObject.SetActive(false);
            }
        }
    }

    public void Init(Player player)
    {
        _player = player;
    }
}
