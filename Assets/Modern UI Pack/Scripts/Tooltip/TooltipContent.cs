using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    [AddComponentMenu("Modern UI Pack/Tooltip/Tooltip Content")]
    public class TooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Content")]
        [TextArea] public string description;
        public float delay;

        [Header("Resources")]
        public GameObject tooltipRect;
        public TextMeshProUGUI descriptionText;

        [Header("Settings")]
        public bool forceToUpdate = false;
        public bool useIn3D = false;

        TooltipManager tpManager;
        [HideInInspector] public Animator tooltipAnimator;

        // internal state
        private Coroutine showCoroutine;
        private Coroutine widthCalcCoroutine;
        private bool isPointerOver = false;
        private static readonly int InHash = Animator.StringToHash("In");
        private static readonly int OutHash = Animator.StringToHash("Out");

        private CanvasGroup tooltipCanvasGroup;

        void Awake()
        {
            if (tooltipRect != null)
                tooltipCanvasGroup = tooltipRect.GetComponent<CanvasGroup>();
            if (tooltipCanvasGroup == null && tooltipRect != null)
                tooltipCanvasGroup = tooltipRect.AddComponent<CanvasGroup>();
        }

        private void SetTooltipVisible(bool visible)
        {
            if (tooltipRect == null || tooltipCanvasGroup == null) return;

            tooltipCanvasGroup.alpha = visible ? 1f : 0f;
            tooltipCanvasGroup.interactable = visible;
            tooltipCanvasGroup.blocksRaycasts = visible;
        }

        void Start()
        {
            if (tooltipRect == null || descriptionText == null)
            {
                try
                {
                    tooltipRect = GameObject.Find("Tooltip").transform.Find("Tooltip Rect").gameObject;
                    descriptionText = tooltipRect.transform.GetComponentInChildren<TextMeshProUGUI>();
                }
                catch
                {
                    Debug.LogError("<b>[Tooltip Content]</b> Tooltip Rect is missing.", this);
                    return;
                }
            }

            if (tooltipRect != null)
            {
                tpManager = tooltipRect.GetComponentInParent<TooltipManager>();
                tooltipAnimator = tooltipRect.GetComponentInParent<Animator>();
            }

            if (tpManager != null && tpManager.contentLE == null && descriptionText != null)
                tpManager.contentLE = descriptionText.GetComponent<LayoutElement>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerOver = true;
            StartEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOver = false;
            StartExit();
        }

#if !UNITY_IOS && !UNITY_ANDROID
        public void OnMouseEnter() { if (useIn3D) { isPointerOver = true; StartEnter(); } }
        public void OnMouseExit() { if (useIn3D) { isPointerOver = false; StartExit(); } }
