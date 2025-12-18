using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData",menuName = "CustomMenu/StageData",order = 0)]
public class StageData : ScriptableObject
{
    public List<PhaseData> phaseData = new List<PhaseData>();

    [System.Serializable]
    public class PhaseData
    {
        public enum Branch //クリア条件
        {
            /// <summary>
            /// 分岐前ステージ
            /// </summary>
            Origin,
            /// <summary>
            /// 耐久ステージ
            /// </summary>
            Endure,
            /// <summary>
            /// 討伐ステージ
            /// </summary>
            Defeat
        }

        [Header("ステージ情報")]
        public string phaseTitle; // フェーズのタイトル
        public string stageNumber; // ステージナンバリング
        public Branch branch; // 分岐

        [Header("ボスの立ち絵")]
        public Sprite secret; // 未挑戦
        public Sprite normal; // 未クリア
        public Sprite clear; // クリア済み

        [Header("シーン情報、0に耐久1に討伐をいれること")]
        public GameObject stagePrefab; // ステージ
        public List<GameObject> nextPrefabs = new List<GameObject>(); // 次のステージ
    }
}