using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected string _name;
    public string Name => _name;

    public abstract void Show();
    public abstract void Hide();
}