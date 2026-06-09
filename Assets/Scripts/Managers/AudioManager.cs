using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("배경음")]
    public AudioClip mainBGM;

    private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        bgmSource = GetComponent<AudioSource>();

        PlayMainBGM();
    }

    public void PlayMainBGM()
    {
        if (mainBGM == null)
        {
            Debug.LogWarning("배경음이 연결되지 않았습니다.");
            return;
        }

        bgmSource.clip = mainBGM;
        bgmSource.loop = true;
        bgmSource.Play();
    }
}