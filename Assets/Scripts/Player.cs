using System.Collections;
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
    private bool _isTripleShotActive = false, _isSpeedBoostActive = false, _isShieldActive = false, _isFanBurstActive = false;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _fanBurstPrefab;
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
    private float _thrusterPower = 2.5f;
    private float _boost = 1;
    [SerializeField]
    private float _thrusterReserve = 100f;
    private bool _canBoost = true;
    [SerializeField]
    private CameraShake _camera;
    [SerializeField]
    private int _enemyKillCount = 0;
    [SerializeField]
    private GameObject _EMP;
    private bool _isEMPActive = false;


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
        _canvas.UpdateThruster(_thrusterReserve);
        _canvas.UpdateAmmo(_ammo);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isEMPActive)
        {
            CalculateMovement();
            if ((Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Fire1")) && Time.time > _canFire)
            {
                _misfirePrefab.SetActive(false);
                FireLaser();
            }
        }
    }

    void CalculateMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _thrusterReserve >= 0f && _canBoost) //On LFT Shift hold, increase Boost to match Thruster Power.
        {
            _boost = _thrusterPower;
            _thrusterReserve = _thrusterReserve - (5f * Time.deltaTime);
            _canvas.UpdateThruster(_thrusterReserve);
            if ( _thrusterReserve <1f)
            {
                _canBoost = false;
            }
        }
        else if (_thrusterReserve < 100f && !Input.GetKey(KeyCode.LeftShift))
        {
            _thrusterReserve = _thrusterReserve + (1f * Time.deltaTime);
            _canvas.UpdateThruster(_thrusterReserve);
            if (_thrusterReserve > 50f)
            {
                _canBoost = true;
            }
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
            else if (_isFanBurstActive == true)
            {
                _canFire = Time.time + (_fireRate * 2);
                Vector3 offset = new Vector3(transform.position.x, (transform.position.y + 1.1f), 0);
                Instantiate(_fanBurstPrefab, offset, Quaternion.identity);
                _audioSource.PlayOneShot(_laserSound);
            }
            else if (_isTripleShotActive == false && _isFanBurstActive == false)
            {
                _canFire = Time.time + _fireRate;
                Vector3 offset = new Vector3(transform.position.x, (transform.position.y + 1.1f), 0);
                Instantiate(_laserPrefab, offset, Quaternion.identity);
                _audioSource.PlayOneShot(_laserSound);
            }
            _ammo -= 1; //remove one round
            _canvas.UpdateAmmo(_ammo);
        }
        else if (_ammo == 0)
        {
            _misfirePrefab.SetActive(true);
        }
    }

    public void Damage()
    {
        if (_isShieldActive == true && !_isEMPActive)
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
        StartCoroutine(_camera.Shake(.25f, .1f));
        _lives --;
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
        _canvas.UpdateLives(_lives);

    }

    public void EnemyKill()
    {
        _enemyKillCount++;
    }

    public void RestKillCount()
    {
        _enemyKillCount = 0;
    }

    public int KillCount()
    {
        return _enemyKillCount;
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
            if (_ammo > 50)
            { _ammo = 50; }
            _canvas.UpdateAmmo(_ammo);
        }
        _canvas.UpdateLives(_lives);
    }

    public void ActivateTripleShot()
    {
        StopCoroutine(TripleShotPowerDownRoutine());
        _isTripleShotActive = true;
        _ammo += 25; //add ammo when triple shot is picked up.
        if (_ammo > 50)
        { _ammo = 50; } //limit ammo to 50 count.
        _canvas.UpdateAmmo(_ammo);
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void ActivateFanBurst()
    {
        StopCoroutine(FanBurstPowerDownRoutine());
        _isFanBurstActive = true;
        _ammo += 25; //add ammo when triple shot is picked up.
        if (_ammo > 50)
        { _ammo = 50; } //limit ammo to 50 count.
        _canvas.UpdateAmmo(_ammo);
        StartCoroutine(FanBurstPowerDownRoutine());

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

    public void ActivateEMP()
    {
        _EMP.SetActive(true);
        _isEMPActive = true;
        if(_isShieldActive)
        {
            _playerShield.SetActive(false);
        }
        _thrusters.SetActive(false);
        StartCoroutine(EMPCoolDownRoutine());
    }

    public void RechargeAmmo()
    {
        _ammo = 50;
        _canvas.UpdateAmmo(_ammo);
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        while (_isTripleShotActive)
        {
            yield return new WaitForSeconds(5f);
            _isTripleShotActive = false;
        }
    }

    IEnumerator FanBurstPowerDownRoutine()
    {
        while (_isFanBurstActive)
        {
            yield return new WaitForSeconds(5f);
            _isFanBurstActive = false;
        }
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        while(_isSpeedBoostActive)
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

    IEnumerator EMPCoolDownRoutine()
    {
        while(_isEMPActive)
        {
            yield return new WaitForSeconds(3f);
            if (_shieldStrength >0)
            {
                _playerShield.SetActive(true);
            }
            _thrusters.SetActive(true);
            _EMP.SetActive(false);
            _isEMPActive = false;
        }
    }

}
