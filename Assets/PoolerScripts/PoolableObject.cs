using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Poolable ")]
public class PoolableObject : ScriptableObject
{
    [SerializeField] private GameObject m_Prefab; 
    [SerializeField] private float m_CopyAmount;

    public GameObject Prefab => m_Prefab;
    public float CopyAmount => m_CopyAmount;
}
