using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;
    private Image fadeImage;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //シーンが切り替わっても消えないように
            DontDestroyOnLoad(gameObject.transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject.transform.root.gameObject);
        }

        fadeImage = GetComponent<Image>();
        //最初は透明に
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    //画面遷移とフェードをセットで実行する関数
    public void FadeAndLoadScnee(int sceneIndex)
    {
        StartCoroutine(FadeOutAndIn(sceneIndex));
    }

    private IEnumerator FadeOutAndIn(int sceneIndex)
    {
        //徐々に画面を黒く
        float time = 0;
        while(time < 1.0f)
        {
            time += Time.deltaTime;
            //透明度を変える
            fadeImage.color = new Color(0, 0, 0, Mathf.Min(time, 1.0f));
            yield return null;
        }

        //シーンを読み込む
        SceneManager.LoadScene(sceneIndex);

        //徐々に透明に
        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            //透明度を変える
            fadeImage.color = new Color(0, 0, 0, Mathf.Max(time, 0.0f));
            yield return null;
        }
    }
}
