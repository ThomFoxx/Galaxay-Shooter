using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 8.0f;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);
        if (transform.position.y >= 8 )
        {
            if (transform.parent != null)
            {
                if (transform.parent.childCount <= 2)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
            Destroy(this.gameObject);
        }
    }
}
