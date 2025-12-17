using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 最初に各個人のセーブデータを作成する、ステージの進行状況と選択ステージ格納用を作成
/// </summary>
public class CreateUserData : MonoBehaviour
{
    [SerializeField,Header("ステージデータ")] List<StageData> stageData = new List<StageData>();

    void Start()
    {
        CreateData();
    }

    /// <summary>
    /// データ作成、ステージデータの数とそれぞれのフェーズ数を数えてその分の進行状況を記録する
    /// </summary>
    void CreateData()
    {
        // 進行状況記録を作成
        for(int n = 0; n < stageData.Count; n++) // ステージ数
        {
            for(int m = 0; m < stageData[n].phaseData.Count; m++) // 各ステージのフェーズ数
            {
                string stageNumber = stageData[n].phaseData[m].stageNumber;

                if (!PrefsData.HasProgress(stageNumber))
                {
                    PrefsData.SaveProgress(stageNumber, 0);

                    if(stageNumber == "00_0" || stageNumber == "01_0")
                    {
                        PrefsData.SaveProgress(stageNumber, 1);
                    }

                    Debug.Log(stageNumber + "| クリア進行度 |" + PrefsData.GetProgress(stageNumber));
                }
            }
        }

        // 選択ステージ記録を作成
        PrefsData.SaveSetting("");
    }
}
