using UnityEngine;
using UnityEngine.EventSystems;

public class StageBase : MonoBehaviour
{
    // ステージ情報取得
    protected StageData.PhaseData info;

    // HP取得用
    [HideInInspector] public float hp_player = 1.0f;
    [HideInInspector] public float hp_boss = 1.0f;

    // スタート時の時間
    /// <summary>
    /// 討伐分岐の場合は変更なし
    /// </summary>
    [HideInInspector] public float timeLimit = 99.0f;
    float time = 0.0f;

    // PlayerPrefsから情報取得用
    string stageNumber = null;
    public int clearState = 0;

    // 一度きりメソッドに使う
    bool isOnce = false;

    // コンポーネント取得
    PlayerBase player;
    BossBase boss;
    GameObject manager;
    StageManager stageManager;

    protected virtual void Start()
    {
        // 初期化
        isOnce = false;

        // コンポーネント取得
        boss = GameObject.FindWithTag("Boss").GetComponent<BossBase>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBase>();
        manager = GameObject.FindWithTag("Manager"); // メッセージ送信用
        stageManager = manager.GetComponent<StageManager>();

        // 使うデータ同期とステージナンバリング取得
        info = stageManager.setData;
        stageNumber = PrefsData.GetSetting();

        // プレイヤーの最大HP取得、制限時間もあげる
        hp_player = player.hp;
        stageManager.timeMax = timeLimit;

        // クリアのやつ、PlayerPrefsから情報とって書き換えてね 追記:書き換えてやったわ
        if (PrefsData.HasProgress(stageNumber)) 
        {
            clearState = PrefsData.GetProgress(stageNumber);
        }

        // 記録がないなら作ればいいじゃない
        if (info.branch == StageData.PhaseData.Branch.Defeat) // 討伐分岐
        {
            if (!PrefsData.HasTime(stageNumber))
            {
                PrefsData.SaveTime(stageNumber, 99.0f); // 最長経過時間
            }
        }
        else if (info.branch == StageData.PhaseData.Branch.Endure) // 耐久分岐
        {
            if (!PrefsData.HasLife(stageNumber))
            {
                PrefsData.SaveLife(stageNumber, (int)hp_player); // 被弾回数
            }
        }
        else if (info.branch == StageData.PhaseData.Branch.Origin) // Origin分岐
        {
            if (!PrefsData.HasTime(stageNumber))
            {
                PrefsData.SaveTime(stageNumber, 99.0f);  // 最長経過時間
            }

            if (!PrefsData.HasLife(stageNumber))
            {
                PrefsData.SaveLife(stageNumber, (int)hp_player); // 被弾回数
            }
        }
    }

    protected virtual void Update()
    {
        if(stageManager.isVideo != true)
        {
            // ゲームターンのみ処理
            if (stageManager.turn == StageManager.Turn.Game)
            {
                if (info.branch == StageData.PhaseData.Branch.Defeat) // 討伐分岐
                {
                    // 体力更新
                    hp_boss = boss.hp;
                    time += Time.deltaTime;
                    stageManager.timeReal = time;
                }
                else if (info.branch == StageData.PhaseData.Branch.Endure) //耐久分岐
                {
                    // タイム更新
                    time += Time.deltaTime;
                    stageManager.timeReal = time;
                }
                else if (info.branch == StageData.PhaseData.Branch.Origin) // どっちも
                {
                    // 体力更新
                    hp_boss = boss.hp;

                    // タイム更新
                    time += Time.deltaTime;
                    stageManager.timeReal = time;
                }

                // ライフ更新
                hp_player = player.hp;
                stageManager.lifeReal = hp_player;

                // フェーズ終了したら一度だけ処理
                if (!isOnce)
                {
                    if (IsFinish())
                    {
                        isOnce = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// ステージ終了条件
    /// </summary>
    /// <param name="branch">クリア条件</param>
    /// <returns>ステージが終わったかどうか</returns>
    protected bool IsFinish()
    {
        switch (info.branch)
        {
            case StageData.PhaseData.Branch.Origin: // Origin分岐、時間経過、ボス討伐、プレイヤー討伐

                if(hp_boss <= 0.0f) // ボス討伐
                {
                    if (clearState == 1) // 未クリア
                    {
                        // 討伐クリアに変更
                        clearState = 3;

                        SendResult(true); // リザルト表示
                        return true;
                    }
                    else if(clearState == 2) // 耐久クリア済み
                    {
                        // 両方クリアに変更
                        clearState = 4;

                        SendResult(true); // リザルト表示
                        return true;
                    }
                    else // 討伐クリア済み、両方クリア済み
                    {
                        SendResult(true); // リザルト表示
                        return true;
                    }
                }

                if(time >= timeLimit) // 時間経過
                {
                    if (clearState == 1) // 未クリア
                    {
                        // 耐久クリアに変更
                        clearState = 2;

                        SendResult(true); // リザルト表示
                        return true;
                    }
                    else if (clearState == 3) // 討伐クリア済み
                    {
                        // 両方クリアに変更
                        clearState = 4;

                        SendResult(true); // リザルト表示
                        return true;
                    }
                    else // 耐久クリア済み、両方クリア済み
                    {
                        SendResult(true); // リザルト表示
                        return true;
                    }
                }

                break;
            case StageData.PhaseData.Branch.Endure: // 耐久分岐、時間経過、プレイヤー討伐

                if (time >= timeLimit) // 時間経過
                {
                    SendResult(true); // リザルト表示

                    // 耐久クリアに変更
                    clearState = 2;
                    return true;
                }

                break;
            case StageData.PhaseData.Branch.Defeat: // 討伐分岐、ボス討伐、プレイヤー討伐

                if (hp_boss <= 0.0f) // ボス討伐
                {
                    // 討伐クリアに変更
                    clearState = 3;

                    SendResult(true); // リザルト表示
                    return true;
                }

                if(time >= timeLimit) // 時間経過
                {
                    SendResult(false); // リザルト表示
                    return true;
                }

                break;
        }

        if (hp_player <= 0.0f) // プレイヤー討伐
        {
            SendResult(false); // リザルト表示

            return true;
        }

        return false;
    }
    /// <summary>
    /// リザルトに記録を送る、クリアしたかどうかの判定が必要
    /// </summary>
    /// <param name="isClear">クリアしたかどうか</param>
    void SendResult(bool isClear)
    {
        // ここで小数点第四位以下は切り捨てる
        stageManager.timeRecord = float.Parse(time.ToString("F3"));

        ExecuteEvents.Execute<IAntenna>(
                target: manager,
                eventData: null,
                functor: (receiver, evebtData) => receiver.Result(isClear, clearState)
            );
    }
}
