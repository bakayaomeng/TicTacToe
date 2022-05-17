public interface UI
{
    string Name { get; }

    void Show();
    void Tick(float deltaTime);
    void Hide();
}