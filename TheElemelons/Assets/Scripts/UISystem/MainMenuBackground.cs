using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackground : MonoBehaviour
{
    RectTransform witch;
    Vector3 witchAnchor;
    RectTransform magicCircle;
    Vector3 magicCircleAnchor;
    List<Transform> buttons;
    Vector3 mousePos;
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    void GetMousePos()
    {
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
    void Initialize()
    {
        witch = transform.GetChild(2).GetComponent<RectTransform>();
        magicCircle = transform.GetChild(1).GetComponent<RectTransform>();
        magicCircleAnchor = magicCircle.position;
        witchAnchor = witch.position;
        mainCamera = Camera.main;
        buttons = new List<Transform>();
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            Transform potentialButton = transform.parent.GetChild(i).transform;
            if (potentialButton.gameObject.name.Contains("Button"))
            {
                buttons.Add(potentialButton);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetMousePos();
        MoveBackground();
    }
    float f(float x, float min, float max)
    {
        return (Mathf.Atan(x) * (2 / Mathf.PI) * ((max - min) / 2)) + min;
    }
    float changeSpeed(float previous, float next, float speed)
    {
        float diff = next - previous;
        float maxChange = Time.fixedDeltaTime * speed;
        if (Mathf.Abs(diff) > maxChange)
        {
            diff = Mathf.Sign(diff) * maxChange;
        }
        return previous + diff;
    }
    void MoveBackground()
    {
        Vector3 moveFactor = new Vector3(300f, -200f, 0);
        float maxWitchScale = 1.5f;
        float minWitchScale = 1.2f;
        Vector3 diff = mousePos - mainCamera.ScreenToWorldPoint(buttons[0].transform.position);
        float witchScale = f(diff.x, minWitchScale, maxWitchScale);
        witchScale = changeSpeed(witch.localScale.x, witchScale, 1f);
        witch.localScale = new Vector3(witchScale, witchScale, 1);
        witch.position = witchAnchor + (witchScale - minWitchScale) / (maxWitchScale - minWitchScale) * moveFactor * (2f / 3f);
        magicCircle.position = magicCircleAnchor - (witchScale - 1f) / (maxWitchScale - 1f) * moveFactor;
        float circleScale = 1f - (maxWitchScale - witchScale);
        magicCircle.localScale = new Vector3(circleScale, circleScale, 1);
    }
}
