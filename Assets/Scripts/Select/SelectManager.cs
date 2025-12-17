using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SelectManager : Animations
{
    [Header("ステージデータ取得")]
    public List<StageData> stages = new List<StageData>();

    [Header("カットインの要素取得")]
    [SerializeField] GameObject checkCut;
    [SerializeField] Image bossImage;
    [SerializeField] Text phaseTitle;

    [Header("アニメーション再生用")]
    [SerializeField] GameObject videoPanel;
    [SerializeField] GameObject blackImage;
    [SerializeField] VideoClip startAnimation, toGameAnimation;

    void Start()
    {
        // 遷移アニメーションを再生
        blackImage.SetActive(false);
        videoPanel.SetActive(true);
        videoPanel.GetComponent<VideoPlayer>().clip = startAnimation;
        videoPanel.GetComponent<VideoPlayer>().Play();

        // 進行状況に応じてステージを解放する
        SettingButton();

        // UIをオフにしておく
        checkCut.SetActive(false);

        // 再生が終わったらオフにする
        Invoke("StartOff", 0.8f);
    }

    /// <summary>
    /// 全てのステージの進行度をチェックして、ボタンの切り替えを行う
    /// </summary>
    void SettingButton()
    {
        for(int n = 0;n < stages.Count; n++)
        {
            for (int m = 0; m < stages[n].phaseData.Count; m++)
            {
                string stageNumber = stages[n].phaseData[m].stageNumber;
                Button changeState = GameObject.Find("Stage" + stageNumber).GetComponent<Button>();

                if (PrefsData.GetProgress(stageNumber) == 0)
                {
                    changeState.interactable = false;
                }
                else
                {
                    changeState.interactable = true;
                }
            }
        }
    }

    /// <summary>
    /// ボタン用メソッド、各ボタンにステージナンバリングを振っておく
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    public void Select(string stageNumber)
    {
        // セットしているステージの更新
        PrefsData.SaveSetting(stageNumber);

        // カットインを表示
        checkCut.SetActive(true);

        // 文字列から、①ステージ番号と②フェーズ番号を取得(①〇〇_②〇)してステージデータリストから検索して代入
        int group = int.Parse(stageNumber.Substring(0,2));
        int nunber = int.Parse(stageNumber.Substring(3));
        StageData.PhaseData settingData = stages[group].phaseData[nunber];

        // フェーズのクリア状況を取得
        int progress = PrefsData.GetProgress(stageNumber);

        // ↓カットインの要素を変更↓

        // フェーズタイトルを変更
        phaseTitle.text = settingData.phaseTitle;

        // 進行状況に応じて、ボスのイラストを変更
        if (progress <= 1) // 未クリアの場合、シークレットイラスト
        {
            if(settingData.secret != null)
            {
                bossImage.sprite = settingData.secret;
                bossImage.enabled = true;
            }
            else
            {
                bossImage.enabled = false;
            }
        }
        else if (progress == 1 && (PlayerPrefs.HasKey("Time" + stageNumber) || PlayerPrefs.HasKey("Life" + stageNumber))) // 記録がない=未挑戦の場合、ノーマルイラスト
        {
            if (settingData.normal != null)
            {
                bossImage.sprite = settingData.normal;
                bossImage.enabled = true;
            }
            else
            {
                bossImage.enabled = false;
            }
        }
        else if(progress >= 2) // クリア済みの場合、やられたよイラスト
        {
            if (settingData.clear != null)
            {
                bossImage.sprite = settingData.clear;
                bossImage.enabled = true;
            }
            else
            {
                bossImage.enabled = false;
            }
        }
    }

    // ステージスタート用
    public void OnStageStart()
    {
        Set_Change();
        StartCoroutine(Change);
    }
    IEnumerator ChangeScene()
    {
        // コンポーネント取得
        VideoPlayer vPlayer = videoPanel.GetComponent<VideoPlayer>();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        asyncLoad.allowSceneActivation = false;

        yield return null;

        // ビデオ切り替えて表示、再生
        vPlayer.clip = toGameAnimation;
        videoPanel.SetActive(true);

        vPlayer.Play();

        yield return new WaitForSeconds((float)toGameAnimation.length);

        blackImage.SetActive(true);
        asyncLoad.allowSceneActivation = true;

        yield break;
    }

    IEnumerator Change = null;
    void Set_Change()
    {
        Change = ChangeScene();
    }

    void StartOff()
    {
        videoPanel.SetActive(false);
    }
}
