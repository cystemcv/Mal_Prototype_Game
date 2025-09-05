using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollRectHardLock : MonoBehaviour
{
    [Tooltip("How long to lock the scroll view when it becomes active (seconds).")]
    public float lockDuration = 0.5f;

    private ScrollRect sr;
    private Vector2 lockedPos;

    private ScrollRect.MovementType prevMovementType;
    private bool prevInertia;
    private float prevSensitivity;

    private Coroutine lockRoutine;

    private void OnEnable()
    {
        sr = GetComponent<ScrollRect>();
        if (sr == null || sr.content == null) return;

        if (lockRoutine != null) StopCoroutine(lockRoutine);
        lockRoutine = StartCoroutine(LockForSeconds(lockDuration));
    }

    private IEnumerator LockForSeconds(float seconds)
    {
        yield return null; // let Unity build layout
        Canvas.ForceUpdateCanvases();

        //  Force scroll to TOP (important: in Unity, 0 = top, 1 = bottom, for verticalNormalizedPosition)
        if (sr.vertical)
        {
            sr.verticalNormalizedPosition = 0f;
        }
        if (sr.horizontal)
        {
            sr.horizontalNormalizedPosition = 0f; // usually "left"
        }
        Canvas.ForceUpdateCanvases();

        // Capture "top" position for locking
        lockedPos = sr.content.anchoredPosition;

        prevMovementType = sr.movementType;
        prevInertia = sr.inertia;
        prevSensitivity = sr.scrollSensitivity;

        sr.inertia = false;
        sr.scrollSensitivity = 0f;
        sr.movementType = ScrollRect.MovementType.Unrestricted;
        sr.StopMovement();

        float t = 0f;
        while (t < seconds)
        {
            yield return new WaitForEndOfFrame();

            Vector2 p = sr.content.anchoredPosition;
            if (sr.vertical) p.y = lockedPos.y;
            if (sr.horizontal) p.x = lockedPos.x;
            sr.content.anchoredPosition = p;
            sr.velocity = Vector2.zero;

            t += Time.unscaledDeltaTime;
        }

        // Restore settings
        sr.movementType = prevMovementType;
        sr.inertia = prevInertia;
        sr.scrollSensitivity = prevSensitivity;

        Canvas.ForceUpdateCanvases();
        sr.StopMovement();

        lockRoutine = null;
    }
}
