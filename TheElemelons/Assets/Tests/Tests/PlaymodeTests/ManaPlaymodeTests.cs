using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ManaPlaymodeTests
{
    private GameObject manaGameObject;
    private Mana mana;

    [SetUp]
    public void Setup()
    {
        // Create a new GameObject and attach the Mana script
        manaGameObject = new GameObject("ManaObject");
        mana = manaGameObject.AddComponent<Mana>();

        // Initialize variables
        mana.maxMana = 5f;
        mana.mana = 0f; // Start at 0 to test regeneration
        mana.manaPerRegen = 0.25f;
        mana.manaRegenTime = 0.2f;
        mana.current_regenTime = mana.manaRegenTime;
        mana.justCasted = false;
        mana.gameRunning = true;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(manaGameObject);
    }

    [UnityTest]
    public IEnumerator ManaRegen_RegeneratesManaOverTime()
    {
        //Start the ManaRegen coroutine
        mana.StartCoroutine(mana.ManaRegen());

        yield return new WaitForSeconds(1f);

        // Assert: Mana should have regenerated
        Assert.Greater(mana.GetMana(), 0f, "Mana did not regenerate over time.");
    }

    [UnityTest]
    public IEnumerator ManaRegen_StopsAtMaxMana()
    {
        //Start the ManaRegen coroutine
        mana.mana = 0f;
        mana.StartCoroutine(mana.ManaRegen());

        yield return new WaitForSeconds(5f);

        // Assert: Mana should not exceed maxMana
        Assert.AreEqual(mana.maxMana, mana.GetMana(), "Mana exceeded maxMana.");
    }
}