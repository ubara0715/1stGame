using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CutInManager : Animations,IEventSystemHandler
{
    [Header("アニメーション用")]
    [SerializeField] List<GameObject> animaObjects = new List<GameObject>();
    [SerializeField] List<GameObject> markers = new List<GameObject>();

    Dictionary<string, Vector2> startPos = new Dictionary<string, Vector2>();
    Dictionary<string, Vector2> endPos = new Dictionary<string, Vector2>();

    void Start()
    {
        // GoogleのAI君はポンコツ
        // はじまりと終わりの位置をそれぞれオブジェクト名と同期させて辞書に記録
        for (int i = 0; i < animaObjects.Count; i++)
        {
            GameObject addValue = animaObjects[i];
            startPos.Add(addValue.name, addValue.GetComponent<RectTransform>().anchoredPosition);
        }
        for (int i = 0; i < markers.Count; i++)
        {
            GameObject addValue = markers[i];
            endPos.Add(addValue.name, addValue.GetComponent<RectTransform>().anchoredPosition);
        }
    }

    //はじめからスタートさせる場合には、Set()を使用
    IEnumerator AnimaStart = null;
    void Set_InAnima()
    {
        AnimaStart = InAnimation();
    }

    //はじめからスタートさせる場合には、Set()を使用
    IEnumerator AnimaEnd = null;
    void Set_OutAnima()
    {
        AnimaEnd = OutAnimation();
    }

    // オンになったとき
    void OnEnable()
    {
        // 再スタートように新しく取得
        Set_InAnima();
        Set_OutAnima();

        StartCoroutine(AnimaStart);
    }

    // オフになったとき
    void OnDisable()
    {
        StopAllCoroutines();
    }

    // キャンセルボタン用
    public void OnCancel()
    {
        StopAllCoroutines();

        StartCoroutine(AnimaEnd);
    }

    

    /// <summary>
    /// インするときのアニメーション
    /// </summary>
    IEnumerator InAnimation()
    {
        yield return null;

        for(int i = 0;i < animaObjects.Count; i++)
        {
            RectTransform setObject = animaObjects[i].GetComponent<RectTransform>();
            setObject.anchoredPosition = startPos[setObject.name];
        }

        yield return null;

        IEnumerator anima01 = InAnima_move(0, 0.02f);
        StartCoroutine(anima01);

        yield return new WaitForSeconds(0.1f);

        IEnumerator anima03 = InAnima_move(2, 0.02f);
        StartCoroutine(anima03);

        IEnumerator anima04 = InAnima_move(3, 0.02f);
        StartCoroutine(anima04);

        yield return new WaitForSeconds(0.3f);

        IEnumerator anima02 = InAnima_move(1, 0.01f);
        StartCoroutine(anima02);

        yield return new WaitForSeconds(2.5f);

        StopCoroutine(anima01);
        StopCoroutine(anima02);
        StopCoroutine(anima03);
        StopCoroutine(anima04);

        yield break;
    }
    /// <summary>
    /// マークまで移動させる専用のメソッド
    /// </summary>
    /// <param name="number">移動させたい、辞書に保存したオブジェクトの登録番号</param>
    /// <param name="slideSpeed">スピード</param>
    IEnumerator InAnima_move(int number,float slideSpeed)
    { 
        string moveObject = animaObjects[number].name;
        RectTransform move = animaObjects[number].GetComponent<RectTransform>();

        if(move.anchoredPosition.x >= endPos[animaObjects[number].name].x) // 中央より右にあるオブジェクト
        {
            while (move.anchoredPosition.x >= endPos[animaObjects[number].name].x + 0.1f)
            {
                move.anchoredPosition = Vector3.Lerp(move.anchoredPosition, endPos[moveObject], slideSpeed);
                yield return null;
            }
        }
        else if(move.anchoredPosition.x <= endPos[animaObjects[number].name].x) // 中央より左にあるオブジェクト
        {
            while (move.anchoredPosition.x <= endPos[animaObjects[number].name].x - 0.1f)
            {
                move.anchoredPosition = Vector3.Lerp(move.anchoredPosition, endPos[moveObject], slideSpeed);
                yield return null;
            }
        }

        move.anchoredPosition = endPos[moveObject];
        yield break;
    }

    /// <summary>
    /// 引っ込むときのアニメーション
    /// </summary>
    IEnumerator OutAnimation()
    {
        yield return null;

        IEnumerator anima01 = OutAnima_move(0, 0.03f);
        StartCoroutine(anima01);

        IEnumerator anima02 = OutAnima_move(1, 0.03f);
        StartCoroutine(anima02);

        IEnumerator anima03 = OutAnima_move(2, 0.03f);
        StartCoroutine(anima03);

        IEnumerator anima04 = OutAnima_move(3, 0.03f);
        StartCoroutine(anima04);

        yield return new WaitForSeconds(1.0f);

        StopCoroutine(anima01);
        StopCoroutine(anima02);
        StopCoroutine(anima03);
        StopCoroutine(anima04);

        for (int i = 0; i < animaObjects.Count; i++)
        {
            RectTransform setObject = animaObjects[i].GetComponent<RectTransform>();
            setObject.anchoredPosition = startPos[setObject.name];
        }

        yield return null;

       gameObject.SetActive(false);
    }
    /// <summary>
    /// 最初の位置まで移動させる専用のメソッド
    /// </summary>
    /// <param name="number">動させたい、辞書に保存したオブジェクトの登録番号</param>
    /// <param name="slideSpeed">スピード</param>
    IEnumerator OutAnima_move(int number, float slideSpeed)
    {
        string moveObject = animaObjects[number].name;
        RectTransform move = animaObjects[number].GetComponent<RectTransform>();

        if (move.anchoredPosition.x >= startPos[animaObjects[number].name].x) // 中央より左にあるオブジェクト
        {
            while (move.anchoredPosition.x <= endPos[animaObjects[number].name].x + 0.1f)
            {
                move.anchoredPosition = Vector3.Lerp(move.anchoredPosition, startPos[moveObject], slideSpeed);
                yield return null;
            }
        }
        else if(move.anchoredPosition.x <= startPos[animaObjects[number].name].x) // 中央より右にあるオブジェクト
        {
            while (move.anchoredPosition.x >= endPos[animaObjects[number].name].x - 0.1f)
            {
                move.anchoredPosition = Vector3.Lerp(move.anchoredPosition, startPos[moveObject], slideSpeed);
                yield return null;
            }
        }

        move.anchoredPosition = endPos[moveObject]; 
        yield break;
    }
}
