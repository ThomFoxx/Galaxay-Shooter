using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid2 : MonoBehaviour
{
    float rotationSpeed;
    Animator explosionAnimator;
    Collider2D explosionCollider;
    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = 10f;
        explosionAnimator = GetComponent<Animator>();
        explosionCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            explosionAnimator.SetTrigger("onAsteroidDestroy");
            Destroy(this.gameObject, 5.5f);
        }
    }
}