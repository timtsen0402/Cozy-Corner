using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 0f;
    }

    public Sound[] bgmSounds;
    public Sound[] sfxSounds;

    [field: SerializeField]
    public float fadeSecondsBGM { get; private set; }

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
    public IEnumerator PlayBgmRandomly()
    {
        if (bgmSounds.Length == 0) yield break;

        if (playedBgmIndices.Count == bgmSounds.Length)
        {
            playedBgmIndices.Clear();
        }

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
        FadeBGM(fadeSecondsBGM, 1f);

        yield return new WaitUntil(() => !bgmSource.isPlaying);
        FadeBGM(fadeSecondsBGM, 0f);
        StartCoroutine(PlayBgmRandomly());
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