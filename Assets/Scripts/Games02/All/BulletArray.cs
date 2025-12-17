using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletArray : MonoBehaviour
{
    [Header("配列にはなんもいれないでね")]
    public GameObject[] bullets;

    void Start()
    {
        // 子の数だけ配列にGameObject型でぶち込む
        bullets = new GameObject[transform.childCount]; // リサイズ
        for (int i = 0; i < transform.childCount; i++)
        {
            bullets[i] = transform.GetChild(i).gameObject;
        }
    }
}
