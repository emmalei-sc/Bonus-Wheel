using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliceReferences : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshPro TMPComponent;

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetText(string text)
    {
        TMPComponent.text = text;
    }
}
