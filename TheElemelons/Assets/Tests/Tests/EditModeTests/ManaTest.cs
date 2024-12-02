using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class ManaTest
{
    private GameObject manaGameObject;
    private Mana mana;

    [SetUp]
    public void Setup()
    {
        manaGameObject = new GameObject("ManaObject");
        mana = manaGameObject.AddComponent<Mana>();

        mana.maxMana = 5f;
        mana.mana = mana.maxMana;
        mana.manaPerRegen = 0.25f;
        mana.manaRegenTime = 0.2f;
        mana.current_regenTime = mana.manaRegenTime;
        mana.justCasted = false;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(manaGameObject);
    }

    [Test]
    public void SubtractMana_DecreasesManaCorrectly()
    {
        float manaCost = 2f;

        mana.SubtractMana(manaCost);

        Assert.AreEqual(3f, mana.GetMana(), "SubtractMana did not decrease mana correctly.");
    }

    [Test]
    public void SubtractMana_DoesNotGoBelowZero()
    {
        float manaCost = 10f;

        mana.SubtractMana(manaCost);

        Assert.AreEqual(0f, mana.GetMana(), "SubtractMana allowed mana to go below zero.");
    }

    [Test]
    public void AddMana_IncreasesManaCorrectly()
    {
        float manaMultiplier = 2f;
        mana.mana = 2f;

        mana.AddMana(manaMultiplier);

        Assert.AreEqual(4f, mana.GetMana(), "AddMana did not multiply mana correctly.");
    }

    [Test]
    public void AddMana_DoesntExceedsMaxMana()
    {
        float manaPlus = 2f;
        mana.mana = mana.maxMana;
        mana.AddMana(manaPlus);

        Assert.AreEqual(mana.GetMana(), mana.maxMana, "AddMana shouldnt allow mana to exceed maxMana in current implementation.");
    }

    [Test]
    public void GetMana_ReturnsCorrectValue()
    {
        mana.mana = 3f;

        float currentMana = mana.GetMana();

        Assert.AreEqual(3f, currentMana, "GetMana did not return the correct mana value.");
    }

    [Test]
    public void IsZero_ReturnsTrueWhenManaIsZero()
    {
        mana.mana = 0f;

        bool isZero = mana.IsZero();

        Assert.IsTrue(isZero, "IsZero should return true when mana is zero.");
    }

    [Test]
    public void IsZero_ReturnsFalseWhenManaIsAboveZero()
    {
        mana.mana = 1f;

        bool isZero = mana.IsZero();

        Assert.IsFalse(isZero, "IsZero should return false when mana is above zero.");
    }
    [Test]
    public void SubtractMana_CantGoBelowZero()
    {
        float manaCost = 2f;
        mana.mana = 1f;
        mana.SubtractMana(manaCost);

        Assert.AreEqual(0f, mana.GetMana(), "SubtractHealth did not decrease health correctly.");
    }
}
