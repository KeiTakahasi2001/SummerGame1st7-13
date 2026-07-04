using UnityEngine;
using UnityEngine.SceneManagement; // 👈 シーンを切り替えるための魔法

public class TitleManager : MonoBehaviour
{
    // ボタンが押されたときに呼ばれる命令
    public void OnStartButton()
    {
        SceneManager.LoadScene("GameScene");// 「GameScene」という名前のシーンに移動して!
    }
}