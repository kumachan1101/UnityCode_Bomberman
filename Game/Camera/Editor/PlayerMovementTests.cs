using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class PlayerMovementTests
{
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
    private int originalXMax;
    private int originalZMax;
    private float originalFixedDeltaTime;
    private bool originalAutoSimulation;
    private GameObject player;
    private GameObject wall;
    private Rigidbody body;
    private PlayerMovement movement;

    [SetUp]
    public void SetUp()
    {
        originalXMax = GameManager.xmax;
        originalZMax = GameManager.zmax;
        originalFixedDeltaTime = Time.fixedDeltaTime;
        originalAutoSimulation = Physics.autoSimulation;
        GameManager.xmax = 20;
        GameManager.zmax = 20;
        Time.fixedDeltaTime = 0.02f;

        player = new GameObject("Player movement test");
        Animator animator = player.AddComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<GameObject>("Player")
            .GetComponent<Animator>().runtimeAnimatorController;
        body = player.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezePositionY |
                           RigidbodyConstraints.FreezeRotation;
        player.AddComponent<CapsuleCollider>();
        movement = player.AddComponent<PlayerMovement>();
        movement.Awake();
        player.GetComponent<PlayerAnimation>().Awake();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        if (wall != null) Object.DestroyImmediate(wall);
        GameManager.xmax = originalXMax;
        GameManager.zmax = originalZMax;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        Physics.autoSimulation = originalAutoSimulation;
    }

    [Test]
    public void MoveQueuesPhysicsVelocityWithoutChangingTransformImmediately()
    {
        player.transform.position = new Vector3(10f, 0.5f, 10f);
        Vector3 start = player.transform.position;

        movement.Move(Vector3.right);

        Assert.That(player.transform.position, Is.EqualTo(start));
        InvokePrivate("FixedUpdate");
        Assert.That(body.velocity.x, Is.EqualTo(3f).Within(0.001f));
        Assert.That(body.velocity.z, Is.EqualTo(0f).Within(0.001f));
        Assert.That(body.collisionDetectionMode, Is.EqualTo(CollisionDetectionMode.ContinuousDynamic));
        Assert.That(body.interpolation, Is.EqualTo(RigidbodyInterpolation.Interpolate));
    }

    [Test]
    public void FixedUpdateLimitsAHighSpeedStepToTheOuterWallBoundary()
    {
        player.transform.position = new Vector3(10f, 0.5f, 10f);
        typeof(PlayerMovement).GetField("moveSpeed", PrivateInstance).SetValue(movement, 1000f);
        movement.Move(Vector3.right);

        InvokePrivate("FixedUpdate");

        float predictedX = body.position.x + body.velocity.x * Time.fixedDeltaTime;
        Assert.That(predictedX, Is.LessThanOrEqualTo(17f + 0.001f));
        Assert.That(predictedX, Is.EqualTo(17f).Within(0.001f));
    }

    [Test]
    public void LateUpdateReturnsAnEscapedPlayerInsideThePlayableArea()
    {
        body.position = new Vector3(-50f, 0.5f, 90f);
        body.velocity = new Vector3(-20f, 0f, 20f);

        InvokePrivate("LateUpdate");

        Assert.That(body.position.x, Is.EqualTo(2f).Within(0.001f));
        Assert.That(body.position.z, Is.EqualTo(17f).Within(0.001f));
        Assert.That(body.velocity.x, Is.Zero.Within(0.001f));
        Assert.That(body.velocity.z, Is.Zero.Within(0.001f));
    }

    [Test]
    public void ContinuousPhysicsCannotTunnelThroughAWallAtExtremeSpeed()
    {
        Physics.autoSimulation = false;
        player.transform.position = new Vector3(10f, 0.5f, 10f);
        wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Outer wall test";
        wall.transform.position = new Vector3(12f, 0.5f, 10f);
        typeof(PlayerMovement).GetField("moveSpeed", PrivateInstance).SetValue(movement, 1000f);
        Physics.SyncTransforms();

        movement.Move(Vector3.right);
        InvokePrivate("FixedUpdate");
        Physics.Simulate(Time.fixedDeltaTime);

        Assert.That(body.position.x, Is.LessThanOrEqualTo(11.01f));
        Assert.That(body.position.x, Is.GreaterThan(10f));
    }

    [Test]
    public void ClampUsesTheCurrentFieldSizeForAllGameModes()
    {
        GameManager.xmax = 40;
        GameManager.zmax = 28;

        Vector3 clamped = PlayerMovement.ClampToPlayableArea(new Vector3(100f, 3f, -10f));

        Assert.That(clamped, Is.EqualTo(new Vector3(37f, 3f, 2f)));
    }

    private void InvokePrivate(string methodName)
    {
        typeof(PlayerMovement).GetMethod(methodName, PrivateInstance).Invoke(movement, null);
    }
}
