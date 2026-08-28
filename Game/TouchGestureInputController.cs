using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TouchGestureResult
{
    None,
    BombTap
}

// Keeps gesture classification independent from Unity's frame input so the
// tap/drag boundary and multi-touch behaviour can be regression tested.
public sealed class TouchGestureRecognizer
{
    private sealed class GestureState
    {
        public Vector2 startPosition;
        public Vector2 currentPosition;
        public float startTime;
        public float maximumDistance;
        public long sequence;
    }

    public const int NoPointer = int.MinValue;

    private readonly Dictionary<int, GestureState> gestures =
        new Dictionary<int, GestureState>();
    private readonly List<int> missingPointers = new List<int>();
    private long nextSequence;
    private int movementPointer = NoPointer;

    public float DragThreshold { get; set; }
    public float TapMaximumDuration { get; set; }
    public Vector2 Movement { get; private set; }
    public bool IsMoving { get { return movementPointer != NoPointer; } }

    public TouchGestureRecognizer(float dragThreshold, float tapMaximumDuration)
    {
        DragThreshold = Mathf.Max(1f, dragThreshold);
        TapMaximumDuration = Mathf.Max(0.01f, tapMaximumDuration);
    }

    public void PointerDown(int pointerId, Vector2 position, float time)
    {
        // WebGL can reuse a finger id immediately after a fast flick. If the
        // preceding end event was skipped, never inherit its movement state.
        if (gestures.ContainsKey(pointerId))
            CancelPointer(pointerId);

        gestures[pointerId] = new GestureState
        {
            startPosition = position,
            currentPosition = position,
            startTime = time,
            sequence = nextSequence++
        };
    }

    public void PointerMove(int pointerId, Vector2 position)
    {
        GestureState gesture;
        if (!gestures.TryGetValue(pointerId, out gesture)) return;

        gesture.currentPosition = position;
        Vector2 delta = position - gesture.startPosition;
        gesture.maximumDistance = Mathf.Max(gesture.maximumDistance, delta.magnitude);

        if (movementPointer == NoPointer && gesture.maximumDistance >= DragThreshold)
            movementPointer = pointerId;

        if (movementPointer == pointerId)
            Movement = CalculateMovement(delta);
    }

    public TouchGestureResult PointerUp(int pointerId, Vector2 position, float time)
    {
        GestureState gesture;
        if (!gestures.TryGetValue(pointerId, out gesture)) return TouchGestureResult.None;

        PointerMove(pointerId, position);
        bool wasMovementPointer = movementPointer == pointerId;
        bool isTap = !wasMovementPointer &&
            gesture.maximumDistance < DragThreshold &&
            time - gesture.startTime <= TapMaximumDuration;

        gestures.Remove(pointerId);
        if (wasMovementPointer)
        {
            movementPointer = NoPointer;
            Movement = Vector2.zero;
            PromoteWaitingDrag();
        }

        return isTap ? TouchGestureResult.BombTap : TouchGestureResult.None;
    }

    public void CancelPointer(int pointerId)
    {
        if (!gestures.Remove(pointerId)) return;
        if (movementPointer != pointerId) return;

        movementPointer = NoPointer;
        Movement = Vector2.zero;
        PromoteWaitingDrag();
    }

    public void Reset()
    {
        gestures.Clear();
        movementPointer = NoPointer;
        Movement = Vector2.zero;
    }

    public void CancelPointersExcept(ICollection<int> activePointerIds)
    {
        missingPointers.Clear();
        foreach (int pointerId in gestures.Keys)
        {
            if (!activePointerIds.Contains(pointerId))
                missingPointers.Add(pointerId);
        }

        foreach (int pointerId in missingPointers)
            CancelPointer(pointerId);
        missingPointers.Clear();
    }

    private Vector2 CalculateMovement(Vector2 delta)
    {
        float responseRadius = Mathf.Max(DragThreshold * 3f, 1f);
        return Vector2.ClampMagnitude(delta / responseRadius, 1f);
    }

    private void PromoteWaitingDrag()
    {
        int candidateId = NoPointer;
        GestureState candidate = null;
        foreach (KeyValuePair<int, GestureState> pair in gestures)
        {
            if (pair.Value.maximumDistance < DragThreshold) continue;
            if (candidate == null || pair.Value.sequence < candidate.sequence)
            {
                candidateId = pair.Key;
                candidate = pair.Value;
            }
        }

        if (candidate == null) return;
        movementPointer = candidateId;
        Movement = CalculateMovement(candidate.currentPosition - candidate.startPosition);
    }
}

[DisallowMultipleComponent]
public sealed class TouchGestureInputController : MonoBehaviour
{
    private const float TapMaximumDuration = 0.3f;
    private const float MinimumDragThreshold = 18f;
    private const float MaximumDragThreshold = 48f;
    private const float RelativeDragThreshold = 0.04f;

