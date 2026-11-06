using UnityEngine;
using System;

public class EnemyController : MonoBehaviour,IDamageable
{
    [Header("基本設定")]
    //リキッドボディを取得する変数
    Rigidbody rb;
    //開始地点
    Vector3 pointA;
    //終了地点
    Vector3 pointB;
    //移動距離
    [SerializeField] Vector3 positionOffset;
    //移動にかかる時間
    [SerializeField] float moveSpeed = 3.0f;
    //どれだけ速度を低下させるか
    [SerializeField] float speedDownRate = 0.9f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pointA = rb.position;
        pointB = pointA + positionOffset;
    }
    
    void Update()
    {
        //ラープ処理用の0～1を往復するレイト
        float raito = Mathf.PingPong(Time.time * moveSpeed, 1.0f);

        //AとBを往復させる
        transform.position = Vector3.Lerp(pointA, pointB, raito);
    }

    //弾に触れた時の処理
    void OnTriggerEnter(Collider other)
    {
        //当たったのが弾か判別
        if(other.CompareTag("Bullet"))
        {
            //弾の速度を下げる
            Bullet bullet = other.GetComponent<Bullet>();
            bullet.HitEnemy(speedDownRate);

            //ダメージを受ける
            TakeDamage(1);
        }
    }

    //IDamageableの実装
    public void TakeDamage(int damage)
    {
        //敵は一撃で死亡
        if(rb != null)
        {
            gameObject.SetActive(false);
        }
    }
}
