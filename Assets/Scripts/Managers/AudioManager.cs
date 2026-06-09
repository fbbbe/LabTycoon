using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("배경음")]
    public AudioClip mainBGM;
    
    [Header("효과음")]
    public AudioClip taskCompleteSound;
    public AudioClip cleaningSound;


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

   public void PlayTaskCompleteSound()
    {
        Debug.Log("효과음 재생");

        AudioSource.PlayClipAtPoint(
            taskCompleteSound,
            Camera.main.transform.position
        );
    }

    public void PlayCleaningSound()
    {
        if (cleaningSound == null)
        {
            return;
        }

        bgmSource.PlayOneShot(cleaningSound);
    }
}