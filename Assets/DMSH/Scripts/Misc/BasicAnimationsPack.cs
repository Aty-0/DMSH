using Unity;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class BasicAnimationsPack
{
    public static IEnumerator SmoothAwakeText(Text text, float speed = 15.0f, float seconds = 0.01f)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
        text.gameObject.SetActive(true);

        float alpha = 0.0f;
        while (alpha <= 255.0f)
        {
            alpha += speed * Time.deltaTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return new WaitForSeconds(seconds);
        }
    }

    public static IEnumerator SmoothFadeText(Text text, float speed = 35.0f, float seconds = 0.01f)
    {
        float alpha = text.color.a;
        while (alpha >= 0.0f)
        {
            alpha -= speed * Time.deltaTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
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

    public static IEnumerator SmoothChangeToColorForText(Text text, Color to_color, float intensive = 40.0f, float seconds = 0.001f)
    {
        while (text.color != to_color)
        {
            Color color = Color.Lerp(text.color, to_color, Time.deltaTime * intensive);
            text.color = color;
            yield return new WaitForSeconds(seconds);
        }
    }
}

