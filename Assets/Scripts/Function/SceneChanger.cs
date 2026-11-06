using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    //ボタンクリックでプレイシーンに
    public void StartGame()
    {
        SceneManager.LoadScene("Play");
    }

    //タイトルに戻る
    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
