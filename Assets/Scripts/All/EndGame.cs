using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField, Header("メニュー内のオブジェクト")] List<GameObject> menuObject = new();
    bool isMenu = false;

    void Start()
    {
        // メニュー内のオブジェクトを非表示にする
        for(int i = 0; i < menuObject.Count; i++)
        {
            menuObject[i].SetActive(false);
        }
    }

    void Update()
    {
        // メニュー表示の時は、時間を止める
        if (isMenu)
        {
            // ザ・ワ―ルド
            Time.timeScale = 0.0f;
        }
        else
        {
            // 時は動き出す…
            Time.timeScale = 1.0f;
        }

        // ゲーム終了メソッド
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isMenu)
            {
                // メニュー表示
                EndMenu();

                // メニュー状態
                isMenu = true;
            }
            else
            {
                // キャンセルと同じ挙動
                OnEndGame_false();
            }
        }
    }

    /// <summary>
    /// メニュー表示
    /// </summary>
    void EndMenu()
    {
        // メニュー内のオブジェクトを表示する
        for (int i = 0; i < menuObject.Count; i++)
        {
            menuObject[i].SetActive(true);
        }
    }

    /// <summary>
    /// ボタン用メソッド、ゲーム終了
    /// </summary>
    public void OnEndGame_true()
    {
        if (Application.isEditor) // エディター
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else // それ以外(実機)
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// ボタン用メソッド、メニューを閉じる
    /// </summary>
    public void OnEndGame_false()
    {
        // メニュー内のオブジェクトを非表示にする
        for (int i = 0; i < menuObject.Count; i++)
        {
            menuObject[i].SetActive(false);
        }

        // メニュー状態解除
        isMenu = false;
    }
}
