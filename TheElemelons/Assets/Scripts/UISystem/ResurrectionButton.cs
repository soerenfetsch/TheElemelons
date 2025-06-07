using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResurrectionButton : MonoBehaviour
{
    public DeathEffect deathEffect;
    Transform playerTransform;
    [Header("Hovering Motion")]
    public float amplitude = 20f;
    public float frequency = 0.25f;
    Vector2 fixpoint;

    void Awake()
    {
        InititializeResurrectButton();
        playerTransform = GameObject.Find("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        fixpoint = transform.position;
    }
    void OnEnable()
    {
        playerOnOff(false);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moving_by = Vector2.up * Mathf.Sin(frequency * Time.frameCount * Time.fixedDeltaTime) * amplitude;
        transform.position = fixpoint + moving_by;
    }

    private void InititializeResurrectButton()
    {
        Button button = gameObject.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("ResurrectionButton not found in SettingsMenuUI.");
            return;
        }
        // Add on click listener to return to last menu
        button.onClick.AddListener(() => OnResurrectButtonClicked());
    }
    void playerOnOff(bool onOff)
    {
        playerTransform.GetComponent<SpriteRenderer>().enabled = onOff;
        foreach (BoxCollider2D boxCollider2D in playerTransform.GetComponents<BoxCollider2D>())
        {
            boxCollider2D.enabled = onOff;
        }
        playerTransform.GetComponent<ToolTip>().ToggleToolTipOnOff(onOff);
        playerTransform.GetComponent<Player>().ModifyIsHidden(onOff ? -1 : 1);
        playerTransform.Find("Wand_Animation").gameObject.SetActive(onOff);
        if (!onOff)
        {
            Instantiate(deathEffect, playerTransform.position, Quaternion.Euler(0, 0, 0));
            playerTransform.GetComponent<Casting>().ClearCharges();
            BuffDebuffManager.Instance.ClearEffects(playerTransform.gameObject);
        }
    }
    private void OnResurrectButtonClicked()
    {
        GameManager.Instance.ContinueSavedGame();
        if (playerTransform != null)
        {
            playerOnOff(true);
        }
    }
}
