using System;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using TMPro;

namespace DMSH.Misc.Animated
{
    public static class BasicAnimationsPack
    {
        public static IEnumerator SmoothAwakeSprite(SpriteRenderer sprite, float speed = 8.0f, float neededAlpha = 0.0f, float seconds = 0.01f)
        {
            neededAlpha = (neededAlpha == 0.0f) ? sprite.color.a : neededAlpha;
            sprite.color = sprite.color.a >= 0.9f ? new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.0f) : sprite.color;
            while (sprite.color.a <= neededAlpha)
            {
                sprite.color = Color.Lerp(sprite.color, new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1.0f), Time.deltaTime * speed);
                yield return new WaitForSeconds(seconds);
            }
        }

        [Obsolete("Please, use TextMeshPro")]
        public static IEnumerator SmoothAwakeText(Text text, float neededAlpha = 0.0f, float speed = 15.0f, float seconds = 0.01f)
        {
            neededAlpha = (neededAlpha == 0.0f) ? text.color.a : neededAlpha;
            float alpha = 0.0f;
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
            text.gameObject.SetActive(true);
            while (alpha <= neededAlpha)
            {
                text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, neededAlpha), Time.deltaTime * speed);
                yield return new WaitForSeconds(seconds);
            }
        }
        
        public static IEnumerator SmoothAwakeText(TMP_Text text, float neededAlpha = 0.0f, float speed = 15.0f, float seconds = 0.01f)
        {
            neededAlpha = (neededAlpha == 0.0f) ? text.color.a : neededAlpha;
            float alpha = 0.0f;
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
            text.gameObject.SetActive(true);
            while (alpha <= neededAlpha)
            {
                text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, neededAlpha), Time.deltaTime * speed);
                yield return new WaitForSeconds(seconds);
            }
        }
        
        public static IEnumerator SmoothFadeText(TMP_Text text, float speed = 35.0f, float seconds = 0.01f)
        {
            float alpha = text.color.a;
            while (alpha >= 0.0f)
            {
                text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 0.0f), Time.deltaTime * speed);
                yield return new WaitForSeconds(seconds);
            }

            text.gameObject.SetActive(false);
        }

        public static IEnumerator SmoothResize(Transform transform, float resize_to = 1.15f, float intensive = 40.0f, float seconds = 0.001f)
        {
            Vector3 resize_vec = new Vector3(resize_to, resize_to, resize_to);
            while (transform.localScale != resize_vec)
            {
                Vector3 changed_scale = Vector3.Lerp(transform.localScale, resize_vec, Time.deltaTime * intensive);
                transform.localScale = changed_scale;
                yield return new WaitForSeconds(seconds);
            }
        }

        public static IEnumerator SmoothChangeToColor(Color color, Color to_color, float intensive = 40.0f, float seconds = 0.001f)
        {
            while (color != to_color)
            {
                color = Color.Lerp(color, to_color, Time.deltaTime * intensive);
                yield return new WaitForSeconds(seconds);
            }
        }
        
        public static IEnumerator SmoothChangeToColorForText(TMP_Text text, Color to_color, float intensive = 40.0f, float seconds = 0.001f)
        {
            while (text.color != to_color)
            {
                Color color = Color.Lerp(text.color, to_color, Time.deltaTime * intensive);
                text.color = color;
                yield return new WaitForSeconds(seconds);
            }
        }
    }
}