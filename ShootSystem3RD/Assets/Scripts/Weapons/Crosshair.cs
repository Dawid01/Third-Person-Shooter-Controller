using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{

    private RectTransform rect;
    public float baseSize = 60f;
    public float recoil;
    private List<Image> images = new List<Image>();
    public Color hitColor;
    private Color normalColor;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        for (int i = 0; i < transform.childCount; i++) {
            images.Add(transform.GetChild(i).GetComponent<Image>());
        }
        normalColor = images[0].color;
    }

    void Update()
    {
        float size = baseSize + (recoil * baseSize)/2f;
        size = Mathf.Clamp(size, baseSize, 1000f);
        rect.sizeDelta = new Vector2(size, size);
    }

    public void HitEnemy() {
        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor() {
        foreach (Image img in images) {
            img.color = hitColor;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (Image img in images)
        {
            img.color = normalColor;
        }
    } 
}
