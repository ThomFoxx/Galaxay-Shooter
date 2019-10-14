using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4f;
    private UIManager _canvas;
    private Animator _animator;
    private Player _player;
    private BoxCollider2D _collider;
    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _audiosource;
    [SerializeField]
    private GameObject _enemyLaser;
    private float _lastFire;
    [SerializeField]
    private AudioClip _laserSound;
    private bool _isDead = false;

    private void Start()
    {
        _audiosource = GetComponent<AudioSource>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.Log("Player is NUll.");
        }
        _canvas = GameObject.Find("Canvas").GetComponent<UIManager>();
       if (_canvas == null)
        {
            Debug.Log("UIManager is NULL.");
        }
        _animator = transform.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.Log("Animator is NULL.");
        }
        _collider = transform.GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        float _randomFire = Random.Range(2f, 7f);
        if (Time.time > (_lastFire + _randomFire) && _isDead != true)
        {
            FireLaser();
        }

    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y <= -6)
        {
            float randomX = Random.Range(-10.0f, 10.0f);
            transform.position = new Vector3(randomX, 9, 0);
        }
    }

    private void FireLaser()
    {
        _lastFire = Time.time;
        Instantiate(_enemyLaser, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(_laserSound, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            _isDead = true;
            _animator.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0.5f;
            _collider.enabled = false;
            _audiosource.PlayOneShot(_explosionSound);
            Destroy(this.gameObject, 3.5f);

        }

        if (other.tag == "Laser")
        {
            _canvas.UpdateScore(10);
            _isDead = true;
            Destroy(other.gameObject);
            _animator.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0.5f;
            _collider.enabled = false;
            _audiosource.PlayOneShot(_explosionSound);
            Destroy(this.gameObject, 3.5f);

        }

    }

}
