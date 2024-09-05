using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
    }

    public Sound[] bgmSounds;
    public Sound[] sfxSounds;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();
    private List<int> playedBgmIndices = new List<int>();
    private int currentBgmIndex = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        foreach (Sound s in bgmSounds)
        {
            soundDictionary[s.name] = s;
        }
        foreach (Sound s in sfxSounds)
        {
            soundDictionary[s.name] = s;
        }
    }

    // 這裡我們將添加播放音樂和音效的方法

    public void PlayBGM(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            bgmSource.clip = s.clip;
            bgmSource.volume = s.volume;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM: " + name + " not found!");
        }
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }
    public void PlaySFX(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            sfxSource.PlayOneShot(s.clip, s.volume);
        }
        else
        {
            Debug.LogWarning("SFX: " + name + " not found!");
        }
    }
    public void PlayBgmRandomly()
    {
        if (bgmSounds.Length == 0) return;

        // 如果所有音樂都播放過了，重置列表
        if (playedBgmIndices.Count == bgmSounds.Length)
        {
            playedBgmIndices.Clear();
        }

        // 選擇一個還沒播放過的音樂
        int nextIndex;
        do
        {
            nextIndex = Random.Range(0, bgmSounds.Length);
        } while (playedBgmIndices.Contains(nextIndex));

        playedBgmIndices.Add(nextIndex);
        currentBgmIndex = nextIndex;

        Sound s = bgmSounds[currentBgmIndex];
        bgmSource.clip = s.clip;
        bgmSource.volume = s.volume;
        bgmSource.Play();

        // 設置在這首音樂結束時播放下一首
        StartCoroutine(WaitForMusicEnd());
    }

    private IEnumerator WaitForMusicEnd()
    {
        yield return new WaitUntil(() => !bgmSource.isPlaying);
        PlayBgmRandomly();
    }
    public List<string> GetAllBGMNames()
    {
        return bgmSounds.Select(s => s.name).ToList();
    }

    // 可選：獲取所有SFX名稱的方法
    public List<string> GetAllSFXNames()
    {
        return sfxSounds.Select(s => s.name).ToList();
    }

    public float BgmVolume()
    {
        return bgmSource.volume;
    }

    public void FadeBGM(float duration, float targetVolume)
    {
        StartCoroutine(FadeBGMCoroutine(duration, targetVolume));
    }

    private IEnumerator FadeBGMCoroutine(float duration, float targetVolume)
    {
        float startVolume = bgmSource.volume;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }
}