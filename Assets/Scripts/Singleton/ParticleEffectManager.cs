using UnityEngine;
using System.Collections.Generic;

public class ParticleEffectManager : MonoBehaviour
{
    public static ParticleEffectManager Instance { get; private set; }

    [System.Serializable]
    public class ParticleEffectEntry
    {
        public string name;
        public GameObject prefab;
    }

    public List<ParticleEffectEntry> effects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayEffect(string effectName, Vector3 position)
    {
        // find
        ParticleEffectEntry effect = effects.Find(x => x.name == effectName);

        if (effect != null)
        {
            // instantiate
            GameObject obj = Instantiate(effect.prefab, position, Quaternion.identity);
            Destroy(obj, 3f);
        }
    }
}