
using UnityEngine;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float swipeThreshold = 50f; // Minimum mesafe threshold
    public float tapThreshold = 0.2f; // Tap için maksimum süre

    [Header("Events")]
    public UnityEvent onTap;
    public UnityEvent<Vector2, Vector2> onSwipeLeft;
    public UnityEvent<Vector2, Vector2> onSwipeRight;
    public UnityEvent<Vector2, Vector2> onSwipeUp;
    public UnityEvent<Vector2, Vector2> onSwipeDown;
    public UnityEvent onMouseUp;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float startTime;

    private bool isSwiping = false;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput(); // PC ve Editor için fare girişi
#elif UNITY_IOS || UNITY_ANDROID
        HandleTouchInput(); // Mobil cihazlar için dokunma girişi
#endif
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Dokunma başladı
        {
            StartInput(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping) // Dokunma bitti
        {
            EndInput(Input.mousePosition);
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartInput(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndInput(touch.position);
                    break;
            }
        }
    }

    private void StartInput(Vector2 position)
    {
        startTouchPosition = position;
        startTime = Time.time;
        isSwiping = true;
    }

    private void EndInput(Vector2 position)
    {
        endTouchPosition = position;
        float elapsedTime = Time.time - startTime;

        onMouseUp?.Invoke(); // MouseUp veya dokunma bırakıldığında tetiklenir

        if (elapsedTime <= tapThreshold && Vector2.Distance(startTouchPosition, endTouchPosition) < swipeThreshold)
        {
            onTap?.Invoke(); // Tap eventini tetikle
        }
        else
        {
            DetectSwipe();
        }

        isSwiping = false;
    }

    private void DetectSwipe()
    {
        Vector2 swipeDirection = endTouchPosition - startTouchPosition;

        if (swipeDirection.magnitude >= swipeThreshold) // Yeterince uzun bir swipe mı?
        {
            Vector2 normalizedDirection = swipeDirection.normalized;

            // Yatay mı dikey mi karar ver
            if (Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.y))
            {
                if (normalizedDirection.x > 0)
                {
                    onSwipeRight?.Invoke(startTouchPosition, Vector2.right);
                }
                else
                {
                    onSwipeLeft?.Invoke(startTouchPosition, Vector2.left);
                }
            }
            else
            {
                if (normalizedDirection.y > 0)
                {
                    onSwipeUp?.Invoke(startTouchPosition, Vector2.up);
                }
                else
                {
                    onSwipeDown?.Invoke(startTouchPosition, Vector2.down);
                }
            }
        }
    }
}
