using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class BombPlacementAndStageStartTests
{
    private int originalXMax;
    private int originalZMax;

    [SetUp]
    public void SetUp()
    {
        originalXMax = GameManager.xmax;
        originalZMax = GameManager.zmax;
        GameManager.xmax = 20;
        GameManager.zmax = 20;
    }

    [TearDown]
    public void TearDown()
    {
        GameManager.xmax = originalXMax;
        GameManager.zmax = originalZMax;
    }

    [Test]
    public void DiagonalFacingUsesItsDominantGridAxis()
    {
        Assert.That(BomGridRules.GetCardinalDirection(new Vector3(0.3f, 0f, 0.9f)),
            Is.EqualTo(Vector3.forward));
        Assert.That(BomGridRules.GetCardinalDirection(new Vector3(-0.8f, 0f, 0.2f)),
            Is.EqualTo(Vector3.left));
    }

    [Test]
    public void ZeroFacingDoesNotInventAPlacementDirection()
    {
        Assert.That(BomGridRules.GetCardinalDirection(Vector3.zero), Is.EqualTo(Vector3.zero));
    }

    [Test]
    public void ConsecutiveMultiBombCellsAreAlwaysDistinct()
    {
        Vector3 origin = new Vector3(4.1f, 0.5f, 4.1f);
        Vector3 diagonalFacing = new Vector3(0.2f, 0f, 0.98f);

        Vector3 first = BomGridRules.GetCellInDirection(origin, diagonalFacing, 1);
        Vector3 second = BomGridRules.GetCellInDirection(origin, diagonalFacing, 2);
        Vector3 third = BomGridRules.GetCellInDirection(origin, diagonalFacing, 3);

        Assert.That(first, Is.EqualTo(new Vector3(4f, 1f, 5f)));
        Assert.That(second, Is.EqualTo(new Vector3(4f, 1f, 6f)));
        Assert.That(third, Is.EqualTo(new Vector3(4f, 1f, 7f)));
    }

    [Test]
    public void ThrownBombStopsAtPreviousCellWhenNextCellIsOccupied()
    {
        GameObject bomb = new GameObject("Moving bomb test");
        try
        {
            bomb.transform.position = new Vector3(4f, 1f, 4f);
            BomMover mover = bomb.AddComponent<BomMover>();
            mover.ReqMove(Vector3.right, 2);

            mover.Advance(0.25f, cell => cell == new Vector3(5f, 1f, 4f));

            Assert.That(bomb.transform.position, Is.EqualTo(new Vector3(4f, 1f, 4f)));
            Assert.That(mover.IsMoving, Is.False);
        }
        finally
        {
            Object.DestroyImmediate(bomb);
        }
    }

    [Test]
    public void NewlyOccupiedTargetReturnsPartiallyMovedBombToSafeCell()
    {
        GameObject bomb = new GameObject("Partially moving bomb test");
        try
        {
            bomb.transform.position = new Vector3(4f, 1f, 4f);
            BomMover mover = bomb.AddComponent<BomMover>();
            mover.ReqMove(Vector3.right, 1);
            mover.Advance(0.2f, cell => false);
            Assert.That(bomb.transform.position.x, Is.GreaterThan(4f));

            mover.Advance(0.02f, cell => true);

            Assert.That(bomb.transform.position, Is.EqualTo(new Vector3(4f, 1f, 4f)));
            Assert.That(mover.IsMoving, Is.False);
        }
        finally
        {
            Object.DestroyImmediate(bomb);
        }
    }

    [Test]
    public void GridCollisionRecognizesBombsAndBothWallKinds()
    {
        GameObject candidate = new GameObject("Bom(Clone)");
        GameObject broken = new GameObject("Broken(Clone)");
        GameObject fixedWall = new GameObject("FixedWall(Clone)");
        GameObject player = new GameObject("Player1");
        try
        {
            Assert.That(Bom_Base_CollisionManager.IsBlockingObject(candidate), Is.True);
            Assert.That(Bom_Base_CollisionManager.IsBlockingObject(broken), Is.True);
            Assert.That(Bom_Base_CollisionManager.IsBlockingObject(fixedWall), Is.True);
            Assert.That(Bom_Base_CollisionManager.IsBlockingObject(player), Is.False);
        }
        finally
        {
            Object.DestroyImmediate(candidate);
            Object.DestroyImmediate(broken);
            Object.DestroyImmediate(fixedWall);
            Object.DestroyImmediate(player);
        }
    }

    [Test]
    public void BlockPlacementCompletionIsNotReportedAtCoroutineStart()
    {
        GameManager.xmax = 3;
        GameManager.zmax = 3;
        GameObject field = new GameObject("Broken block placement test");
        BrokenBlockManager manager = field.AddComponent<BrokenBlockManager>();
        bool completed = false;

        try
        {
            MethodInfo coroutineMethod = typeof(BrokenBlockManager).GetMethod(
                "AddBrokenBlockCoroutine", BindingFlags.Instance | BindingFlags.NonPublic);
            IEnumerator placement = (IEnumerator)coroutineMethod.Invoke(
                manager, new object[] { 1, new Action(() => completed = true) });

            Assert.That(placement.MoveNext(), Is.True);
            Assert.That(completed, Is.False,
                "配置イテレーターの開始直後にゲーム開始扱いにしてはいけません。");

            int stepCount = 0;
            while (placement.MoveNext() && stepCount++ < 100)
            {
            }

            Assert.That(completed, Is.True);
        }
        finally
        {
            Object.DestroyImmediate(field);
        }
    }
}
