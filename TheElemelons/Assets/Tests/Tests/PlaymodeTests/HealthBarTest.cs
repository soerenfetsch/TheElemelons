using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HealthBarTests
{
    private GameObject healthBarGameObject;
    private HealthBar healthBar;
    private Health health;

    [SetUp]
    public void Setup()
    {
        // Create HealthBar GameObject
        healthBarGameObject = new GameObject("HealthBar");
        healthBar = healthBarGameObject.AddComponent<HealthBar>();

        // Load prefab AnimatorController
#if UNITY_EDITOR
        var canvasPrefab = AssetDatabase.LoadAssetAtPath<Canvas>("Assets/UI_Canvas.prefab");
        Assert.NotNull(canvasPrefab, "UI_Canvas prefab not found at specified path.");
#endif
        Animator prefabHeartAnimator = canvasPrefab.transform.GetChild(0).GetComponent<Animator>();
        Assert.NotNull(prefabHeartAnimator, "Animator not found in the first child of UI_Canvas prefab.");

        // Add the Health script
        health = healthBarGameObject.AddComponent<Health>();
        health.SetHealth(5f);
        health.SetMaxHealth(5f);
        // Initialize hearts array
        healthBar.hearts = new Image[5];
        for (int i = 0; i < healthBar.hearts.Length; i++)
        {
            GameObject heartGameObject = new GameObject($"Heart{i}");
            Image heartImage = heartGameObject.AddComponent<Image>();
            Animator heartAnimator = heartGameObject.AddComponent<Animator>();

            heartAnimator.runtimeAnimatorController = prefabHeartAnimator.runtimeAnimatorController;

            heartGameObject.transform.SetParent(healthBarGameObject.transform, false);
            healthBar.hearts[i] = heartImage;
        }


        healthBarGameObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(healthBarGameObject);
    }

    [Test]
    public void HealthBar_Hurt_TriggersHurtAnimation()
    {
        healthBar.Hurt();
        // Assert
        foreach (var heart in healthBar.hearts)
        {
            var animator = heart.GetComponent<Animator>();
            Assert.IsTrue(animator.GetBool("hurt"), "Hurt animation not triggered.");
        }
    }

    [Test]
    public void HealthBar_Heal_TriggersHealAnimation()
    {
        
        healthBar.Heal();

        // Assert
        foreach (var heart in healthBar.hearts)
        {
            var animator = heart.GetComponent<Animator>();
            Assert.IsTrue(animator.GetBool("heal"), "Heal animation not triggered.");
        }
    }

    

    [UnityTest]
    public IEnumerator HealthBar_UpdatesHeartSpritesBasedOnHealth()
    {
        yield return null; // Wait for Start to initialize
        
        healthBar.UpdateHeartFullness(3.5f,5f);
        // Act
        yield return null;

        // Assert
        for (int i = 0; i < healthBar.hearts.Length; i++)
        {
            if (i < 3)
            {
                Assert.AreEqual(healthBar.hearts[i].GetComponent<Animator>().GetFloat("fullness"), 1f, $"Heart {i} sprite should be full.");
            }
            else if (i == 3)
            {
                Assert.AreEqual(healthBar.hearts[i].GetComponent<Animator>().GetFloat("fullness"), 0.5f ,$"Heart {i} sprite should be half full.");
            }
            else
            {
                Assert.AreEqual(healthBar.hearts[i].GetComponent<Animator>().GetFloat("fullness"), 0f, $"Heart {i} sprite should be empty.");
            }
        }
    }
}

