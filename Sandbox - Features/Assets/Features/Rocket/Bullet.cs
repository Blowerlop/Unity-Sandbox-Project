using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bullet")]
public class Bullet : ScriptableObject
{
    public string name;
    public float speed;
    public float[] optionalSettings;
    public GameObject prefab;
}
