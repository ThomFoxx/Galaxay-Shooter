using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLance : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 4.5f;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);
        if (transform.position.y <= -8)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                if (transform.parent != null)
                {
                    if (transform.parent.childCount <= 2)
                    {
                        Destroy(transform.parent.gameObject);
                    }
                }
            }
            Destroy(this.gameObject);
        }
    }
}
