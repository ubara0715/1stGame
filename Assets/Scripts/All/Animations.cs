using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Animations : MonoBehaviour
{
    float time;
    /// <summary>
    ///  Updata専用関数、点滅させたいときに使うべし
    /// </summary>
    /// <returns>アルファ値が変わったColor</returns>
    protected Color Blink(Color color,float speed = 3.0f)
    {
        time += Time.deltaTime * speed * 3.0f;
        color.a = Mathf.Sin(time);

        return color;
    }

    /// <summary>
    /// イメージ専用のフェードアウトメソッド
    /// </summary>
    /// <param name="fadeObject">フェードアウトさせたいイメージ</param>
    /// <param name="time">かかる時間</param>
    protected IEnumerator FadeOut_image(Image fadeObject,float time = 1.0f)
    {
        float timer = 0.0f;
        Color startColor = fadeObject.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.0f);

        while(timer + 0.05f <= time)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / time);

            fadeObject.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeObject.color = endColor;
        yield break;
    }

    /// <summary>
    /// テキスト専用のフェードアウトメソッド
    /// </summary>
    /// <param name="fadeObject">フェードアウトさせたいテキスト</param>
    /// <param name="time">かかる時間</param>
    protected IEnumerator FadeOut_text(Text fadeObject, float time = 1.0f)
    {
        float timer = 0.0f;
        Color startColor = fadeObject.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.0f);

        while (timer + 0.05f <= time)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / time);

            fadeObject.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeObject.color = endColor;
        yield break;
    }
}
