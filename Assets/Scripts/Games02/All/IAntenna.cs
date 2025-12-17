using UnityEngine.EventSystems;

/// <summary>
/// アンテナみたいだなぁって思って命名、ラジオの周波みたいだよね
/// </summary>
public interface IAntenna : IEventSystemHandler
{
    /// <summary>
    /// リザルト表示変更用
    /// </summary>
    /// <param name="isClear">クリアしたかどうか</param>
    /// <param name="clearState">クリア進行度</param>
    void Result(bool isClear, int clearState);
}
