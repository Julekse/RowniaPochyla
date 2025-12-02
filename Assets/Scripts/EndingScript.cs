using UnityEngine;
using UnityEngine.UI;
using System;

public class EndingScript : MonoBehaviour
{
    public Text FinalText;
    public Square_script squareScript;
    bool wasshown = false;

    void Start()
    {
        if (squareScript == null)
            squareScript = FindObjectOfType<Square_script>();

        if (FinalText == null)
            FinalText = GetComponent<Text>() ?? GetComponentInChildren<Text>();

        if (FinalText != null)
            FinalText.gameObject.SetActive(false);

        if (squareScript != null)
        {
            // subskrybuj eventy
            squareScript.OnStopped += OnSquareStopped;
            squareScript.OnStarted += OnSquareStarted;
        }
        else
        {
            Debug.LogWarning("EndingScript: Nie znaleziono Square_script w Start()", this);
        }
    }

    void OnDestroy()
    {
        if (squareScript != null)
        {
            squareScript.OnStopped -= OnSquareStopped;
            squareScript.OnStarted -= OnSquareStarted;
        }
    }

    private void OnSquareStopped()
    {
        if (FinalText == null) return;
        FinalText.text = "Koniec!\nLiczba odbiæ: " + (squareScript != null ? squareScript.iteration.ToString("n0") : "0")
                        + "\nWprowadŸ ponownie dane i wciœnij 'p' aby uruchomiæ ponownie";
        FinalText.gameObject.SetActive(true);
        wasshown = true;
        Debug.Log("EndingScript: OnSquareStopped wywo³ane.");
    }

    private void OnSquareStarted()
    {
        if (FinalText == null) return;
        FinalText.gameObject.SetActive(false);
        wasshown = false;
        Debug.Log("EndingScript: OnSquareStarted wywo³ane.");
    }

    // zachowujemy Update() jako dodatkowe zabezpieczenie (opcjonalne)
    void Update()
    {
        if (squareScript == null)
            squareScript = FindObjectOfType<Square_script>();
    }
}