#endif

        private void StartEnter()
        {
            // Cancel any pending hide
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
                showCoroutine = null;
            }

            showCoroutine = StartCoroutine(ProcessEnterRoutine());
        }

        public void StartExit()
        {
            // Cancel pending show
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
                showCoroutine = null;
            }

            ProcessExitImmediate();
        }

        public void HideTooltip()
        {
            isPointerOver = false;

            if (tpManager != null)
            {
                if (tpManager.currentTooltip == this)
                    tpManager.currentTooltip = null;

                tpManager.allowUpdate = false;
            }

            if (tooltipAnimator != null)
                tooltipAnimator.Play("Out", 0, 0f);

            if (descriptionText != null)
                descriptionText.text = string.Empty;

            if (tooltipRect != null)
                SetTooltipVisible(false);
        }


        private IEnumerator ProcessEnterRoutine()
        {
            yield return new WaitForSecondsRealtime(0.05f);

            if (!isPointerOver)
                yield break;

            // If description is empty/whitespace, explicitly hide any lingering tooltip and bail out.
            if (string.IsNullOrWhiteSpace(description))
            {
                HideTooltip();
                if (tooltipRect != null)
                    SetTooltipVisible(false);
                yield break;
            }

            if (tooltipRect == null)
                yield break;

            // Set content
            descriptionText.text = description;

            if (tpManager != null)
            {
                tpManager.allowUpdate = true;
                tpManager.currentTooltip = this;

                if (tpManager.contentLE == null)
                    CreateLayoutElementIfNeeded();

                tpManager.contentLE.preferredWidth = tpManager.preferredWidth;
                tpManager.contentLE.enabled = false;
            }

            CheckForContentWidth();

            if (tooltipAnimator != null)
            {
                tooltipAnimator.gameObject.SetActive(false);
                tooltipAnimator.gameObject.SetActive(true);
            }

            if (delay <= 0f)
            {
                PlayIn();
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
                if (!isPointerOver)
                    yield break;

                PlayIn();
            }

            if (forceToUpdate)
                StartCoroutine(UpdateLayoutPosition());
        }


        private void ProcessExitImmediate()
        {
            if (tooltipRect == null || tooltipAnimator == null)
                return;

            // Play out animation (optional)
            tooltipAnimator.Play("Out", 0, 0f);

            // Stop any pending show coroutine to avoid race conditions
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
                showCoroutine = null;
            }

            if (tpManager != null)
            {
                tpManager.allowUpdate = false;
                if (tpManager.currentTooltip == this)
                    tpManager.currentTooltip = null;
            }

            if (descriptionText != null)
                descriptionText.text = string.Empty;

            // Hide tooltip instantly by CanvasGroup alpha
            SetTooltipVisible(false);

            // Reset tooltip position off-screen (or to a fixed safe position)
            RectTransform rt = tooltipRect.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(-10000, -10000);  // move far off-screen
            }
        }




        private void PlayIn()
        {
            if (tooltipAnimator == null || tooltipRect == null) return;

            tooltipRect.SetActive(true);
            tooltipAnimator.Play("In", 0, 0f);
        }

        private void PlayOut()
        {
            if (tooltipAnimator == null || tooltipRect == null) return;

            tooltipAnimator.Play("Out", 0, 0f);
            // immediately hide to avoid ghosting; if you want the out animation to be visible, delay this by the clip length.
            SetTooltipVisible(false);
        }

        public void CheckForContentWidth()
        {
            CreateLayoutElementIfNeeded();
            if (widthCalcCoroutine != null)
                StopCoroutine(widthCalcCoroutine);
            widthCalcCoroutine = StartCoroutine(CalculateContentWidth());
        }

        private void CreateLayoutElementIfNeeded()
        {
            if (tpManager == null || descriptionText == null)
                return;

            if (tpManager.contentLE == null)
            {
                descriptionText.gameObject.AddComponent<LayoutElement>();
                tpManager.contentLE = descriptionText.GetComponent<LayoutElement>();
            }

            tpManager.contentLE.preferredWidth = tpManager.preferredWidth;
            tpManager.contentLE.enabled = false;
        }

        private IEnumerator CalculateContentWidth()
        {
            yield return new WaitForSecondsRealtime(0.05f);

            if (descriptionText == null || tpManager == null || tpManager.contentLE == null)
                yield break;

            float tempWidth = descriptionText.GetComponent<RectTransform>().sizeDelta.x;

            if (tempWidth >= tpManager.preferredWidth + 1)
                tpManager.contentLE.enabled = true;

            LayoutRebuilder.ForceRebuildLayoutImmediate(tpManager.contentLE.gameObject.GetComponent<RectTransform>());
            tpManager.contentLE.preferredWidth = tpManager.preferredWidth;
        }

        IEnumerator UpdateLayoutPosition()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            if (tooltipAnimator != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipAnimator.gameObject.GetComponent<RectTransform>());
            }
        }

        public void ProcessExit()
        {
            isPointerOver = false;
            StartExit(); // cancels pending show and runs exit logic
        }

        // Optional: if object or tooltip gets disabled, ensure state resets
        void OnDisable()
        {
            isPointerOver = false;
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
                showCoroutine = null;
            }

            if (widthCalcCoroutine != null)
            {
                StopCoroutine(widthCalcCoroutine);
                widthCalcCoroutine = null;
            }

            if (tpManager != null)
                tpManager.allowUpdate = false;
        }
    }
}