    private readonly HashSet<int> ignoredPointers = new HashSet<int>();
    private readonly HashSet<int> observedPointers = new HashSet<int>();
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    private TouchGestureRecognizer recognizer;
    private JoystickController joystickController;
    private DropBomBtn dropBomButton;
    private bool gestureModeEnabled;

    public bool GestureModeEnabled { get { return gestureModeEnabled; } }
    public Vector2 CurrentMovement
    {
        get { return recognizer != null ? recognizer.Movement : Vector2.zero; }
    }

    private void Awake()
    {
        Transform joystick = transform.Find("JoystickPlayer");
        Transform bomb = transform.Find("bom");
        joystickController = joystick != null ? joystick.GetComponent<JoystickController>() : null;
        dropBomButton = bomb != null ? bomb.GetComponent<DropBomBtn>() : null;
        recognizer = new TouchGestureRecognizer(CalculateDragThreshold(), TapMaximumDuration);
        SetGestureMode(Input.touchSupported);
    }

    private void Update()
    {
        if (!gestureModeEnabled) return;

        recognizer.DragThreshold = CalculateDragThreshold();
        observedPointers.Clear();
        for (int index = 0; index < Input.touchCount; index++)
        {
            Touch touch = Input.GetTouch(index);
            observedPointers.Add(touch.fingerId);
            ProcessTouch(touch);
        }

        // Do not rely solely on Ended/Canceled. Under a rapid WebGL flick the
        // browser can drop that one-frame event, while the next frame already
        // reports no finger. Reconcile against the actual active touch list.
        recognizer.CancelPointersExcept(observedPointers);
        ignoredPointers.RemoveWhere(pointerId => !observedPointers.Contains(pointerId));
        if (observedPointers.Count == 0)
            recognizer.Reset();

        ApplyMovement();
    }

    private void OnDisable()
    {
        ResetInput();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) ResetInput();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused) ResetInput();
    }

    public void SetGestureMode(bool enabled)
    {
        gestureModeEnabled = enabled;
        SetLegacyControlVisible(transform.Find("JoystickPlayer"), !enabled);
        SetLegacyControlVisible(transform.Find("bom"), !enabled);

        if (joystickController != null)
            joystickController.SetGestureControlEnabled(enabled);
        ResetInput();
    }

    private void ProcessTouch(Touch touch)
    {
        int pointerId = touch.fingerId;
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (IsOverProtectedButton(pointerId, touch.position))
                {
                    ignoredPointers.Add(pointerId);
                    return;
                }
                recognizer.PointerDown(pointerId, touch.position, Time.unscaledTime);
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (!ignoredPointers.Contains(pointerId))
                    recognizer.PointerMove(pointerId, touch.position);
                break;
            case TouchPhase.Ended:
                if (ignoredPointers.Remove(pointerId)) return;
                if (recognizer.PointerUp(pointerId, touch.position, Time.unscaledTime) ==
                    TouchGestureResult.BombTap)
                {
                    RequestDropBomb();
                }
                break;
            case TouchPhase.Canceled:
                if (!ignoredPointers.Remove(pointerId)) recognizer.CancelPointer(pointerId);
                break;
        }
    }

    private void ApplyMovement()
    {
        if (joystickController != null)
            joystickController.SetGestureMoveVector(recognizer.Movement);
    }

    private void RequestDropBomb()
    {
        if (dropBomButton != null) dropBomButton.PushButton();
    }

    private bool IsOverProtectedButton(int pointerId, Vector2 position)
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            pointerId = pointerId,
            position = position
        };
        raycastResults.Clear();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (RaycastResult result in raycastResults)
        {
            Transform current = result.gameObject.transform;
            while (current != null)
            {
                Button button = current.GetComponent<Button>();
                if (button != null && button.isActiveAndEnabled && button.IsInteractable())
                    return true;
                current = current.parent;
            }
        }
        return false;
    }

    private void ResetInput()
    {
        ignoredPointers.Clear();
        observedPointers.Clear();
        if (recognizer != null) recognizer.Reset();
        if (joystickController != null)
            joystickController.SetGestureMoveVector(Vector2.zero);
    }

    private static float CalculateDragThreshold()
    {
        float shortestSide = Mathf.Min(Screen.width, Screen.height);
        return Mathf.Clamp(shortestSide * RelativeDragThreshold,
            MinimumDragThreshold, MaximumDragThreshold);
    }

    private static void SetLegacyControlVisible(Transform control, bool visible)
    {
        if (control == null) return;
        CanvasGroup canvasGroup = control.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = control.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}
