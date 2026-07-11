using UnityEngine;
using TMPro; // ⭐️ TextMeshProを使うための呪文
using UnityEngine.SceneManagement;

public class BackStartScene : MonoBehaviour
{
    public void OnReplayButtonClick()
    {
        SceneManager.LoadScene("StartScene");
    }
}
