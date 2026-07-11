using UnityEngine;

public class SceneSound : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic; // ここにクリア音やゲームオーバー音を入れる
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (sceneMusic != null)
        {
            audioSource.PlayOneShot(sceneMusic);
        }
    }
}