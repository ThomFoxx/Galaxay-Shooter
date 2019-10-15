﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultipler = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _misfirePrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _ammo = 50;
    private SpawnManager _spawnManager;
    private UIManager _canvas;
    [SerializeField]
    private bool _isTripleShotActive = false, _isSpeedBoostActive = false, _isShieldActive = false;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _playerShield;
    [SerializeField]
    private Transform _shieldTransform;
    [SerializeField]
    private int _shieldStrength = 0;
    private int _score;
    [SerializeField]
    private GameObject[] _engines; //0 = Right 1 = Left
    [SerializeField]
    private GameObject _thrusters;
    [SerializeField]
    private AudioClip _laserSound;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionSound;
    private GameObject _explosion;
    [SerializeField]
    private GameObject _shipExplosion;
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private float _thursterPower = 2.5f;
    private float _boost = 1;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _audioSource = GetComponent<AudioSource>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _canvas = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.Log("SpriteRenederer is NULL.");
        }
        if (_spawnManager == null)
        {
            Debug.Log("The Spawn Manager is NULL.");
        }  
        if (_canvas == null)
        {
            Debug.Log("The UI Manager is NULL.");
        }
        _playerShield.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if ((Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Fire1")) && Time.time > _canFire)
        {
            _misfirePrefab.SetActive(false);
            FireLaser();
        }

    }

    void CalculateMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift)) //On LFT Shift hold, increase Boost to match Thruster Power.
        {
            _boost = _thursterPower;
        }
        else { _boost = 1; } //on release, return Boost to 1.
        float horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");
        float verticalInput = CrossPlatformInputManager.GetAxis("Vertical");
        if (_isSpeedBoostActive == false)
        {
            transform.Translate(Vector3.right * horizontalInput * _speed * _boost * Time.deltaTime);
            transform.Translate(Vector3.up * verticalInput * _speed * _boost * Time.deltaTime);
        }
        else if (_isSpeedBoostActive == true)
        {
            transform.Translate(Vector3.right * horizontalInput * (_speed * _speedMultipler) * _boost * Time.deltaTime);
            transform.Translate(Vector3.up * verticalInput * (_speed * _speedMultipler) * _boost * Time.deltaTime);
        }
        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -4)
        {
            transform.position = new Vector3(transform.position.x, -4, 0);
        }

        if (transform.position.x > 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x < -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        if (_ammo > 0) //only ammo firing whern ammo is available.
        {
            if (_isTripleShotActive == true)
            {
                _canFire = Time.time + (_fireRate * 2);
                Vector3 offset = new Vector3(transform.position.x, (transform.position.y + 1.1f), 0);
                Instantiate(_tripleShotPrefab, offset, Quaternion.identity);
                _audioSource.PlayOneShot(_laserSound);
            }
            else if (_isTripleShotActive == false)
            {
                _canFire = Time.time + _fireRate;
                Vector3 offset = new Vector3(transform.position.x, (transform.position.y + 1.1f), 0);
                Instantiate(_laserPrefab, offset, Quaternion.identity);
                _audioSource.PlayOneShot(_laserSound);
            }
            _ammo -= 1; //remove one round
        }
        else if (_ammo == 0)
        {
            _misfirePrefab.SetActive(true);
        }
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            float ShieldScaleX = _shieldTransform.localScale.x;
            float ShieldScaleY = _shieldTransform.localScale.y;
            if (_shieldStrength > 1)
            {
                _shieldStrength -= 1;
                ShieldScaleX -= .5f;
                ShieldScaleY -= .5f;
                _shieldTransform.localScale = new Vector3(ShieldScaleX, ShieldScaleY, 2f);
            }
            else if (_shieldStrength == 1)
            {
                _shieldStrength = 0;
                _isShieldActive = false;
                _playerShield.SetActive(false);
            }
            return;
        }

        _lives --;
        _canvas.UpdateLives(_lives);
        int randomEngine = Random.Range(0, 2);
        if (_lives == 2  )
        {
            _engines[randomEngine].SetActive(true);
        }
        else if (_lives == 1)
        {
            _engines[0].SetActive(true);
            _engines[1].SetActive(true);
        }
        else if (_lives < 1)
        {
            _speed = 0f;
            _explosion = Instantiate(_shipExplosion, transform.position, Quaternion.identity);
            _engines[0].SetActive(false);
            _engines[1].SetActive(false);
            _thrusters.SetActive(false);
            _spawnManager.OnPlayerDeath();
            _audioSource.PlayOneShot(_explosionSound);
            StartCoroutine(ShipDestructionRoutine());
            _canvas.GameOver();
        }
    }

    public void Repair()
    {
        if (_lives < 3)
        {
            Debug.Log("Repair Collected, Live > 3");
            _lives += 1;
            if (_engines[0].activeInHierarchy)
            {
                _engines[0].SetActive(false);
            }
            else
            {
                _engines[1].SetActive(false);
            }
        }
        else if (_lives == 3)
        {
            _ammo += 10;
        }
    }

    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        _ammo += 25; //add ammo when triple shot is picked up.
        if (_ammo > 50)
        { _ammo = 50; } //limit ammo to 50 count.
        StartCoroutine(TripleShotPowerDownRoutine());
       
    }

    public void ActivateSpeedBoost()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ActivateShields()
    {
        _shieldTransform.localScale = new Vector3(2, 2, 2);
        _shieldStrength = 3;
        _isShieldActive = true;
        _playerShield.SetActive(true);
    }

    public void RechargeAmmo()
    {
        _ammo = 50;
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            _isTripleShotActive = false;
        }
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);
            _isSpeedBoostActive = false;
        }
    }

    IEnumerator ShipDestructionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            _spriteRenderer.enabled = false;
            Destroy(_explosion.gameObject, 3.0f);
            Destroy(this.gameObject, 3f);
        }
    }

}
