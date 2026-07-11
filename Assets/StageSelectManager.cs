using UnityEngine;
using UnityEngine.SceneManagement; // ⭐️ シーン切り替えの呪文

public class StageSelectManager : MonoBehaviour
{
    // 「かんたん」ボタン用
    public void OnEasyButtonClick()
    {
        SceneManager.LoadScene("Easy_GameScene");
    }

    // 「ふつう」ボタン用
    public void OnNormalButtonClick()
    {
        SceneManager.LoadScene("Normal_GameScene");
    }

    // 「むずかしい」ボタン用
    public void OnHardButtonClick()
    {
        SceneManager.LoadScene("Hard_GameScene");
    }
}