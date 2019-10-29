using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage4Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4f;
    private UIManager _canvas;
    private SpawnManager _spawnManager;
    private Animator _animator;
    private AudioSource _audioSource;
    private Player _player;
    private Collider2D _collider;
    [SerializeField]
    private AudioClip _explosionSound;
    private bool _isDead = false;
    [SerializeField]
    private GameObject _shield;
    private bool _isShieldActive = false;
    private Transform _target;

    private void Start()
    {
        _audioSource = transform.GetComponent<AudioSource>();
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
        _collider = transform.GetComponent<Collider2D>();
        int shieldChance = Random.Range(0, 100);
        if (shieldChance <= (2 * _spawnManager.Wave()))
        {
            _isShieldActive = true;
            _shield.SetActive(true);
        }
        _target = _player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        float _randomFire = Random.Range(3f, 8f) / _spawnManager.Wave();

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
                _animator.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0.5f;
                _collider.enabled = false;
                _audioSource.PlayOneShot(_explosionSound);
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
                _animator.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0.5f;
                _collider.enabled = false;
                _audioSource.PlayOneShot(_explosionSound);
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
