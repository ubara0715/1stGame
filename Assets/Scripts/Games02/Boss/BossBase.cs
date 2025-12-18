using UnityEngine;
using System.Collections;

public class BossBase : StatusBase
{
    // コンポーネント取得
    StageManager manager;
    Animator animator01, animator02;

    // ダメージ演出
    SpriteRenderer sR01, sR02;
    Color originalColor, damageColor;

    [Header("HPバー表示用")]
    [SerializeField] GameObject gauge;
    RectTransform gaugeTransform;
    float oneSize, onePosition;

    void Start()
    {
        // コンポーネント取得
        gauge = GameObject.Find("Gauge");
        gaugeTransform = gauge.GetComponent<RectTransform>();
        animator01 = transform.GetChild(0).GetComponent<Animator>(); // 顔アニメーション
        animator02 = transform.GetChild(1).GetComponent<Animator>(); // 本体アニメーション
        sR01 = transform.GetChild(0).GetComponent<SpriteRenderer>(); // 顔のスプライト
        sR02 = transform.GetChild(1).GetComponent<SpriteRenderer>(); // 本体のスプライト
        manager = GameObject.FindWithTag("Manager").GetComponent<StageManager>();

        // 一回あたるごとに減る「ゲージの量」を計算
        oneSize = gaugeTransform.sizeDelta.x / hp;
        onePosition = gaugeTransform.position.x / hp;

        // ダメージ受けた時のカラーと元のカラー取得
        originalColor = sR01.color;
        damageColor = new Color(0.6f, 1.0f, 0.3f);

        if (manager.setData.branch == StageData.PhaseData.Branch.Endure)
        {
            gauge.SetActive(false);
        }
    }

    void Update()
    {
        // ビデオ再生中は停止
        if(manager.isVideo != true)
        {
            // リザルトのときに全てが止まったように見せるため、アニメーションを止める
            if (manager.turn == StageManager.Turn.Result) // リザルトターンの時
            {
                animator01.enabled = false;
                animator02.enabled = false;
            }
            else
            {
                animator01.enabled = true;
                animator02.enabled = true;
            }
        }
        else
        {
            animator01.enabled = true;
            animator02.enabled = true;
        }
    }

    // ダメージ計算、表示とゲージ減らし
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (manager.setData.branch != StageData.PhaseData.Branch.Endure)
        {
            GetDamege();
            StartCoroutine(DamageEffect());

            UpdateUI();
        }
    }

    /// <summary>
    /// ボスのHPゲージ管理用、サイズと位置をずらしている
    /// </summary>
    void UpdateUI()
    {
        gaugeTransform.sizeDelta = new Vector2((gaugeTransform.sizeDelta.x - oneSize), gaugeTransform.sizeDelta.y);
        gaugeTransform.position = new Vector2((gaugeTransform.position.x - onePosition), gaugeTransform.position.y);
    }

    /// <summary>
    /// ダメージエフェクト(カラー変更)のコルーチン、1フレームだけだと短すぎる
    /// </summary>
    IEnumerator DamageEffect()
    {
        sR01.color = damageColor;
        sR02.color = damageColor;

        yield return new WaitForSeconds(0.25f);

        sR01.color = originalColor;
        sR02.color = originalColor;

        yield break;
    }
}
