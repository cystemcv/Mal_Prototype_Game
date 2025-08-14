using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingTextAnimator : MonoBehaviour
{
    public TMP_Text loadingText; // For old Unity UI
    // public TMP_Text loadingText; // If using TextMeshPro

    public float dotInterval = 0.1f; // Seconds between dot changes
    private string baseText = "Loading";
    private bool isAnimating = false;

    private void OnEnable()
    {
        isAnimating = true;
        StartCoroutine(AnimateLoadingText());
    }

    private void OnDisable()
    {
        isAnimating = false;
    }

    private IEnumerator AnimateLoadingText()
    {
        int dotCount = 0;

        while (isAnimating)
        {
            dotCount = (dotCount + 1) % 4; 
            loadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(dotInterval);
        }
    }
}
