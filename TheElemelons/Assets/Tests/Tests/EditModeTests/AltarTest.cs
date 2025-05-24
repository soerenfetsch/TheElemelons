using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AltarTest
{
    private GameObject player;
    private PlayerInteraction playerInteraction;
    private Altar altarScript;
    private GameObject altar;
    private LayerMask interactableLayer;

    [SetUp]
    public void Setup()
    {
        // Create player object with PlayerInteraction script
        player = new GameObject("Player");
        playerInteraction = player.AddComponent<PlayerInteraction>();

        // Create a new LayerMask for interactables
        int interactableLayerIndex = LayerMask.NameToLayer("Interactable");
        playerInteraction.interactableLayer = LayerMask.GetMask("Interactable");

        // Create interactable objects, assign layer, and add colliders
        altar = new GameObject("Altar");
        altarScript = altar.AddComponent<Altar>();
        altar.AddComponent<Animator>();
        altar.AddComponent<Rigidbody2D>();
        altar.GetComponent<Rigidbody2D>().isKinematic = true;
        altar.AddComponent<BoxCollider2D>();
        altarScript.collectableHeart = new GameObject("CollectableHeartMock");
        altarScript.collectableMaxHeart = new GameObject("CollectableMaxHeartMock");
        altarScript.collectableMaxMana = new GameObject("CollectableMaxManaMock");
        altarScript.collectablePinkPotion = new GameObject("CollectablePinkPotionMock");
        altarScript.InitializeLists();
    }
    [Test]
    public void TestChoseLootHeart()
    {
        altarScript.blood = true;
        altarScript.gold = false;
        List<GameObject> hearts = altarScript.ChoseLoot(spawn: false);
        Assert.True(hearts.Count > 0, "empty lootlist");
        for (int i = 0; i < hearts.Count; i++)
        {
            Assert.AreEqual(hearts[i].name, "CollectableHeartMock", "spawned " + hearts[0].name + " instead of CollectableHeartMock");
        }
        Assert.True(hearts.Count < altarScript.maxSpawnableHearts, " spawned too many hearts");
    }
    [Test]
    public void TestChoseLootMaxHeart()
    {
        altarScript.blood = true;
        altarScript.gold = true;
        List<GameObject> hearts = altarScript.ChoseLoot(spawn: false);
        Assert.True(hearts.Count == 1, "lootlist length should be 1 but is " + hearts.Count);
        for (int i = 0; i < hearts.Count; i++)
        {
            Assert.AreEqual(hearts[i].name, "CollectableMaxHeartMock", "spawned " + hearts[0].name + " instead of CollectableMaxHeartMock");
        }
    }
    [Test]
    public void TestChoseLootMaxMana()
    {
        altarScript.blood = false;
        altarScript.gold = true;
        List<GameObject> hearts = altarScript.ChoseLoot(spawn: false);
        Assert.True(hearts.Count == 1, "lootlist length should be 1 but is " + hearts.Count);
        for (int i = 0; i < hearts.Count; i++)
        {
            Assert.AreEqual(hearts[i].name, "CollectableMaxManaMock", "spawned " + hearts[0].name + " instead of CollectableMaxManaMock");
        }
    }
    [Test]
    public void TestChoseLootPinkPotion()
    {
        altarScript.blood = false;
        altarScript.gold = false;
        List<GameObject> hearts = altarScript.ChoseLoot(spawn: false);
        Assert.True(hearts.Count == 1, "lootlist length should be 1 but is " + hearts.Count);
        for (int i = 0; i < hearts.Count; i++)
        {
            Assert.AreEqual(hearts[i].name, "CollectablePinkPotionMock", "spawned " + hearts[0].name + " instead of CollectablePinkPotionMock");
        }
    }
}
