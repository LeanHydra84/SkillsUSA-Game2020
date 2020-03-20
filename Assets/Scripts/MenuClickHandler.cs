using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuClickHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public bool Locked;

    private protected readonly Color based = Color.black;
    public Color newColor;

    public virtual void Clicked()
    {
        print("You are gay");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Locked)
            GetComponent<Text>().color = newColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Text>().color = based;
    }
}
