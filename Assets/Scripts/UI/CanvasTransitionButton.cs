using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTransitionButton : MonoBehaviour
{
    [SerializeField] GameObject fromCanvas;
    [SerializeField] GameObject toCanvas;
    public void OnClick()
    {
        toCanvas.SetActive(true);
        fromCanvas.SetActive(false);
    }
}
