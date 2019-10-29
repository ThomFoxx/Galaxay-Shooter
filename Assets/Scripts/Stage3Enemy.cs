using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3Enemy : MonoBehaviour
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
    [SerializeField]
    private GameObject _thruster;
    private bool _isDead = false;
    private bool _hasFired = false;
    [SerializeField]
    private GameObject _shield;
    private bool _isShieldActive = false;

    private void Start()
    {
        _audiosource = GetComponent<AudioSource>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.Log("Player is NUll in Enemy2.");
        }
        _canvas = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_canvas == null)
        {
            Debug.Log("UIManager is NULL in Enemy2.");
        }
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("Spawn Manager is NULL in Enemy2.");
        }
        _animator = transform.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.Log("Animator is NULL in Enemy2.");
        }
        _collider = transform.GetComponent<BoxCollider2D>();
        _lastFire = Time.time;
        int shieldChance = Random.Range(0, 100);
        if (shieldChance <= (3 * _spawnManager.Wave()))
        {
            _isShieldActive = true;
            _shield.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        float _randomFire = Random.Range(5f, 10f) / _spawnManager.Wave();
        if (Time.time > (_lastFire + _randomFire) && !_isDead && !_hasFired)
        {
            FireLaser();
        }

    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.up * _enemySpeed * Time.deltaTime);
        if (transform.position.y <= -6 && !_spawnManager.SpawningStopped())
        {
            float randomX = Random.Range(-10.0f, 10.0f);
            transform.position = new Vector3(randomX, 9, 0);
        }
        else if (transform.position.y <= -6 && _spawnManager.SpawningStopped())
        {
            Destroy(this.gameObject);
        }
        else if (transform.position.y >= 10)
        {
            Destroy(this.gameObject);
        }
    }

    private void FireLaser()
    {
        Vector3 rotation = transform.localEulerAngles;
        rotation.z = rotation.z - 180;
        _lastFire = Time.time;
        Instantiate(_enemyLaser, transform.position, Quaternion.Euler(rotation));
        AudioSource.PlayClipAtPoint(_laserSound, transform.position);
        transform.rotation = new Quaternion(0,0,0,0);
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
            if (!_isShieldActive)
            {
                _isDead = true;
                _thruster.SetActive(false);
                _animator.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0.5f;
                _collider.enabled = false;
                _audiosource.PlayOneShot(_explosionSound);
                Destroy(this.gameObject, 3.5f);
            }
            else if (_isShieldActive)
            {
                _isShieldActive = false;
                _shield.SetActive(false);
            }
        }

        if (other.tag == "Laser")
        {
            if (!_isShieldActive)
            {
                _canvas.UpdateScore(10);
                _player.EnemyKill();
                _isDead = true;
                Destroy(other.gameObject);
                _thruster.SetActive(false);
                _animator.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0.5f;
                _collider.enabled = false;
                _audiosource.PlayOneShot(_explosionSound);
                Destroy(this.gameObject, 3.5f);
            }
            else if (_isShieldActive)
            {
                _isShieldActive = false;
                _shield.SetActive(false);
            }
        }

    }

}
