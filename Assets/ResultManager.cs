using TMPro; // ⭐️ TextMeshProを使うための呪文
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    // Unityの画面で、さっき作ったテキスト部品をここに紐付けます！
    public TextMeshProUGUI resultText;

    void Start()
    {
        // ⭐️【超重要】GameManagerがゲームシステムに保存してくれた static な数字を呼び出す！
        int finalCount = GameManager.flipCount;
        float finalTime = GameManager.timer;

        // 画面に映るテキストを、本物の記録に書き換える！
        resultText.text = "手数: " + finalCount + "回\n" +
                          "タイム: " + finalTime.ToString("F1") + "秒";
    }
    public void OnReplayButtonClick()
    {
        SceneManager.LoadScene("StartScene");
    }
}