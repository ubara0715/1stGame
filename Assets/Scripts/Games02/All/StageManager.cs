using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class StageManager : MonoBehaviour,IAntenna
{
    public enum Turn
    {
        /// <summary>
        /// トークターン、攻撃停止と会話発生
        /// </summary>
        Talk,
        /// <summary>
        /// ゲームターン、トークUIとリザルトUIをオフにして攻撃開始
        /// </summary>
        Game,
        /// <summary>
        /// リザルトターン、ゲームターンで稼働していたものをすべて止めてリザルト表示
        /// </summary>
        Result,
    }

    [Header("ターン表示")]
    public Turn turn;
    [Header("ステージデータ取得")]
    public List<StageData> stages = new List<StageData>();
    [HideInInspector] public StageData.PhaseData setData;
    string stageNumber = null;

    [Header("トークターン")]
    [SerializeField] GameObject talkCanvas;
    [SerializeField] Text text;
    [SerializeField] GameObject player;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject talkBox;
    //bool isNextLog　= false;

    [Header("ゲームターン")]
    [SerializeField] Text timeText; // UI
    [SerializeField] Text timeVisualization; // UI
    [SerializeField] Text lifeVisualization; // UI
    [HideInInspector] public float timeMax; // 制限時間記録用、StageBaseで更新
    [HideInInspector] public float timeReal; // リアルタイムの記録、UI用、StageBaseで更新
    [HideInInspector] public float lifeReal; // リアルタイムの記録、UI用、StageBaseで更新
    [HideInInspector] public float timeRecord; // 終了した時の経過時間、StageBaseで更新
    [HideInInspector] public int hitRecord; // 終了した時の被弾回数、PlayerDataで更新

    [Header("リザルトターン")]
    [SerializeField] GameObject resultCanvas;
    [SerializeField] VideoPlayer resultVideo;
    /// <summary>
    /// 0→結果、1,2→スコア、3,4→ハイスコアのとき
    /// </summary>
    [SerializeField] List<Text> texts = new List<Text>();
    [SerializeField] Button toNextButton;
    [SerializeField] Text toNextButtonText;
    string nextPhase = null;

    [Header("アニメーション制御")]
    [SerializeField] GameObject videoCanvas;
    [SerializeField] GameObject blackImage;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] VideoClip toScene, toRetry, fromScene;
    IEnumerator retry = null, next = null, from = null;
    public bool isVideo;

    void Start()
    {
        // リザルトリセット＆非表示
        for(int i = 1; i < texts.Count; i++)
        {
            texts[i].enabled = false;
        }
        toNextButton.interactable = false;
        resultCanvas.SetActive(false);

        // データセット、リストから検索して取得
        int group = int.Parse(PrefsData.GetSetting().Substring(0, 2));
        int nunber = int.Parse(PrefsData.GetSetting().Substring(3));
        setData = stages[group].phaseData[nunber];

        // PlayerPrefsからセットしたステージナンバリングを取得
        stageNumber = PrefsData.GetSetting();

        // クリア済みの場合はトークターン飛ばす
        if(PrefsData.GetProgress(stageNumber) > 1)
        {
            turn = Turn.Game;
        }
        else
        {
            turn = Turn.Talk;
        }

        // 初期UI
        switch (setData.branch)
        {
            case StageData.PhaseData.Branch.Origin: // 残り時間表示、残りライフ表示

                timeText.text = "残り接続時間";
                timeVisualization.text = timeMax.ToString("F3");
                lifeVisualization.text = ((int)lifeReal).ToString();

                break;
            case StageData.PhaseData.Branch.Endure: // 残り時間表示、残りライフ表示

                timeText.text = "残り接続時間";
                timeVisualization.text = timeMax.ToString("F3");
                lifeVisualization.text = ((int)lifeReal).ToString();

                break;
            case StageData.PhaseData.Branch.Defeat: // 経過時間表示、残りライフ表示

                timeText.text = "経過時間";
                timeVisualization.text = timeReal.ToString("F3");
                lifeVisualization.text = ((int)lifeReal).ToString();

                break;
        }

        // ステージ生成
        SettingStage();

        // 初期化
        isVideo = false;
        // アニメーション
        from = From();
        StartCoroutine(from);
    }

    void Update()
    {
        if(isVideo != true)
        {
            // トークターン(未完成)
            if (Input.GetKeyDown(KeyCode.Return) && turn == Turn.Talk)
            {
                Talk();
            }

            // ターンによってUIの表示非表示を切り替える
            switch (turn)
            {
                case Turn.Talk: // トークUIのみ
                    talkCanvas.SetActive(true);
                    resultCanvas.SetActive(false);
                    break;
                case Turn.Game: // どっちもなし
                    talkCanvas.SetActive(false);
                    resultCanvas.SetActive(false);
                    break;
                case Turn.Result: // リザルトUIのみ
                    talkCanvas.SetActive(false);
                    resultCanvas.SetActive(true);
                    break;
            }

            // クリア条件によってUI表示を変える
            switch (setData.branch)
            {
                case StageData.PhaseData.Branch.Origin: // 残り時間表示、残りライフ表示

                    timeText.text = "残り接続時間";
                    timeVisualization.text = (timeMax - timeReal).ToString("F3");
                    lifeVisualization.text = ((int)lifeReal).ToString();

                    break;
                case StageData.PhaseData.Branch.Endure: // 残り時間表示、残りライフ表示

                    timeText.text = "残り接続時間";
                    timeVisualization.text = (timeMax - timeReal).ToString("F3");
                    lifeVisualization.text = ((int)lifeReal).ToString();

                    break;
                case StageData.PhaseData.Branch.Defeat: // 経過時間表示、残りライフ表示

                    timeText.text = "経過時間";
                    timeVisualization.text = timeReal.ToString("F3");
                    lifeVisualization.text = ((int)lifeReal).ToString();

                    break;
            }
        }
    }

    /// <summary>
    /// ステージ切り替え
    /// </summary>
    void SettingStage()
    {
        if(GameObject.Find(setData.stagePrefab.name) == null)
        {
            Instantiate(setData.stagePrefab);
        }
    }

    /// <summary>
    /// セリフ流し、ストーリーデータを作ってそこからシナリオ取得しよう
    /// </summary>
    void Talk()
    {
        turn = Turn.Game;
    }

    /// <summary>
    /// シーン遷移用アニメーション
    /// </summary>
    /// <param name="next">次のシーンの名前</param>
    /// <param name="video">遷移用アニメーション</param>
    IEnumerator ToNext(string next,VideoClip video)
    {
        isVideo = true;

        // 非同期読み込み開始
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(next);
        asyncLoad.allowSceneActivation = false;

        videoCanvas.SetActive(true);
        videoPlayer.clip = video;

        yield return null;

        videoPlayer.Play();

        yield return new WaitForSeconds((float)videoPlayer.length);

        asyncLoad.allowSceneActivation = true;
        blackImage.SetActive(true);
        isVideo = false;

        yield break;
    }
    /// <summary>
    /// はじまりのアニメーション、再生した後ビデオフラグをオフにして再生用キャンバスもオフにする
    /// </summary>
    IEnumerator From()
    {
        isVideo = true;

        videoCanvas.SetActive(true);
        videoPlayer.clip = fromScene;

        yield return null;

        videoPlayer.Play();

        yield return new WaitForSeconds((float)videoPlayer.length);

        videoCanvas.SetActive(false);

        isVideo = false;

        yield break;
    }

    /// <summary>
    /// <para>リザルト表示変更用</para>
    /// <para>やられた場合→でか文字を再挑戦てきなニュアンスに変えて、次へボタンを非表示か押せなくする</para>
    /// <para>耐久クリア→耐久分岐へのシーンをセット、あたった回数を表示</para>
    /// <para>討伐クリア→討伐分岐へのシーンをセット、クリアタイムを表示</para>
    /// </summary>
    /// <param name="isClear">クリアしたか、やられたか</param>
    /// <param name="clearState">0 未解放、1 未クリア、2 耐久クリア、3 討伐クリア、4 両方クリア</param>
    public void Result(bool isClear, int clearState)
    {
        // ターン切り替え
        turn = Turn.Result;

        // リザルト表示
        resultCanvas.SetActive(true);
        resultVideo.Play();

        if (!isClear) //やられた場合
        {
            // テキスト変更
            texts[0].text = "データロスト";
            // ボタンをオフにして色変える
            toNextButton.interactable = false;
            toNextButtonText.color = toNextButton.colors.disabledColor;

            if(clearState != 1) // 前にクリアしてるなら、その時の最高記録できる
            {
                // 二個とも表示
                texts[1].enabled = true;
                texts[2].enabled = true;

                // 最高スコアに書き換え
                texts[1].text = "最短経過時間：" + PrefsData.GetTimeRecord(stageNumber); 
                texts[2].text = "最小被弾回数：" + PrefsData.GetLifeRecord(stageNumber);
            }
            else // 初回挑戦
            {
                texts[1].enabled = false;
                texts[2].enabled = false;
            }
        }
        else // クリアの場合
        {
            // テキスト変更
            texts[0].text = "データ奪取完了";
            // ボタンをオンにして色変える
            toNextButton.interactable = true;
            toNextButtonText.color = toNextButton.colors.normalColor;

            if (clearState == 2) // 耐久クリアのとき、被弾数表示
            {
                // 一個目表示
                texts[1].enabled = true;
                // 被弾数表示
                texts[1].text = "被弾回数：" + hitRecord;

                // 次のフェーズをセットしておく
                if(setData.nextPrefabs != null)
                {
                    nextPhase = setData.nextPrefabs[0].name;
                }

                // 最高記録なら「New Record!!」を表示する
                if (PrefsData.GetLifeRecord(stageNumber) > hitRecord)
                {
                    texts[3].enabled = true;
                }
            }
            else if(clearState == 3) // 討伐クリアのとき、最短タイム表示
            {
                // 一個目表示
                texts[1].enabled = true;
                // 経過時間表示
                texts[1].text = "経過時間：" + timeRecord;

                // 次のフェーズをセットしておく
                if(setData.nextPrefabs != null)
                {
                    if (setData.branch != StageData.PhaseData.Branch.Origin) // Origin分岐のときは一個飛ばすから
                    {
                        nextPhase = setData.nextPrefabs[0].name;
                    }
                    else
                    {
                        nextPhase = setData.nextPrefabs[1].name;
                    }
                }

                // 最高記録なら「New Record!!」を表示する
                if (PrefsData.GetTimeRecord(stageNumber) > timeRecord)
                {
                    texts[3].enabled = true;
                }
            }
            else if (clearState == 4) // どっちのスコアも表示
            {
                // 二個とも使う
                texts[1].enabled = true;
                texts[2].enabled = true;
                // 今回の記録表示
                texts[1].text = "経過時間：" + timeRecord;
                texts[2].text = "被弾回数：" + hitRecord;

                // 耐久クリアなら、被弾回数 | 討伐クリアなら、時間
                if (timeMax - timeRecord <= 0.0f) // 耐久クリア
                {
                    // 最高記録なら「New Record!!」を表示する
                    if (PrefsData.GetLifeRecord(stageNumber) > hitRecord)
                    {
                        texts[4].enabled = true;
                    }

                    // 耐久分岐へ
                    nextPhase = setData.nextPrefabs[0].name;
                }
                else
                {
                    // 最高記録なら「New Record!!」を表示する
                    if (PrefsData.GetTimeRecord(stageNumber) > timeRecord)
                    {
                        texts[3].enabled = true;
                    }

                    // 討伐分岐へ
                    nextPhase = setData.nextPrefabs[1].name;
                }
            }

            // 次のデータが未開放なら開放する
            if(PrefsData.GetProgress(nextPhase) == 0)
            {
                PrefsData.SaveProgress(nextPhase, 1);
            }

            // 記録更新更新
            RecordUpData();
        }

        // クリア進行度更新
        if(PrefsData.GetProgress(stageNumber) < clearState)
        {
            PrefsData.SaveProgress(stageNumber, clearState);
        }
    }
    /// <summary>
    /// プレイヤーデータを変える
    /// </summary>
    void RecordUpData()
    {
        // プレイヤーデータ更新
        switch (setData.branch)
        {
            case StageData.PhaseData.Branch.Origin:

                // タイム更新
                if (PrefsData.GetTimeRecord(stageNumber) > timeRecord)
                {
                    PrefsData.SaveTime(stageNumber, timeRecord);
                }
                else
                {
                    // 被弾回数更新
                    if (PrefsData.GetLifeRecord(stageNumber) > hitRecord)
                    {
                        PrefsData.SaveLife(stageNumber, hitRecord);
                    }
                }

                break;
            case StageData.PhaseData.Branch.Endure:

                // 被弾回数更新
                if (PrefsData.GetLifeRecord(stageNumber) > hitRecord)
                {
                    PrefsData.SaveLife(stageNumber, hitRecord);
                }

                break;
            case StageData.PhaseData.Branch.Defeat:

                // タイム更新
                if (PrefsData.GetTimeRecord(stageNumber) > timeRecord)
                {
                    PrefsData.SaveTime(stageNumber, timeRecord);
                }

                break;
        }

        // デバッグ用
        //Debug.Log($"進行度：{PrefsData.GetProgress(stageNumber)}\n討伐時間：{timeRecord}\n被弾数：{hitRecord}\n");
        //Debug.Log($"最高記録-最短討伐：{PrefsData.GetTimeRecord(stageNumber)}\n最高記録-被弾数：{PrefsData.GetLifeRecord(stageNumber)}\n");
    }

    /// <summary>
    /// ボタン用メソッド、リトライ
    /// </summary>
    public void OnRetry()
    {
        retry = ToNext(SceneManager.GetActiveScene().name, toRetry);
        StartCoroutine(retry);
    }

    /// <summary>
    /// ボタン用メソッド、ステージを消して次に持っていく
    /// </summary>
    public void OnNext()
    {
        Destroy(GameObject.Find(setData.stagePrefab.name));

        if(nextPhase != null)
        {
            // セットしたステージを書き換えてシーン読み込み
            PrefsData.SaveSetting(nextPhase);
            next = ToNext(SceneManager.GetActiveScene().name, toScene);
            StartCoroutine(next);
        }
    }

    /// <summary>
    /// ボタン用メソッド、セレクトシーンに戻る
    /// </summary>
    public void OnSelect()
    {
        SceneManager.LoadScene("SelectScene");
    }
}
