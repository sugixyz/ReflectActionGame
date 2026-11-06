using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour,IVisionable
{
    //リキッドボディを取得する変数
    Rigidbody rb;
    //壁に触れた回数
    int hitCount = 0;
    //弾の速度（GunControllerから取得）
    float bulletSpeed;
    //減速率（GunControllerから取得）
    float drag;
    //下限速度（GunControllerから取得）
    float minSpeed;

    void Start()
    {
        //リキッドボディの取得
        rb = GetComponent<Rigidbody>();

    }

    private void OnEnable()
    {
        //壁に触れた回数をリセット
        hitCount = 0;

        //トレイルレンダラーのリセット
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if(trail != null)
        {
            trail.Clear();
            trail.enabled = true;
        }

        //イベントの購読
        PlayerController.OnVisionAction += OnVisionToggle;
    }

    void OnDisable()
    {
        //イベントの登録解除
        PlayerController.OnVisionAction -= OnVisionToggle;
    }

    //フレーム固定のUpdate
    void FixedUpdate()
    {
        //減速率をかける
        rb.linearVelocity *= drag;

        //速度が下限まで行ったら弾を返却
        if(rb.linearVelocity.magnitude < minSpeed)
        {
            ReturnToPool();
        }
        
    }

    //壁に触れた時の処理
    void OnCollisionEnter(Collision collision)
    {
        //衝突相手が反射機能を持っているか
        IReflectable reflectable = collision.gameObject.GetComponent<IReflectable>();
        if(reflectable != null)
        {
            //UIの更新
            hitCount++;
            UpdateHitCountUI();
            
            //物理演算を一時停止
            rb.isKinematic = true;

            //法線ベクトルと入射ベクトルを取得
            Vector3 normal = collision.contacts[0].normal;
            Vector3 inVector = -collision.relativeVelocity;
            //反射ベクトルを計算
            Vector3 reflectVelocity = Vector3.Reflect(inVector, normal);
            //正規化で方向ベクトルを取得
            reflectVelocity.Normalize();
            //速度をデフォルトに戻す
            reflectVelocity *= bulletSpeed;

            //壁から離す
            transform.position += normal * 0.1f;

            //物理演算を再開
            rb.isKinematic = false;
            //反射ベクトルを速度に適応
            rb.linearVelocity = reflectVelocity;
        }

        //衝突相手がヒット処理を持っているか
        IHitReceiver hitReceiver = collision.gameObject.GetComponent<IHitReceiver>();
        if(hitReceiver != null)
        {
            hitReceiver.OnHit();
        }
    }

    void ReturnToPool()
    {
        //物理挙動のリセット
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //オブジェクトを非表示に
        gameObject.SetActive(false);
    }

    //IVisionableの実装
    public void OnVisionToggle(bool isVisionMode)
    {
        //通常視点の時だけ軌跡をオンにする
        if(GetComponent<TrailRenderer>() != null)
        {
            GetComponent<TrailRenderer>().enabled = !isVisionMode;
        }
    }

    //速度の設定をする関数（Gunが呼ぶ）
    public void SetUp(float bulletV,float dragValue,float minV)
    {
        bulletSpeed = bulletV;
        drag = dragValue;
        minSpeed = minV;
    }

    //敵に当たったら速度を低下
    public void HitEnemy(float rate)
    {
        //現在の速度を保存
        Vector3 vel = rb.linearVelocity;
        //速度低下
        vel = vel * rate;
        rb.linearVelocity = vel;
    }

    //ヒット数UIの更新
    void UpdateHitCountUI()
    {
        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHitCount(hitCount);
        }
    }
}
