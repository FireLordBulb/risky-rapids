using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SpriteFromAtlas : MonoBehaviour
{
    private Image image;
    [SerializeField] SpriteAtlas atlas;
    [SerializeField] string spriteName;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = atlas.GetSprite(spriteName);
    }
    
    public void ChangeSprite(string spriteName) 
    {
        image.sprite = atlas.GetSprite(spriteName);
    }
}
