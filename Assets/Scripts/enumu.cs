using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enumu : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;


    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -10)
        {
            float randoX = Random.Range(-29f, 29f);
            transform.position = new Vector3(randoX, 33, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Enemy damaged");
            Destroy(this.gameObject);
        }
        else if (other.tag == "Layyzurr")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}