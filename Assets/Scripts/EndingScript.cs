using UnityEngine;
using UnityEngine.UI;

public class EndingScript : MonoBehaviour
{
    [Header("UI Components")]
    public Text finalText;

    [Header("References")]
    [SerializeField] private Square_script squareScript;

    private void Start()
    {
        if (squareScript == null)
            squareScript = FindFirstObjectByType<Square_script>();

        if (finalText == null)
            finalText = GetComponent<Text>() ?? GetComponentInChildren<Text>();

        if (finalText != null)
            finalText.gameObject.SetActive(false);

        if (squareScript != null)
        {
            squareScript.OnStopped += OnSquareStopped;
            squareScript.OnStarted += OnSquareStarted;
        }
        else
        {
            Debug.LogError("EndingScript: Brak połączenia do Square_script!", this);
        }
    }

    private void OnDestroy()
    {
        if (squareScript != null)
        {
            squareScript.OnStopped -= OnSquareStopped;
            squareScript.OnStarted -= OnSquareStarted;
        }
    }

    private void OnSquareStopped()
    {
        if (finalText == null || squareScript == null) return;

        finalText.text = $"Koniec!\n" +
                         $"Liczba odbić: {squareScript.iteration:n0}\n" +
                         $"Wprowadź ponownie dane i wciśnij 'P', aby uruchomić ponownie";
        
        finalText.gameObject.SetActive(true);
    }

    private void OnSquareStarted()
    {
        if (finalText != null)
        {
            finalText.gameObject.SetActive(false);
        }
    }
}