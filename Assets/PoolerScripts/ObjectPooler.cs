using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Andrich
{
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler m_Instance { get; private set; }

        private Dictionary<PoolableObject, Queue<GameObject>> m_PrefabDictionary;
        private Dictionary<PoolableObject, Queue<PrefabPool>> m_NestedClassDictionary;

        [SerializeField] private List<PrefabPool> m_PrefabPools = new List<PrefabPool>();

        [System.Serializable]
        public class PrefabPool
        {
            [SerializeField] private string m_ElementName = "Name";
            [SerializeField] private PoolableObject m_WhichPrefab = null;
            [SerializeField] private GameObject m_Prefab = null;
            [SerializeField] private Transform m_Parent = null;
            [SerializeField] private int m_CopyAmount = 50;

            public string ElementName => m_ElementName;
            public PoolableObject WhichPrefab => m_WhichPrefab;
            public GameObject Prefab => m_Prefab;
            public Transform Parent => m_Parent;
            public int CopyAmount => m_CopyAmount;
        }

        private void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            else if (m_Instance != null)
            {
                Destroy(this);
            }


            InstantiatePrefabs();
        }

        //public void DeactivateAllObjects()
        //{
        //    Array prefabsEnum = Enum.GetValues(typeof());

        //    for (int i = 1; i < prefabsEnum.Length; i++)
        //    {
        //        foreach (GameObject gameObject in m_PrefabDictionary[(WhichPrefab)i])
        //        {
        //            gameObject.transform.SetParent(m_NestedClassDictionary[(WhichPrefab)i].Peek().m_Parent);
        //            gameObject.SetActive(false);
        //        }
        //    }
        //}

        private void InstantiatePrefabs()
        {
            m_PrefabDictionary = new Dictionary<PoolableObject, Queue<GameObject>>();
            m_NestedClassDictionary = new Dictionary<PoolableObject, Queue<PrefabPool>>();

            foreach (PrefabPool pool in m_PrefabPools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                Queue<PrefabPool> nestedClass = new Queue<PrefabPool>();

                for (int i = 0; i < pool.CopyAmount; i++)
                {
                    GameObject copy = Instantiate(pool.Prefab, pool.Parent);
                    copy.SetActive(false);

                    objectPool.Enqueue(copy);
                    nestedClass.Enqueue(pool);
                }

                m_PrefabDictionary.Add(pool.WhichPrefab, objectPool);
                m_NestedClassDictionary.Add(pool.WhichPrefab, nestedClass);
            }
        }

        public void DeactivateObject(GameObject gameObject, PoolableObject poolableObject)
        {
            gameObject.transform.SetParent(m_NestedClassDictionary[poolableObject].Peek().Parent);
            gameObject.SetActive(false);
        }

        private void IncreasePoolSize(PoolableObject prefabEnum)
        {
            PrefabPool pool = m_NestedClassDictionary[prefabEnum].Peek();

            for (int i = 0; i < pool.CopyAmount; i++)
            {
                GameObject copy = Instantiate(pool.Prefab, pool.Parent);
                copy.SetActive(false);

                m_PrefabDictionary[prefabEnum].Enqueue(copy);
                m_NestedClassDictionary[prefabEnum].Enqueue(pool);
            }
        }

        public GameObject SetActiveFromPool(PoolableObject poolableObject, Vector3 position, Quaternion rotation)
        {
            GameObject objectToSetActive;

            if (!m_PrefabDictionary.ContainsKey(poolableObject))
            {
                Debug.LogError("The enum: " + poolableObject + " hasn't been assigned yet");
                return null;
            }

            if (m_PrefabDictionary[poolableObject].Peek().activeSelf)
            {
                Debug.Log("Increasing Pool Size");
                IncreasePoolSize(poolableObject);
            }

            objectToSetActive = m_PrefabDictionary[poolableObject].Dequeue();

            objectToSetActive.SetActive(true);
            objectToSetActive.transform.position = position;
            objectToSetActive.transform.rotation = rotation;

            m_PrefabDictionary[poolableObject].Enqueue(objectToSetActive);

            return objectToSetActive;
        }
    }
}


