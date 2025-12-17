using System.Collections;
using UnityEngine;

public class PlayerBase : StatusBase
{
    // コンポーネント取得
    StageManager manager;

    // 移動速度
    float speed = 1.0f; // 変数
    readonly float highSpeed = 24.0f; // 通常速度
    readonly float slowSpeed = 6.0f; // ゆっくり速度

    // 移動制限用
    readonly float heightRestriction = 4.8f; //上下
    readonly float widthRestriction_L = -8.69f; // 左側
    readonly float widthRestriction_R = 2.8f; // 右側

    [SerializeField,Header("プレイヤーのビジュアル")] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    Color originalColor, damageColor;

    void Start()
    {
        // コンポーネント取得
        manager = GameObject.FindWithTag("Manager").GetComponent<StageManager>();
        originalColor = spriteRenderer.color;
        damageColor = new Color(1.0f, 0.3f, 0.3f);
    }

    void Update()
    {
        if(manager.isVideo != true)
        {
            if (manager.turn != StageManager.Turn.Result) // リザルトターン以外では動ける
            {
                animator.enabled = true;
                InputTranslate();
            }
            else
            {
                animator.enabled = false;
            }
        }
        else
        {
            animator.enabled = false;
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GetDamege();
        // 被弾回数増加
        manager.hitRecord += 1;
        StartCoroutine(DamageEffect());
    }

    /// <summary>
    /// 移動用関数、Transform移動
    /// </summary>
    void InputTranslate()
    {
        // 速度変更
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = slowSpeed;
        }
        else
        {
            speed = highSpeed;
        }

        // 縦移動
        if (Input.GetAxis("Vertical") != 0)
        {
            float PosY = transform.position.y + Input.GetAxis("Vertical") * speed * Time.deltaTime;

            // 移動制限
            if (PosY < -heightRestriction || PosY > heightRestriction)
            {
                PosY = Mathf.Clamp(PosY, -heightRestriction, heightRestriction);
            }

            transform.position = new Vector3(transform.position.x, PosY, transform.position.z);
        }

        // 横移動
        if (Input.GetAxis("Horizontal") != 0)
        {
            // アニメーション制御
            if (Input.GetAxis("Horizontal") < 0) // 左入力
            {
                animator.SetBool("IsLeft", true);
            }
            else // 右入力
            {
                animator.SetBool("IsRight", true);
            }

            float PosX = transform.position.x + Input.GetAxis("Horizontal") * speed * Time.deltaTime;

            // 移動制限
            if (PosX < widthRestriction_L || PosX > widthRestriction_R)
            {
                PosX = Mathf.Clamp(PosX, widthRestriction_L, widthRestriction_R);
            }

            transform.position = new Vector3(PosX, transform.position.y, transform.position.z);
        }
        else
        {
            // Idleに戻す
            animator.SetBool("IsRight", false);
            animator.SetBool("IsLeft", false);
        }
    }

    IEnumerator DamageEffect()
    {
        spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(0.25f);

        spriteRenderer.color = originalColor;

        yield break;
    }
}
