using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BuffDebuffManagerTests
{
    private GameObject mockTarget;
    private BuffDebuffManager manager;

    [SetUp]
    public void Setup()
    {
        mockTarget = new GameObject();
        mockTarget.AddComponent<TestBuffable>();
        manager = new GameObject("BuffDebuffManager").AddComponent<BuffDebuffManager>();
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(mockTarget);
        GameObject.DestroyImmediate(manager.gameObject);
    }

    [Test]
    public void ApplyBuff_AddsBuffToTarget()
    {
        manager.ApplyBuffDebuff(mockTarget, BuffDebuffManager.BuffDebuffType.SpeedBuff, 5f, 2f);

        var effects = manager.GetActiveEffects(mockTarget);
        Assert.AreEqual(1, effects.Count);
        Assert.AreEqual(BuffDebuffManager.BuffDebuffType.SpeedBuff, effects[0].type);
    }

    [UnityTest]
    public IEnumerator BuffExpires_RemovesBuffFromTarget()
    {
        manager.ApplyBuffDebuff(mockTarget, BuffDebuffManager.BuffDebuffType.DamageBuff, 1f, 1.5f);

        Assert.AreEqual(1, manager.GetActiveEffects(mockTarget).Count);
        
        // UnityTest will wait properly for the seconds
        yield return new WaitForSeconds(1.1f);

        Assert.AreEqual(0, manager.GetActiveEffects(mockTarget).Count);
    }

    [Test]
    public void MultipleBuffs_CanBeStacked()
    {
        manager.ApplyBuffDebuff(mockTarget, BuffDebuffManager.BuffDebuffType.SpeedBuff, 5f, 2f);
        manager.ApplyBuffDebuff(mockTarget, BuffDebuffManager.BuffDebuffType.DamageBuff, 5f, 1.5f);

        var effects = manager.GetActiveEffects(mockTarget);
        Assert.AreEqual(2, effects.Count);
    }
}
