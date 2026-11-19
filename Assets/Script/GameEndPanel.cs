using UnityEngine;

public class GameEndPanel : MonoBehaviour
{
    private void OnEnable()
    {
        CaroEvent.OnWin += ShowWinPanel;
    }

    private void OnDisable()
    {
        CaroEvent.OnWin -= ShowWinPanel;
    }
    private void ShowWinPanel()
    {
        Debug.Log("Win");
    }
}