using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleManager : Animations
{
    [Header("文字用")] 
    [SerializeField] Text titleText;
    [SerializeField] float textAnimation_speed = 1.0f;

    // 画面タップフラグ、ビデオ再生終了フラグ(ワンショット)、一回だけ実行
    bool isToSelect = false, isOnce = false;

    // ロゴフェードアウト、テキストフェードアウト、遷移アニメーション
    IEnumerator fadeOut_logo = null, fadeOut_titleText = null, toSelect = null, load = null , fadeOut_loadText = null;

    [Header("BGのコンポーネント取得")]
    [SerializeField] VideoPlayer videoObject;
    [SerializeField] RawImage videoImage;

    [Header("Animationsフォルダから")]
    [SerializeField] Sprite toSelect_end;
    [SerializeField] VideoClip toSelectAnimation, loopAnimation;

    [Header("ゲームオブジェクトたち")]
    [SerializeField] Image titleLogo;
    [SerializeField] GameObject touchPanel;
    [SerializeField] GameObject resetButton;
    [SerializeField] Text gauge;

    // フェードイン開始
    [SerializeField] GameObject blackImage;
    IEnumerator fadeStart = null, start = null;

    void Start()
    {
        videoObject.loopPointReached += FinishPlayingVideo; // 動画の最後に起きるイベントを追加
        isToSelect = false;
        gauge.enabled = false;

        videoObject.clip = loopAnimation;

        start = FedeStart();
        StartCoroutine(start);
    }

    void Update()
    {
        if (isToSelect != true)
        {
            titleText.color = Blink(titleText.color, textAnimation_speed);
        }
    }

    /// <summary>
    /// ボタン用メソッド、押すとループ再生が切られ再生終了時にイベントが発生する
    /// </summary>
    public void ToSelect()
    {
        isToSelect = true; // 手動ループを切る
        touchPanel.SetActive(false); // 重複防止
        resetButton.SetActive(false);

        videoObject.playbackSpeed = 1.7f; // 早送り
    }

    /// <summary>
    /// 遷移用アニメーション再生して選択シーンに遷移する
    /// </summary>
    IEnumerator PlayToSelect()
    {
        videoObject.clip = toSelectAnimation; // 変更
        videoObject.playbackSpeed = 1.6f; // 速度調整
        videoObject.Play(); // 再生

        yield return new WaitForSeconds((float)(videoObject.clip.length - 1.02f));

        videoImage.texture = toSelect_end.texture;

        load = Loading();
        StartCoroutine(load);

        yield break;
    }

    /// <summary>
    /// 再生終了後に追加するイベント、フラグが立つとコルーチン起動する
    /// </summary>
    public void FinishPlayingVideo(VideoPlayer vp)
    {
        if (isToSelect == false)
        {
            videoObject.Play(); // フラグが立つ前は手動ループ
        }
        else
        {
            if (!isOnce)
            {
                // ロゴフェードアウト
                fadeOut_logo = FadeOut_image(titleLogo, 0.5f);
                StartCoroutine(fadeOut_logo);

                // テキストフェードアウト
                fadeOut_titleText = FadeOut_text(titleText, 0.8f);
                StartCoroutine(fadeOut_titleText);

                // 遷移アニメーション
                toSelect = PlayToSelect();
                StartCoroutine(toSelect);

                isOnce = true;
            }
        }
    }

    /// <summary>
    /// 非同期読み込み用、上手く動いている気配がしない
    /// </summary>
    IEnumerator Loading()
    {
        gauge.enabled = true;

        yield return null;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SelectScene"); // 読み込み開始
        asyncLoad.allowSceneActivation = false; // 読み込み終了時に自動的に切り替わる設定を切る

        while (asyncLoad.progress < 0.9f)
        {
            gauge.text = ($"{float.Parse(asyncLoad.progress.ToString("F3")) * 100}%");
            yield return null;
        }

        gauge.text = "100%";

        yield return new WaitForSeconds(0.5f);

        fadeOut_loadText = FadeOut_text(gauge, 0.2f);
        StartCoroutine(fadeOut_loadText);

        while (gauge.color.a >= 0.01f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true; // 設定をもとに戻す

        yield break;
    }

    IEnumerator FedeStart()
    {
        blackImage.SetActive(true);
        fadeStart = FadeOut_image(blackImage.GetComponent<Image>(), 1.5f);

        StartCoroutine(fadeStart);

        yield return fadeStart;

        blackImage.SetActive(false);

        yield break;
    }
}
