using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BuffDebuffEffectTests
{
    private GameObject mockTarget;
    private BuffDebuffManager manager;
    private TestBuffable target;

    [SetUp]
    public void Setup()
    {
        mockTarget = new GameObject();
        target = mockTarget.AddComponent<TestBuffable>();
        manager = new GameObject("BuffDebuffManager").AddComponent<BuffDebuffManager>();
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(mockTarget);
        GameObject.DestroyImmediate(manager.gameObject);
    }

    [Test]
    public void SpeedBuff_IncreasesSpeed()
    {
        float originalSpeed = target.speed;

        manager.ApplyBuffDebuff(mockTarget, BuffDebuffManager.BuffDebuffType.SpeedBuff, 5f, 1.5f);
        Assert.AreEqual(originalSpeed * 1.5f, target.speed);
    }

    [UnityTest]
    public IEnumerator SpeedBuff_ExpiresRevertsSpeed()
    {
        float originalSpeed = target.speed;

        manager.ApplyBuffDebuff(mockTarget, BuffDebuffManager.BuffDebuffType.SpeedBuff, 1f, 1.5f);
        Assert.AreEqual(originalSpeed * 1.5f, target.speed);

        // Simulate wait time (buff duration + slight buffer)
        yield return new WaitForSeconds(1.1f);

        Assert.AreEqual(originalSpeed, target.speed);
    }
}
