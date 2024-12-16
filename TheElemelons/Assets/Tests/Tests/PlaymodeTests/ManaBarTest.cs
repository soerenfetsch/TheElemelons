using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ManaBarTest
{
    private GameObject manaBarGameObject;
    private ManaBar manaBar;
    private Mana mana;

    [SetUp]
    public void Setup()
    {
        // Create ManaBar GameObject
        manaBarGameObject = new GameObject("ManaBar");
        manaBar = manaBarGameObject.AddComponent<ManaBar>();

        // Load prefab AnimatorController
#if UNITY_EDITOR
        var canvasPrefab = AssetDatabase.LoadAssetAtPath<Canvas>("Assets/UI_Canvas.prefab");
        Assert.NotNull(canvasPrefab, "UI_Canvas prefab not found at specified path.");
#endif
        Animator prefabHeartAnimator = canvasPrefab.transform.GetChild(0).GetComponent<Animator>();
        Assert.NotNull(prefabHeartAnimator, "Animator not found in the first child of UI_Canvas prefab.");

        // Add the Mana script
        mana = manaBarGameObject.AddComponent<Mana>();
        manaBar.manaScript = mana;

        // Initialize manaStars array
        manaBar.manaStars = new Image[5];
        for (int i = 0; i < manaBar.manaStars.Length; i++)
        {
            GameObject heartGameObject = new GameObject($"Heart{i}");
            Image heartImage = heartGameObject.AddComponent<Image>();
            Animator heartAnimator = heartGameObject.AddComponent<Animator>();

            heartAnimator.runtimeAnimatorController = prefabHeartAnimator.runtimeAnimatorController;

            heartGameObject.transform.SetParent(manaBarGameObject.transform, false);
            manaBar.manaStars[i] = heartImage;
        }

        // Create placeholder textures and sprites
        Texture2D placeholderTexture = new Texture2D(2, 2);
        manaBar.manaStar0 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);
        manaBar.manaStar1 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);
        manaBar.manaStar2 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);
        manaBar.manaStar3 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);
        manaBar.manaStar4 = Sprite.Create(placeholderTexture, new Rect(0, 0, 2, 2), Vector2.zero);

        manaBarGameObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(manaBarGameObject);
    }

    [UnityTest]
    public IEnumerator ManaBar_Hurt_TriggersHurtAnimation()
    {
        // Arrange
        mana.SetMana(3f);
        mana.SetMaxMana(5f);
        yield return null; // Wait for Start to initialize

        // Act
        mana.SubtractMana(1f); // Simulate taking damage
        yield return null; // Wait for Update to process

        // Assert
        foreach (var heart in manaBar.manaStars)
        {
            var animator = heart.GetComponent<Animator>();
            Assert.IsTrue(animator.GetBool("hurt"), "Hurt animation not triggered.");
        }
    }

    [UnityTest]
    public IEnumerator ManaBar_Heal_TriggersHealAnimation()
    {
        // Arrange
        mana.SetMana(2f);
        mana.SetMaxMana(5f);
        yield return null; // Wait for Start to initialize

        // Act
        mana.AddMana(1f); // Simulate healing
        yield return null; // Wait for Update to process

        // Assert
        foreach (var heart in manaBar.manaStars)
        {
            var animator = heart.GetComponent<Animator>();
            Assert.IsTrue(animator.GetBool("heal"), "Heal animation not triggered.");
        }
    }




    [UnityTest]
    public IEnumerator ManaBar_UpdatesHeartSpritesBasedOnMana()
    {
        // Arrange
        mana.SetMana(3.5f);
        mana.SetMaxMana(5f);
        mana.SetManaPerRegen(0f);
        yield return null; // Wait for Start to initialize

        // Act
        yield return null;
        // Assert
        for (int i = 0; i < manaBar.manaStars.Length; i++)
        {
            Debug.Log(i);
            if (i < 3)
            {
                Assert.AreEqual(manaBar.manaStar4, manaBar.manaStars[i].sprite, $"ManaStarSprite {i} sprite should be full.");
            }
            else if (i == 3)
            {
                Assert.AreNotEqual(manaBar.manaStar0, manaBar.manaStars[i].sprite, "is 0");
                Assert.AreNotEqual(manaBar.manaStar1, manaBar.manaStars[i].sprite, "is 1");
                Assert.AreNotEqual(manaBar.manaStar3, manaBar.manaStars[i].sprite, "is 3");
                Assert.AreNotEqual(manaBar.manaStar4, manaBar.manaStars[i].sprite, "is 4");
                Assert.AreEqual(manaBar.manaStar2, manaBar.manaStars[i].sprite, $"ManaStarSprite {i} sprite should be half full");
            }
            else
            {
                Assert.AreEqual(manaBar.manaStar0, manaBar.manaStars[i].sprite, $"ManaStarSprite {i} sprite should be empty.");
            }
        }
    }
}
