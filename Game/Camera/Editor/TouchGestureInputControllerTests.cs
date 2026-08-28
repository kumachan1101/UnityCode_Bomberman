using NUnit.Framework;
using UnityEngine;

public sealed class TouchGestureInputControllerTests
{
    [Test]
    public void ShortStationaryTouchPlacesOneBomb()
    {
        TouchGestureRecognizer recognizer = CreateRecognizer();

        recognizer.PointerDown(1, new Vector2(100f, 200f), 1f);
        TouchGestureResult result = recognizer.PointerUp(
            1, new Vector2(105f, 203f), 1.2f);

        Assert.That(result, Is.EqualTo(TouchGestureResult.BombTap));
        Assert.That(recognizer.IsMoving, Is.False);
        Assert.That(recognizer.Movement, Is.EqualTo(Vector2.zero));
    }

    [Test]
    public void DragMovesInTouchDirectionAndNeverPlacesBomb()
    {
        TouchGestureRecognizer recognizer = CreateRecognizer();

        recognizer.PointerDown(3, new Vector2(100f, 100f), 5f);
        recognizer.PointerMove(3, new Vector2(160f, 130f));

        Assert.That(recognizer.IsMoving, Is.True);
        Assert.That(recognizer.Movement.x, Is.GreaterThan(0f));
        Assert.That(recognizer.Movement.y, Is.GreaterThan(0f));

        TouchGestureResult result = recognizer.PointerUp(
            3, new Vector2(102f, 101f), 5.2f);
        Assert.That(result, Is.EqualTo(TouchGestureResult.None),
            "A drag must not turn into a bomb tap after returning near its start.");
        Assert.That(recognizer.IsMoving, Is.False);
        Assert.That(recognizer.Movement, Is.EqualTo(Vector2.zero));
    }

    [Test]
    public void SecondFingerCanTapBombWithoutStoppingMovement()
    {
        TouchGestureRecognizer recognizer = CreateRecognizer();
        recognizer.PointerDown(10, Vector2.zero, 2f);
        recognizer.PointerMove(10, new Vector2(60f, 0f));
        Vector2 movementBeforeTap = recognizer.Movement;

        recognizer.PointerDown(11, new Vector2(300f, 200f), 2.1f);
        TouchGestureResult result = recognizer.PointerUp(
            11, new Vector2(302f, 202f), 2.2f);

        Assert.That(result, Is.EqualTo(TouchGestureResult.BombTap));
        Assert.That(recognizer.IsMoving, Is.True);
        Assert.That(recognizer.Movement, Is.EqualTo(movementBeforeTap));
    }

    [Test]
    public void HoldingOrCanceledTouchDoesNotPlaceBombAndStopsMovement()
    {
        TouchGestureRecognizer recognizer = CreateRecognizer();
        recognizer.PointerDown(20, Vector2.zero, 0f);
        Assert.That(recognizer.PointerUp(20, Vector2.zero, 0.31f),
            Is.EqualTo(TouchGestureResult.None));

        recognizer.PointerDown(21, Vector2.zero, 1f);
        recognizer.PointerMove(21, Vector2.right * 60f);
        recognizer.CancelPointer(21);

        Assert.That(recognizer.IsMoving, Is.False);
        Assert.That(recognizer.Movement, Is.EqualTo(Vector2.zero));
    }

    [Test]
    public void WaitingDragContinuesWhenFirstMovementFingerIsReleased()
    {
        TouchGestureRecognizer recognizer = CreateRecognizer();
        recognizer.PointerDown(30, Vector2.zero, 0f);
        recognizer.PointerMove(30, Vector2.right * 60f);
        recognizer.PointerDown(31, Vector2.zero, 0.1f);
        recognizer.PointerMove(31, Vector2.up * 60f);

        recognizer.PointerUp(30, Vector2.right * 60f, 0.2f);

        Assert.That(recognizer.IsMoving, Is.True);
        Assert.That(recognizer.Movement.x, Is.EqualTo(0f).Within(0.001f));
        Assert.That(recognizer.Movement.y, Is.GreaterThan(0f));
    }

    [Test]
    public void MissingEndEventCannotLeaveMovementActive()
    {
        TouchGestureRecognizer recognizer = CreateRecognizer();
        recognizer.PointerDown(40, Vector2.zero, 0f);
        recognizer.PointerMove(40, Vector2.right * 80f);
        Assert.That(recognizer.IsMoving, Is.True);

        recognizer.CancelPointersExcept(new int[0]);

        Assert.That(recognizer.IsMoving, Is.False);
        Assert.That(recognizer.Movement, Is.EqualTo(Vector2.zero));
    }

    [Test]
    public void RepeatedFlicksAlwaysStopWithoutReceivingPointerUp()
    {
        TouchGestureRecognizer recognizer = CreateRecognizer();
        for (int index = 0; index < 100; index++)
        {
            int pointerId = index % 3;
            Vector2 start = new Vector2(index, index * 2f);
            recognizer.PointerDown(pointerId, start, index);
            recognizer.PointerMove(pointerId, start + Vector2.right * 80f);
            Assert.That(recognizer.IsMoving, Is.True, "flick " + index);

            recognizer.CancelPointersExcept(new int[0]);
            Assert.That(recognizer.IsMoving, Is.False, "release " + index);
            Assert.That(recognizer.Movement, Is.EqualTo(Vector2.zero),
                "release vector " + index);
        }
    }

    [Test]
    public void ReusedFingerIdDoesNotInheritStaleMovement()
    {
        TouchGestureRecognizer recognizer = CreateRecognizer();
        recognizer.PointerDown(50, Vector2.zero, 0f);
        recognizer.PointerMove(50, Vector2.right * 80f);

        recognizer.PointerDown(50, new Vector2(300f, 200f), 0.1f);

        Assert.That(recognizer.IsMoving, Is.False);
        Assert.That(recognizer.Movement, Is.EqualTo(Vector2.zero));
        Assert.That(recognizer.PointerUp(50, new Vector2(302f, 201f), 0.2f),
            Is.EqualTo(TouchGestureResult.BombTap));
    }

    private static TouchGestureRecognizer CreateRecognizer()
    {
        return new TouchGestureRecognizer(20f, 0.3f);
    }
}
