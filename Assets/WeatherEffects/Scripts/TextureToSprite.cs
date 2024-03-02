using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureToSprite
{
    public static Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
