using System.Collections.Generic;
using UnityEngine;

public class ResetData : MonoBehaviour
{
    [SerializeField] List<StageData> stageData = new List<StageData>();

    /// <summary>
    /// ボタン用メソッド、現在の選択されているステージを取得し、データを消して再生成する
    /// </summary>
    public void OnReset()
    {
        string setStage = PlayerPrefs.GetString("Set");

        PlayerPrefs.DeleteAll();

        for (int n = 0; n < stageData.Count; n++)
        {
            for (int m = 0; m < stageData[n].phaseData.Count; m++)
            {
                string stageNumber = stageData[n].phaseData[m].stageNumber;

                PrefsData.SaveProgress(stageNumber, 0);

                // 初期段階で開放しておくステージ
                if (stageNumber == "00_0" || stageNumber == "01_0")
                {
                    PrefsData.SaveProgress(stageNumber, 1);
                }
            }
        }
        PrefsData.SaveSetting(setStage);
    }
}
