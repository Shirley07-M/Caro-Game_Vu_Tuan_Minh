using UnityEngine;

public class GameEndPanel : MonoBehaviour
{
    private void OnEnable()
    {
        CaroEvent.OnWin += ShowWinPanel;
        CaroEvent.OnLost += ShowLostPanel;
        CaroEvent.OnDraw += ShowDrawPanel;
    }

    private void OnDisable()
    {
        CaroEvent.OnWin -= ShowWinPanel;
        CaroEvent.OnLost -= ShowLostPanel;
        CaroEvent.OnDraw -= ShowDrawPanel;
    }

    private void ShowWinPanel()
    {
        Debug.Log("Win");
    }

    private void ShowLostPanel()
    {
        Debug.Log("Lost");
    }

    private void ShowDrawPanel()
    {
        Debug.Log("Draw");
    }
}