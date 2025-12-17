using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HPを管理するだけの継承用クラス
/// </summary>
public class StatusBase : MonoBehaviour
{
    /// <summary>
    /// BaseスクリプトのHP
    /// </summary>
    public float hp = 1.0f;

    /// <summary>
    /// ダメージ処理、基本的に1ダメージ
    /// </summary>
    /// <param name="damage">ダメージ</param>
    /// <returns>ダメージ受けた後のHP</returns>
    protected float GetDamege(float damage = 1.0f)
    {
        hp -= damage;

        return hp;
    }
}
