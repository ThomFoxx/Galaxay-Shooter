using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _powerupSpeed = 3.0f;
    [SerializeField] // 0 = TripleShot  1 = SpeedBoost  2 = Shields
    private int _powerupID;
    private AudioSource _audiosource;
    [SerializeField]
    private AudioClip _pickupSound;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);
        if (transform.position.y <= -7)
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
                AudioSource.PlayClipAtPoint(_pickupSound, transform.position);
                switch (_powerupID)
                {
                    case 0:
                        player.ActivateTripleShot();
                        break;
                    case 1:
                        player.ActivateSpeedBoost();
                        break;
                    case 2:
                        player.ActivateShields();
                        break;
                    case 3:
                        player.RechargeAmmo();
                        break;
                    case 4:
                        player.Repair();
                        break;
                    case 5:
                        player.ActivateFanBurst();
                        break;
                    default:
                        Debug.Log("Non standard PowerUp Value.");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
