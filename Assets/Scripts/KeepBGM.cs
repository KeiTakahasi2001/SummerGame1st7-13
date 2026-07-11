using UnityEngine;

public class KeepBGM : MonoBehaviour
{
    void Awake()
    {
        // ⭐️【魔法の呪文】シーンを移動しても、この箱を消さないで！
        DontDestroyOnLoad(gameObject);
    }
}