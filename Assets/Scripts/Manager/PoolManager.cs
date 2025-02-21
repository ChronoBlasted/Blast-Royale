using BaseTemplate.Behaviours;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class PoolManager : MonoSingleton<PoolManager>
{
    public Transform poolContainer;

    public List<ResourceType> resourceInPool = new();

    [field: SerializeField] public int DefaultPoolSize { get; protected set; } = 10;
    [field: SerializeField] public int MaxPoolSize { get; protected set; } = 100;

    protected Dictionary<ResourceType, IObjectPool<GameObject>> pools = new();

    ResourceObjectHolder holder;

    public IObjectPool<GameObject> this[ResourceType resourceType]
    {
        get { return pools[resourceType]; }
    }

    [SerializeField] protected bool collectionChecks = true; // Collection checks will throw errors if we try to release an item that is already in the pool.

    public void Init()
    {
        holder = ResourceObjectHolder.Instance;

        SceneManager.sceneUnloaded += DestroyPool;

        foreach (var resourceType in resourceInPool)
        {
            var container = new GameObject(resourceType.ToString()).transform;
            container.SetParent(poolContainer.transform);

            pools[resourceType] = new ObjectPool<GameObject>(
                () =>
                {
                    return Instantiate(holder.GetResourceByType(resourceType).Prefab, container);
                },
                OnTakeFromPool,
                (gameObject) =>
                {
                    if (gameObject.transform as RectTransform) gameObject.transform.SetParent(UIManager.Instance.RectTransformPool);
                    else gameObject.transform.SetParent(container);
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localScale = Vector3.one;
                    gameObject.transform.localRotation = Quaternion.identity;
                    gameObject.SetActive(false);
                },
                OnDestroyPoolObject,
                collectionChecks,
                DefaultPoolSize,
                MaxPoolSize
            );
        }
    }

    protected void DestroyPool(Scene _)
    {
        foreach (var (_, pool) in pools)
        {
            pool.Clear();
        }
    }

    protected void OnTakeFromPool(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    protected void OnDestroyPoolObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}