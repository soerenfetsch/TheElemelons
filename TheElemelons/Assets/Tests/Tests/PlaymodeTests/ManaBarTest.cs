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
        var canvasPrefab = AssetDatabase.LoadAssetAtPath<Canvas>("Assets/Prefabs/UIPrefabs/UI_Canvas.prefab");
        Assert.NotNull(canvasPrefab, "UI_Canvas prefab not found at specified path.");
#endif
        Animator prefabHeartAnimator = canvasPrefab.transform.GetChild(0).GetComponent<Animator>();
        Assert.NotNull(prefabHeartAnimator, "Animator not found in the first child of UI_Canvas prefab.");

        // Add the Mana script
        mana = manaBarGameObject.AddComponent<Mana>();

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

        manaBarGameObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(manaBarGameObject);
    }

    [Test]
    public void ManaBar_Hurt_TriggersHurtAnimation()
    {
        manaBar.Hurt();

        // Assert
        foreach (var heart in manaBar.manaStars)
        {
            var animator = heart.GetComponent<Animator>();
            Assert.IsTrue(animator.GetBool("hurt"), "Hurt animation not triggered.");
        }
    }

    [Test]
    public void ManaBar_Heal_TriggersHealAnimation()
    {
        manaBar.Heal();
        // Assert
        foreach (var heart in manaBar.manaStars)
        {
            var animator = heart.GetComponent<Animator>();
            Assert.IsTrue(animator.GetBool("heal"), "Heal animation not triggered.");
        }
    }




    [UnityTest]
    public IEnumerator HealthBar_UpdatesHeartSpritesBasedOnHealth()
    {
        yield return null; // Wait for Start to initialize
        
        manaBar.UpdateStarFullness(3.5f,5f);
        // Act
        yield return null;

        // Assert
        for (int i = 0; i <manaBar.manaStars.Length; i++)
        {
            if (i < 3)
            {
                Assert.AreEqual(manaBar.manaStars[i].GetComponent<Animator>().GetFloat("fullness"), 1f, $"Heart {i} sprite should be full.");
            }
            else if (i == 3)
            {
                Assert.AreEqual(manaBar.manaStars[i].GetComponent<Animator>().GetFloat("fullness"), 0.5f ,$"Heart {i} sprite should be half full.");
            }
            else
            {
                Assert.AreEqual(manaBar.manaStars[i].GetComponent<Animator>().GetFloat("fullness"), 0f, $"Heart {i} sprite should be empty.");
            }
        }
    }
}
