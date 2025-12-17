using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    // コンポーネント取得
    StageManager manager;
    Rigidbody2D rb;
    Collider2D col;

    /// <summary>
    /// ゲーム画面にあるかないか
    /// </summary>
    public enum State
    {
        /// <summary>
        /// ゲーム画面内
        /// </summary>
        InGame,
        /// <summary>
        /// ゲーム画面外
        /// </summary>
        OutGame
    }
    [HideInInspector] public State state;

    // 位置保存
    Vector3 startPos;

    // パーティクル
    [SerializeField] GameObject damageEffect = null;

    void Start()
    {
        // OutGame状態のときの定位置
        startPos = transform.position;
        state = State.OutGame; // 初期化

        // コンポーネント取得
        manager = GameObject.FindWithTag("Manager").GetComponent<StageManager>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        // リザルトターンになると止まる
        if(manager.turn == StageManager.Turn.Result)
        {
            rb.velocity = Vector2.zero;
        }

        // ゲーム画面外に飛んでったときの処理、横バージョン
        if (transform.position.x >= 4.0f || transform.position.x <= -10.0f)
        {
            state = State.OutGame;
        }
        // ゲーム画面外に飛んでったときの処理、縦バージョン
        if (transform.position.y >= 6.0f || transform.position.y <= -6.0f)
        {
            state = State.OutGame;
        }

        switch (state)
        {
            case State.OutGame: // 画面外

                col.enabled = false; // 当たり判定を消す
                transform.position = startPos; // 定位置に戻る

                break;
            case State.InGame: // 画面内

                col.enabled = true; // 当たり判定復活

                break;
        }
    }

    // 衝突したら定位置に戻す
    void OnCollisionEnter2D(Collision2D collision)
    {
        state = State.OutGame;
        if(damageEffect != null && collision.gameObject.CompareTag("Boss"))
        {
            Instantiate(damageEffect,transform.position,Quaternion.identity);
        }
    }
}
