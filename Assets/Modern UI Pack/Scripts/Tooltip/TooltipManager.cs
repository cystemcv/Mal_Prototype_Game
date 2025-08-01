using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Michsky.MUIP
{
    public class TooltipManager : MonoBehaviour
    {
        // Resources
        public Canvas mainCanvas;
        public GameObject tooltipObject;
        public GameObject tooltipContent;
        public Camera targetCamera;

        // Settings
        [Range(0.01f, 0.5f)] public float tooltipSmoothness = 0.1f;
        [Range(1, 100)] public float dampSpeed = 10; // reduced scale, used directly
        public float preferredWidth = 375;
        public bool allowUpdate = true;
        public bool checkDispose = true;
        public CameraSource cameraSource = CameraSource.Main;
        public TransitionMode transitionMode = TransitionMode.Damp;

        // Content Bounds offsets
        [Range(-50, 50)] public int vBorderTop = -15;
        [Range(-50, 50)] public int vBorderBottom = 10;
        [Range(-50, 50)] public int hBorderLeft = 20;
        [Range(-50, 50)] public int hBorderRight = -15;

        // Hard bounds (screen-space decision thresholds)
        [SerializeField] private int xLeft = -400;
        [SerializeField] private int xRight = 400;
        [SerializeField] private int yTop = -325;
        [SerializeField] private int yBottom = 325;

        [HideInInspector] public LayoutElement contentLE;
        [HideInInspector] public TooltipContent currentTooltip;

        // Internal
        private Vector2 uiPos;
        private Vector3 cursorPos;
        private Vector3 contentOffset = Vector3.zero;
        private Vector3 tooltipVelocity = Vector3.zero;

        private RectTransform contentRect;
        private RectTransform tooltipRect;

        public enum CameraSource { Main, Custom }
        public enum TransitionMode { Damp, Snap }

        void Awake()
        {
            // Fix rect transform of this manager to stretch
            RectTransform sourceRect = gameObject.GetComponent<RectTransform>();
            if (sourceRect == null)
            {
                Debug.LogError("<b>[Tooltip]</b> Rect Transform is missing from the object.", this);
                return;
            }

            sourceRect.anchorMin = Vector2.zero;
            sourceRect.anchorMax = Vector2.one;
            sourceRect.offsetMin = Vector2.zero;
            sourceRect.offsetMax = Vector2.zero;

            // Correctly grab tooltip content and pivot it (only once)
            if (tooltipContent != null)
            {
                RectTransform tc = tooltipContent.GetComponent<RectTransform>();
                if (tc != null)
                {
                    tc.pivot = new Vector2(0f, 0f);
                }
            }

            if (mainCanvas == null)
                mainCanvas = gameObject.GetComponentInParent<Canvas>();

            if (cameraSource == CameraSource.Main)
                targetCamera = Camera.main;

            if (tooltipContent != null)
                contentRect = tooltipContent.GetComponent<RectTransform>();

            if (tooltipObject != null)
                tooltipRect = tooltipObject.GetComponent<RectTransform>();

            contentOffset = new Vector3(hBorderLeft, vBorderTop, 0);
            transform.SetAsLastSibling();
        }

        void Update()
        {
            if (allowUpdate == false)
                return;

            if (checkDispose == true && currentTooltip != null && !currentTooltip.gameObject.activeInHierarchy)
            {
                currentTooltip?.ProcessExit(); // if you added the shim above
                currentTooltip = null;
            }

            if (currentTooltip == null)
                return; // nothing to show/update

            if (currentTooltip == null || string.IsNullOrWhiteSpace(currentTooltip.description))
            {
                // ensure tooltip is hidden if somehow still active
                if (tooltipObject != null && tooltipObject.activeSelf)
                    tooltipObject.SetActive(false);
                return;
            }

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            cursorPos = GetPointerPosition();

            // Compute a reference position for bounds decision: use tooltipRect's current anchoredPosition for UI logic fallback
            if (tooltipRect != null)
                uiPos = tooltipRect.anchoredPosition;

            ApplyBounds();

            if (mainCanvas != null && tooltipRect != null)
            {
                if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera || mainCanvas.renderMode == RenderMode.WorldSpace)
                {
                    // Convert screen point to local point inside tooltip parent
                    Vector2 localPoint;
                    RectTransform parentRect = tooltipRect.parent as RectTransform;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, cursorPos, targetCamera, out localPoint))
                    {
                        tooltipRect.localPosition = localPoint;
                    }

                    if (transitionMode == TransitionMode.Damp)
                    {
                        tooltipContent.transform.localPosition = Vector3.SmoothDamp(
                            tooltipContent.transform.localPosition,
                            contentOffset,
                            ref tooltipVelocity,
                            tooltipSmoothness,
                            Mathf.Infinity,
                            Time.unscaledDeltaTime);
                    }
                    else
                    {
                        tooltipContent.transform.localPosition = contentOffset;
                    }
                }
                else if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    tooltipRect.position = cursorPos;

                    Vector3 targetPos = cursorPos + contentOffset;
                    if (transitionMode == TransitionMode.Damp)
                    {
                        tooltipContent.transform.position = Vector3.SmoothDamp(
                            tooltipContent.transform.position,
                            targetPos,
                            ref tooltipVelocity,
                            tooltipSmoothness,
                            Mathf.Infinity,
                            Time.unscaledDeltaTime);
                    }
                    else
                    {
                        tooltipContent.transform.position = targetPos;
                    }
                }
            }
        }

        private Vector2 GetPointerPosition()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.mousePosition;
#else
            return Input.mousePosition; // fallback if neither define
#endif
        }

        private void ApplyBounds()
        {
            if (tooltipRect == null || contentRect == null)
                return;

            // Decide pivot/offset based on current tooltip anchor position in UI space
            Vector2 anchored = tooltipRect.anchoredPosition;

            // Horizontal
            if (anchored.x <= xLeft)
            {
                contentOffset.x = hBorderLeft;
                contentRect.pivot = new Vector2(0f, contentRect.pivot.y);
            }
            else if (anchored.x >= xRight)
            {
                contentOffset.x = hBorderRight;
                contentRect.pivot = new Vector2(1f, contentRect.pivot.y);
            }

            // Vertical
            if (anchored.y <= yTop)
            {
                contentOffset.y = vBorderBottom;
                contentRect.pivot = new Vector2(contentRect.pivot.x, 0f);
            }
            else if (anchored.y >= yBottom)
            {
                contentOffset.y = vBorderTop;
                contentRect.pivot = new Vector2(contentRect.pivot.x, 1f);
            }
        }
    }
}
