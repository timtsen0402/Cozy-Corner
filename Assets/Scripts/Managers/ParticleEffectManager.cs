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
        public int poolSize = 10;
    }

    public List<ParticleEffectEntry> effects;
    private Dictionary<string, Queue<GameObject>> pools;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        pools = new Dictionary<string, Queue<GameObject>>();
        foreach (var effect in effects)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < effect.poolSize; i++)
            {
                var obj = Instantiate(effect.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            pools[effect.name] = queue;
        }
    }

    public void PlayEffect(GameObject prefab, Vector3 position)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        Destroy(obj, 3f);
    }

    private System.Collections.IEnumerator ReturnToPool(GameObject obj, string effectName, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pools[effectName].Enqueue(obj);
    }
}