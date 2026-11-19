using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CaroState
{
    Empty,
    FilledX,
    FilledO
}
public class CaroController : MonoBehaviour
{
    private TextMeshProUGUI caroValue;
    public Button caro { get; private set; }
    public int caroindex { get; private set; }
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        caroValue = GetComponentInChildren<TextMeshProUGUI>();
        caro = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void SetCaroIndex(int index)
    {
        caroindex = index;
    }

    public void SetCaroValue(CaroState value)
    {
        if (value == CaroState.Empty) caroValue.text = "";
        else if (value == CaroState.FilledX) caroValue.text = "X";
        else caroValue.text = "O";
    }
    public void ForceUnallowInteract()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
