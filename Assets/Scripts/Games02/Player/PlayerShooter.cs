using PatternBase;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    // コンポーネント取得
    StageManager manager;

    // インスタンス生成
    Loading loading = new Loading();
    Setting setting = new Setting();
    Move move = new Move();

    // クールタイム
    float shotInterval = 0.0f; // 変数
    readonly float maxShotInterval = 0.08f;

    // 弾数カウント
    [SerializeField] BulletArray storage;

    // オート実装待ってます
    bool isAuto;

    void Start()
    {
        // 初期化？
        isAuto = false;

        // コンポーネント取得
        manager = GameObject.FindWithTag("Manager").GetComponent<StageManager>();
    }

    void Update()
    {
        if(manager.isVideo != true)
        {
            // クールタイム処理
            if (shotInterval > 0)
            {
                shotInterval -= 1.0f * Time.deltaTime;
            }

            // ショット
            if (manager.turn == StageManager.Turn.Game) // ゲームターンのみ
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    Shot();
                    if (shotInterval <= 0)
                    {
                        shotInterval = maxShotInterval;
                    }
                }
                else if (isAuto)
                {
                    Shot();
                    if (shotInterval <= 0)
                    {
                        shotInterval = maxShotInterval;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 簡単なまっすぐ飛ばすメソッド
    /// </summary>
    void Shot()
    {
        if (shotInterval <= 0)
        {
            GameObject bullet = loading.BulletLoop(storage);

            setting.Way_1(gameObject.transform, bullet, 1.2f);
            move.Straight(gameObject.transform, bullet, 15.0f);
        }
    }
}
