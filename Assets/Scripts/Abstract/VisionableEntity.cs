using UnityEngine;

public abstract class VisionableEntity : MonoBehaviour,IVisionable
{
    [Header("透視の共通設定")]
    [SerializeField] protected float visionAlpha = 0.3f;

    //マテリアルを保存する変数
    protected Renderer myRenderer;
    //初期の色を保存する変数
    Color defaultColor;

    [Header("マテリアル設定")]
    //通常マテリアル
    [SerializeField] private Material normal;
    //透視モード時のマテリアル
    [SerializeField] private Material transparent;

    protected void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        if(myRenderer != null)
        {
            myRenderer.material = normal;
            defaultColor = myRenderer.material.color;
        }
    }

    //イベントの登録
    protected void OnEnable()
    {
        //イベントを購読する
        PlayerController.OnVisionAction += OnVisionToggle;
    }

    //イベントの解除
    protected void OnDisable()
    {
        //イベントの登録を解除
        PlayerController.OnVisionAction -= OnVisionToggle;
    }

    //IVisionableの実装
    public void OnVisionToggle(bool isVisionMode)
    {
        if (myRenderer == null) return;

        //初期の色を取得
        Color targetColor = defaultColor;
        myRenderer.material = normal;
        //透視モードなら透明度を適応
        if(isVisionMode)
        {
            myRenderer.material = transparent;
            targetColor.a = visionAlpha;
        }

        //色を適応
        myRenderer.material.color = targetColor;
    }
}
