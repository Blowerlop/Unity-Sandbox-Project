using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string name;
    public float fireRate;
    public float damage;
    public float ammo;
    public GameObject prefab;
    public Bullet bullet;
}
