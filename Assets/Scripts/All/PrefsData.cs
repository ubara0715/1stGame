using UnityEngine;

public static class PrefsData
{
    /// <summary>
    /// 最短討伐タイムの記録保存用、勝手に小数第四位以下は切り捨てるようにしてある
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <param name="record">最短討伐タイム</param>
    public static void SaveTime(string stageNumber, float record)
    {
        float recordFloor = float.Parse(record.ToString("F3"));
        string key = "Time" + stageNumber;

        PlayerPrefs.SetFloat(key, recordFloor);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// 最短討伐タイムを取得する、小数点第四位以下は切り捨ててるので安心してほしい
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <returns>最短討伐タイム</returns>
    public static float GetTimeRecord(string stageNumber)
    {
        string key = "Time" + stageNumber;
        float record = PlayerPrefs.GetFloat(key, 99.0f);

        return record;
    }
    /// <summary>
    /// 最短討伐タイムがあるかないか
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <returns>あるかないか</returns>
    public static bool HasTime(string stageNumber)
    {
        string key = "Time" + stageNumber;
        bool exist = PlayerPrefs.HasKey(key);

        return exist;
    }

    /// <summary>
    /// 最小被弾数の記録保存用
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <param name="record">被弾数</param>
    public static void SaveLife(string stageNumber, int record)
    {
        string key = "Life" + stageNumber;

        PlayerPrefs.SetInt(key, record);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// フェーズの最小被弾数を取得する
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <returns>最小被弾数</returns>
    public static int GetLifeRecord(string stageNumber)
    {
        string key = "Life" + stageNumber;
        int record = PlayerPrefs.GetInt(key, 3);

        return record;
    }
    /// <summary>
    /// 最小被弾回数の記録があるかないか
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <returns>あるかないか</returns>
    public static bool HasLife(string stageNumber)
    {
        string key = "Life" + stageNumber;
        bool exist = PlayerPrefs.HasKey(key);

        return exist;
    }

    /// <summary>
    /// クリア進行度の記録保存用
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <param name="progress">クリア進行度</param>
    public static void SaveProgress(string stageNumber, int progress)
    {
        PlayerPrefs.SetInt(stageNumber, progress);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// フェーズのクリア進行度をゲットする
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <returns>フェーズのクリア進行度</returns>
    public static int GetProgress(string stageNumber)
    {
        int getInt = PlayerPrefs.GetInt(stageNumber);

        return getInt;
    }
    /// <summary>
    /// クリア進行度の記録があるかないか
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <returns>あるかないか</returns>
    public static bool HasProgress(string stageNumber)
    {
        bool exist = PlayerPrefs.HasKey(stageNumber);

        return exist;
    }

    /// <summary>
    /// ステージセッティング用
    /// </summary>
    /// <param name="stageNumber">選択されたステージ</param>
    public static void SaveSetting(string stageNumber)
    {
        PlayerPrefs.SetString("Set", stageNumber);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// セットしたステージナンバリングを取得する
    /// </summary>
    /// <returns>ステージナンバリング</returns>
    public static string GetSetting()
    {
        string getString = PlayerPrefs.GetString("Set", "No,Setting");

        return getString;
    }
    /// <summary>
    /// セットしたステージナンバリングをいれる箱があるかどうか、正味使わん
    /// </summary>
    /// <param name="stageNumber">ステージナンバリング</param>
    /// <returns>あるかないか</returns>
    public static bool HasSetting()
    {
        bool exist = PlayerPrefs.HasKey("Set");

        return exist;
    }
}
