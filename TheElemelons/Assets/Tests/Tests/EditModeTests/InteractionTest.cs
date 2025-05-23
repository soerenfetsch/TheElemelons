using NUnit.Framework;
using UnityEngine;
using UnityEditor.Animations;

public class InteractableTests
{
    private GameObject player;
    private PlayerInteraction playerInteraction;
    private GameObject interactableA;
    private GameObject interactableB;
    private GameObject interactableC;
    private LayerMask interactableLayer;
    private Animator playerAnimator;
    private string animatorControllerPath = "Assets/TestAnimatorController.controller";
    [SetUp]
    public void Setup()
    {
        // Create player object with PlayerInteraction script
        player = new GameObject("Player");
        playerInteraction = player.AddComponent<PlayerInteraction>();
        playerAnimator = player.AddComponent<Animator>();
        var animatorController = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);
        animatorController.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        playerAnimator.runtimeAnimatorController = animatorController;
        playerInteraction.playerAnimator = playerAnimator;
        player.transform.position = new Vector3(0, 0, 0);
        playerInteraction.InitializeInteractionBoxes();
        // Create a new LayerMask for interactables
        int interactableLayerIndex = LayerMask.NameToLayer("Interactable");
        playerInteraction.interactableLayer = LayerMask.GetMask("Interactable");

        // Create interactable objects, assign layer, and add colliders
        interactableA = CreateInteractable("InteractableA", typeof(TestInteractableA), new Vector2(1.5f, 0));
        interactableB = CreateInteractable("InteractableB", typeof(TestInteractableB), new Vector2(0, 1.5f));
        interactableC = CreateInteractable("InteractableC", typeof(TestInteractableC), new Vector2(3, 3));
    }
    private void SetPlayerDir(float horizontal, float vertical)
    {
        playerAnimator.SetFloat("Horizontal", horizontal);
        playerAnimator.SetFloat("Vertical", vertical);
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
    public void Interacts_With_Closest_Object_Right()
    {
        SetPlayerDir(1, 0);
        Assert.AreEqual(Vector2.right, new Vector2(playerAnimator.GetFloat("Horizontal"),
        playerAnimator.GetFloat("Vertical")), "Player looking in wrong direction instead of right " +
        playerAnimator.GetFloat("Horizontal").ToString() + " | " + playerAnimator.GetFloat("Vertical").ToString());
        player.transform.position = new Vector2(0, 0);
        playerInteraction.InteractWithClosestInteractable();
        var interactableAScript = interactableA.GetComponent<TestInteractableA>();
        Assert.IsTrue(interactableAScript.wasInteracted, "The closest interactable in facing direction (TestInteractableA) was not interacted with.");
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
