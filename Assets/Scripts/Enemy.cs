using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4f;
    private UIManager _canvas;
    private SpawnManager _spawnManager;
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
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("Spawn Manager is NULL in Enemy.");
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
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime, Space.World);
        if (transform.position.y <= -6 && !_spawnManager.SpawningStopped())
        {
            float randomX = Random.Range(-10.0f, 10.0f);
            transform.position = new Vector3(randomX, 9, 0);
        }
        else if (transform.position.y <= -6 && _spawnManager.SpawningStopped())
        {
            Destroy(this.gameObject);
        }
    }

    private void FireLaser()
    {
        Vector3 rotation = transform.localEulerAngles;
        _lastFire = Time.time;
        Instantiate(_enemyLaser, transform.position, Quaternion.Euler(rotation));
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
            _player.EnemyKill();
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
