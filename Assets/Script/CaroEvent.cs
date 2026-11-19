using System;
public static class CaroEvent
{
    public static event Action OnWin;
    public static event Action OnLost;
    public static event Action OnDraw;

    public static void InvokeGameWin() => OnWin?.Invoke();
    public static void InvokeGameLost() => OnLost?.Invoke();
    public static void InvokeGameDraw() => OnDraw?.Invoke();
}
