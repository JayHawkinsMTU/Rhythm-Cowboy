using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetronomeMarker : MonoBehaviour
{
    Image img;
    Color initColor;
    RectTransform rectTransform;
    RectTransform backing;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        backing = transform.parent.GetComponent<RectTransform>();
        img = GetComponent<Image>();
        initColor = img.color;
    }
    public void Track(float pos)
    {  
        rectTransform.anchoredPosition = new Vector2((backing.rect.width / -2) + backing.rect.width * pos, 0);

    }
    public void Pulse(float length)
    {
        StartCoroutine(Pulsate(length));
    }

    private IEnumerator Pulsate(float length)
    {
        transform.localScale = new Vector3(1.1f, 1.1f, 1);
        img.color = Color.white;
        yield return new WaitForSeconds(length);
        transform.localScale = new Vector3(1, 1, 1);
        img.color = initColor;
    }
}
