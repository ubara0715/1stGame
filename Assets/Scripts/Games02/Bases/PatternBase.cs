using UnityEngine;

namespace PatternBase
{
    /// <summary>
    /// 弾装填用のクラス
    /// </summary>
    public class Loading
    {
        int n = 0;

        /// <summary>
        /// 基本的なループ
        /// </summary>
        /// <param name="storage">弾薬庫、種類別にしてな</param>
        /// <returns>移動、発射する弾</returns>
        public GameObject BulletLoop(BulletArray storage)
        {
            n += 1;
            if (n >= storage.bullets.Length - 1)
            {
                n = 0;
            }
            
            return storage.bullets[n];
        }
    }

    /// <summary>
    /// 弾幕を配置するメソッド
    /// </summary>
    public class Setting
    {
        float angle;

        /// <summary>
        /// 一方向に一個飛ばすメソッド
        /// </summary>
        /// <param name="main">発生位置</param>
        /// <param name="bullet">配置する弾幕</param>
        /// <param name="distance">発生位置からの距離</param>
        public void Way_1(Transform main, GameObject bullet, float distance = 0.0f)
        {
            bullet.transform.position = new Vector3(main.position.x, main.position.y + (1.0f * distance), main.position.z);

            bullet.GetComponent<BulletBase>().state = BulletBase.State.InGame;
        }

        /// <summary>
        /// 相手のいる方向に回転させて配置するメソッド、正直Moveのほうでホーミングの動き作ればいいんじゃね？
        /// </summary>
        /// <param name="main">発生位置</param>
        /// <param name="target">相手</param>
        /// <param name="bullet">配置する弾幕</param>
        /// <param name="distance">発生位置からの距離</param>
        public void LockOn(Transform main,Transform target, GameObject bullet ,float distance = 0.0f)
        {
            bullet.transform.position = Vector3.Lerp(main.position, target.position, Mathf.Clamp01(distance));
            bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, (target.position - bullet.transform.position));

            bullet.GetComponent<BulletBase>().state = BulletBase.State.InGame;
        }

        /// <summary>
        /// 全方位に弾幕を円形配置するメソッド、配置する個数だけ繰り返さないといけない
        /// </summary>
        /// <param name="main">発生位置(中心)</param>
        /// <param name="bullet">配置する弾幕</param>
        /// <param name="amount">いくつ配置するか</param>
        /// <param name="distance">発生位置からの距離</param>
        /// <param name="offset">ずらして発射する用、角度</param>
        public void Around(Transform main, GameObject bullet, int amount, float distance = 1.0f, float offset = 0.0f)
        {
            float rad = (angle + offset) * Mathf.Deg2Rad;

            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);

            Vector3 pos = main.transform.position + new Vector3(cos * distance, sin * distance, 0.0f);
            bullet.transform.position = pos;
            bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, (bullet.transform.position - main.transform.position));

            angle += 360 / amount;
            bullet.GetComponent<BulletBase>().state = BulletBase.State.InGame;

            if (angle >= 360)
            {
                angle = 0;
            }
        }
    }

    /// <summary>
    /// 動きを付けるメソッド、単調な動きのみ
    /// </summary>
    public class Move
    {
        /// <summary>
        /// 向いている方向にまっすぐ飛ばすメソッド
        /// </summary>
        /// <param name="main">発生位置</param>
        /// <param name="bullet">動かす弾幕</param>
        /// <param name="speed">スピード</param>
        public void Straight(Transform main, GameObject bullet, float speed)
        {
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            Vector2 vec01 = new Vector2(bullet.transform.position.x - main.position.x, bullet.transform.position.y - main.position.y).normalized;

            rb.velocity = Vector2.zero;
            rb.AddForce(vec01 * speed, ForceMode2D.Impulse);
        }
    }
}
