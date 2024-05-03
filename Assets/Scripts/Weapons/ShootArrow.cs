using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootArrow : MonoBehaviour
{
    // @louis why use a list here ?
    public List<GameObject> ArrowRef;
    public float ArrowSpeed;
    public float startDistance;
    
    // Start is called before the first frame update
    public void Shoot(int arrowIndex, Vector3 initialPos, Vector3 direction)
    {
        var rot = Quaternion.Euler(0f, 0f, 
        Mathf.Atan(direction.y / direction.x) * 180 / Mathf.PI + (direction.x >= 0 ? 90 : -90));
        var arr = Instantiate(ArrowRef[arrowIndex], initialPos - direction * startDistance, rot);
        var projectile = arr.GetComponent<Projectile>();
        projectile.SetVelocity(-direction, 1);
        projectile.speed = ArrowSpeed;
    }
}
