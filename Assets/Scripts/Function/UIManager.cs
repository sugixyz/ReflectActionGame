using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HP設定")]
    //HP用スライダー
    [SerializeField] private Slider hpSlider;
    //スライダーの中身
    [SerializeField] private Image hpFill;

    [Header("エネルギー設定")]
    //エネルギー用画像
    [SerializeField] private Image energyCircle;
    //エネルギー用テキスト
    [SerializeField] private TextMeshProUGUI energyText;

    [Header("ヒット数設定")]
    //ヒット数用テキスト
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private CanvasGroup combo;
    //サイズの戻す速度
    [SerializeField] private float lerpSpeed = 3.0f;
    //コンボのリセット時間
    [SerializeField] private float comboLimitTime = 2.0f;
    //コンボのリセット時間を管理する変数
    float comboTimer;
    //フェードの開始時間を保存する変数
    float fadeTime;
    //現在のヒット数を保存する変数
    int currentHitCount;

    [Header("弾数設定")]
    //弾数用アイコン
    [SerializeField] private GameObject ammoContainer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        comboTimer = 0;
        combo.gameObject.SetActive(false);
        fadeTime = comboLimitTime / 2.0f;

        currentHitCount = 0;
    }

    private void Update()
    {
        //文字のサイズをゆっくり戻す
        countText.transform.localScale = Vector3.Lerp(
            countText.transform.localScale,
            Vector3.one,
            Time.deltaTime * lerpSpeed
        );

        //タイマーのカウントダウン
        if(comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            //リミットの半分まで行ったらフェードアウトさせる
            if(comboTimer <= fadeTime)
            {
                float alpha = comboTimer / fadeTime;
                combo.alpha = alpha;
            }
            else
            {
                combo.alpha = 1.0f;
            }

            //ゼロになったら消す
            if (comboTimer <= 0)
            {
                combo.gameObject.SetActive(false);
                combo.alpha = 1.0f;
                currentHitCount = 0;
            }
        }
    }

    //HPを反映する関数
    public void UpdateHP(int currentHP, int maxHP)
    {
        //スライダーがあるなら
        if (hpSlider != null)
        {
            //割合に応じて色を変える
            float raito = (float)currentHP / maxHP;
            if(raito >= 0.5)
            {
                hpFill.color = Color.green;
            }
            else if(raito >= 0.2)
            {
                hpFill.color = Color.yellow;
            }
            else
            {
                hpFill.color = Color.red;
            }

            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }
    }

    //エネルギーを反映させる関数
    public void UpdateEnergy(float current,float max)
    {
        if(energyCircle != null)
        {
            energyCircle.fillAmount = current / max;
        }

        if (energyText != null)
        {
            energyText.text = $"{(int)current}";
        }
    }

    //ヒット数を反映させる関数
    public void UpdateHitCount(int hit)
    {
        //現在の弾数がhitより小さいなら
        if (hit <= currentHitCount)
        {
            return;
        }
        currentHitCount = hit;

        if(combo != null)
        {
            //ヒット数に応じて色を変える
            if(hit >= 20)
            {
                countText.color = Color.red;
            }
            else if(hit >= 10)
            {
                countText.color = Color.orange;
            }
            else
            {
                countText.color = Color.white;
            }

                countText.text = $"{hit}";

            //文字を一瞬大きく
            countText.transform.localScale = Vector3.one * 1.2f;
            //テキストを表示する
            combo.gameObject.SetActive(true);
            comboTimer = comboLimitTime;
        }
    }

    //弾数を反映させる関数
    public void UpdateAmmo(int currentAmmo)
    {
        //弾数分ループ
        for(int i = 0;i< ammoContainer.transform.childCount;i++)
        {
            //現在の弾数以下を非表示に
            bool shouldShow = true;
            if(i >= currentAmmo)
            {
                shouldShow = false;
            }
            //弾の表示を変更
            ammoContainer.transform.GetChild(i).gameObject.SetActive(shouldShow);
        }
    }
}
