using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HealthTest
{
    private GameObject healthGameObject;
    private Health health;

    [SetUp]
    public void Setup()
    {
        healthGameObject = new GameObject("HealthObject");
        health = healthGameObject.AddComponent<Health>();

        health.maxhealth = 3f;
        health.health = health.maxhealth;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(healthGameObject);
    }

    [Test]
    public void SubtractHealth_DecreasesHealthByDamageAmount()
    {
        float damage = 1f;

        health.SubtractHealth(damage);

        Assert.AreEqual(2f, health.GetHealth(), "SubtractHealth did not decrease health correctly.");
    }

    [Test]
    public void SubtractHealth_AllHealthSubtracted_SetsHealthToZero()
    {
        float damage = 3f;

        health.SubtractHealth(damage);

        Assert.AreEqual(0f, health.GetHealth(), "Health should be zero after subtracting all health.");
    }

    [Test]
    public void GetHealth_ReturnsCorrectHealthValue()
    {
        health.health = 2.5f;

        float currentHealth = health.GetHealth();

        Assert.AreEqual(2.5f, currentHealth, "GetHealth did not return the correct health value.");
    }

    [Test]
    public void IsZero_ReturnsTrueWhenHealthIsZero()
    {
        health.health = 0f;

        bool isZero = health.IsZero();

        Assert.IsTrue(isZero, "IsZero should return true when health is zero.");
    }

    [Test]
    public void IsZero_ReturnsTrueWhenHealthIsBelowZero()
    {
        health.health = -1f;

        bool isZero = health.IsZero();

        Assert.IsTrue(isZero, "IsZero should return true when health is below zero.");
    }

    [Test]
    public void IsZero_ReturnsFalseWhenHealthIsAboveZero()
    {
        health.health = 1f;

        bool isZero = health.IsZero();

        Assert.IsFalse(isZero, "IsZero should return false when health is above zero.");
    }

    [Test]
    public void AddHealth_IncreasesHealthByGivenAmount()
    {
        float healthToAdd = 1f;
        health.health = 1f;
        health.AddHealth(healthToAdd);

        Assert.AreEqual(2f, health.GetHealth(), "AddHealth did not increase health correctly.");
    }

    [Test]
    public void AddHealth_DoesntAllowHealthToExceedMaxHealth()
    {
        float healthToAdd = 2f;
        health.health = health.maxhealth;
        health.AddHealth(healthToAdd);

        Assert.AreEqual(health.GetHealth(), health.maxhealth, "AddHealth shouldnt allow health to exceed maxhealth in the current implementation.");
    }

    [Test]
    public void SubtractHealth_CantGoBelowZero()
    {
        float damage = 2f;
        health.health = 1f;
        health.SubtractHealth(damage);

        Assert.AreEqual(0f, health.GetHealth(), "SubtractHealth did not decrease health correctly.");
    }
}
