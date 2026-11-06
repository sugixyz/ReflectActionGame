using UnityEngine;

//弾を反射する機能
public interface IReflectable
{
    //弾の反射時の加速率を返す関数
    //float GetSpeedIncreaseRate();
}

//弾が当たった時に反射、ダメージ以外の特定の処理をする機能
public interface IHitReceiver
{
    //当たった時の処理をする関数
    void OnHit();
}

//弾が当たった時にダメージを受ける機能
public interface IDamageable
{
    //ダメージを食らう処理
    void TakeDamage(int damage);
}

//特定の条件で移動をする機能
public interface IEasingMovable
{
    //移動する関数
    void Move();
}

//透明化の機能
public interface IVisionable
{
    //透視の切り替え関数
    void OnVisionToggle(bool isVisionMode);
}