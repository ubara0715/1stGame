using PatternBase;
using UnityEngine;

public class Pattern01_0 : MonoBehaviour
{
    // コンポーネント取得
    StageManager manager;
    GameObject player = null;

    [Header("弾数カウント用")]
    [SerializeField] BulletArray storage_rabbit;
    [SerializeField] BulletArray storage_diamond01;
    [SerializeField] BulletArray storage_diamond02;

    // インスタンス生成
    readonly Loading loading01 = new Loading();
    readonly Loading loading02 = new Loading();
    readonly Loading loading03 = new Loading();
    readonly Setting setting = new Setting();
    readonly Move move = new Move();

    // クールタイム
    float interval01 = 0.0f;
    float interval02 = 0.0f;
    float interval03 = 0.0f;

    void Start()
    {
        // コンポーネント取得
        player = GameObject.FindWithTag("Player");
        manager = GameObject.FindWithTag("Manager").GetComponent<StageManager>();
    }

    void Update()
    {
        // ビデオ再生中は停止
        if(manager.isVideo != true)
        {
            if (manager.turn == StageManager.Turn.Game) // ゲームターンのみ
            {
                TestPattern01(0.8f);
                TestPattern02(0.4f);
                TestPattern03(0.3f);
            }
        }
    }

    /// <summary>
    /// ホーミング弾
    /// </summary>
    /// <param name="ct">クールタイム</param>
    void TestPattern01(float ct = 0.0f)
    {
        if (interval01 > 0)
        {
            interval01 -= 1.0f * Time.deltaTime;
        }

        if (interval01 <= 0)
        {
            GameObject bullet = loading01.BulletLoop(storage_rabbit);

            setting.LockOn(gameObject.transform, player.transform, bullet, 0.2f);
            move.Straight(gameObject.transform, bullet, 7.0f);

            interval01 = ct;
        }
    }

    /// <summary>
    /// 全方位弾
    /// </summary>
    /// <param name="ct">クールタイム</param>
    void TestPattern02(float ct = 0.0f)
    {
        if (interval02 > 0)
        {
            interval02 -= 1.0f * Time.deltaTime;
        }

        if (interval02 <= 0)
        {
            int clcle = 12;

            for (int i = 0; i < clcle; i++)
            {
                GameObject bullet = loading02.BulletLoop(storage_diamond01);

                setting.Around(gameObject.transform, bullet, clcle, 1.0f);
                move.Straight(gameObject.transform, bullet, 4.0f);
            }

            interval02 = ct;
        }
    }

    /// <summary>
    /// 全方位弾
    /// </summary>
    /// <param name="ct">クールタイム</param>
    void TestPattern03(float ct = 0.0f)
    {
        if (interval03 > 0)
        {
            interval03 -= 1.0f * Time.deltaTime;
        }

        if (interval03 <= 0)
        {
            int clcle = 12;

            for (int i = 0; i < clcle; i++)
            {
                GameObject bullet = loading03.BulletLoop(storage_diamond02);

                setting.Around(gameObject.transform, bullet, clcle, 1.0f, 15.0f);
                move.Straight(gameObject.transform, bullet, 3.0f);
            }

            interval03 = ct;
        }
    }
}
