using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.PlayerLoop;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;


    [SerializeField] private int poolSize = 10;


    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = 
        new Dictionary<GameObject, Queue<GameObject>>();


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Destroy(gameObject);
    }



    public GameObject GetObject(GameObject prefab)
    {
        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab);
        }

        if(poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);

        GameObject objectToGet = poolDictionary[prefab].Dequeue();
        objectToGet.SetActive(true);
        objectToGet.transform.parent = null;

        return objectToGet;
    }

    public void ReturnObject(GameObject objectToRuturn , float delay = 0.001f)
    {

        StartCoroutine(DelayReturn(delay ,objectToRuturn));
    }

    private IEnumerator DelayReturn(float delay , GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);

        ReturnToPool(objectToReturn);
    }

    private void ReturnToPool(GameObject ObjectToReturn)
    {
        GameObject originalPrefab = ObjectToReturn.GetComponent<PooledObject>().originalPrebaf;
        
        ObjectToReturn.SetActive(false);
        ObjectToReturn.transform.parent = transform;

        poolDictionary[originalPrefab].Enqueue(ObjectToReturn);
    }

    private void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab) 
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrebaf = prefab;

        newObject.SetActive(false);
        poolDictionary[prefab].Enqueue( newObject );
    }
}
