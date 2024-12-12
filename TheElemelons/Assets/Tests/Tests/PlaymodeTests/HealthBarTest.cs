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
        healthBar.healthScript = health;

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

        // Create placeholder textures and sprites
        Texture2D placeholderTexture = new Texture2D(2, 2);
        healthBar.heart0 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);
        healthBar.heart1 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);
        healthBar.heart2 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);
        healthBar.heart3 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);
        healthBar.heart4 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);

        healthBarGameObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(healthBarGameObject);
    }
    
    [UnityTest]
    public IEnumerator HealthBar_Hurt_TriggersHurtAnimation()
    {
        // Arrange
        health.SetHealth(3f);
        health.SetMaxHealth(5f);
        yield return null; // Wait for Start to initialize

        // Act
        health.SubtractHealth(1f); // Simulate taking damage
        yield return null; // Wait for Update to process
        
        // Assert
        foreach (var heart in healthBar.hearts)
        {
            var animator = heart.GetComponent<Animator>();
            Assert.IsTrue(animator.GetBool("hurt"), "Hurt animation not triggered.");
        }
    }

    [UnityTest]
    public IEnumerator HealthBar_Heal_TriggersHealAnimation()
    {
        // Arrange
        health.SetHealth(2f);
        health.SetMaxHealth(5f);
        yield return null; // Wait for Start to initialize

        // Act
        health.AddHealth(1f); // Simulate healing
        yield return null; // Wait for Update to process

        // Assert
        foreach (var heart in healthBar.hearts)
        {
            var animator = heart.GetComponent<Animator>();
            Assert.IsTrue(animator.GetBool("heal"), "Heal animation not triggered.");
        }
    }

    [UnityTest]
    public IEnumerator HealthBar_Dead_SetsFullnessToZero()
    {
        // Arrange
        health.SetHealth(0f);
        health.SetMaxHealth(5f);
        yield return null; // Wait for Start to initialize

        // Act
        healthBar.Dead();
        yield return null;

        // Assert
        foreach (var heart in healthBar.hearts)
        {
            var fullness = heart.GetComponent<Animator>().GetFloat("fullness");
            Assert.AreEqual(0f, fullness, "Fullness should be set to 0 for all hearts.");
        }
    }

    [UnityTest]
    public IEnumerator HealthBar_UpdatesHeartSpritesBasedOnHealth()
    {
        // Arrange
        health.SetHealth(3.5f);
        health.SetMaxHealth(5f);
        yield return null; // Wait for Start to initialize

        // Act
        yield return null;

        // Assert
        for (int i = 0; i < healthBar.hearts.Length; i++)
        {
            if (i < 3)
            {
                Assert.AreEqual(healthBar.heart4, healthBar.hearts[i].sprite, $"Heart {i} sprite should be full.");
            }
            else if (i == 3)
            {
                Assert.AreEqual(healthBar.heart2, healthBar.hearts[i].sprite, $"Heart {i} sprite should be half full.");
            }
            else
            {
                Assert.AreEqual(healthBar.heart0, healthBar.hearts[i].sprite, $"Heart {i} sprite should be empty.");
            }
        }
    }
}

