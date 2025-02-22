using NUnit.Framework;
using UnityEngine;

public class InteractableTests
{
    private GameObject player;
    private PlayerInteraction playerInteraction;
    private GameObject interactableA;
    private GameObject interactableB;
    private GameObject interactableC;
    private LayerMask interactableLayer;

    [SetUp]
    public void Setup()
    {
        // Create player object with PlayerInteraction script
        player = new GameObject("Player");
        playerInteraction = player.AddComponent<PlayerInteraction>();
        playerInteraction.interactionRange = 5f; // Test range

        // Create a new LayerMask for interactables
        int interactableLayerIndex = LayerMask.NameToLayer("Interactable");
        playerInteraction.interactableLayer = LayerMask.GetMask("Interactable");

        // Create interactable objects, assign layer, and add colliders
        interactableA = CreateInteractable("InteractableA", typeof(TestInteractableA), new Vector2(1, 1));
        interactableB = CreateInteractable("InteractableB", typeof(TestInteractableB), new Vector2(2, 2));
        interactableC = CreateInteractable("InteractableC", typeof(TestInteractableC), new Vector2(3, 3));
    }

    private GameObject CreateInteractable(string name, System.Type type, Vector2 position)
    {
        GameObject obj = new GameObject(name);
        obj.layer = LayerMask.NameToLayer("Interactable");
        obj.transform.position = position;
        obj.AddComponent(type);
        
        // Ensure the object has a Collider2D
        obj.AddComponent<CircleCollider2D>().isTrigger = true;
        
        return obj;
    }

    [Test]
    public void Interacts_With_Closest_Object()
    {
        player.transform.position = new Vector2(0, 0);

        playerInteraction.InteractWithClosestInteractable();

        var interactableAScript = interactableA.GetComponent<TestInteractableA>();
        Assert.IsTrue(interactableAScript.wasInteracted, "The closest interactable (TestInteractableA) was not interacted with.");
    }

    [Test]
    public void Interaction_Count_Increments()
    {
        player.transform.position = new Vector2(3, 3);

        var interactableCScript = interactableC.GetComponent<TestInteractableC>();

        playerInteraction.InteractWithClosestInteractable();
        playerInteraction.InteractWithClosestInteractable();

        Assert.AreEqual(2, interactableCScript.interactionCount, "Interaction count did not increment correctly.");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(interactableA);
        Object.DestroyImmediate(interactableB);
        Object.DestroyImmediate(interactableC);
    }
}
