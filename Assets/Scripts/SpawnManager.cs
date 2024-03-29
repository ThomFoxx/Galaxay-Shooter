﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerupContainer;
    [SerializeField]
    private GameObject[] _powerupPrefabs;
    [SerializeField]
    private GameObject _asteroid;
    private GameObject _newEnemy;
    private Player _player;
    private UIManager _canvas;
    private bool _stopSpawning = false;
    [SerializeField]
    private int _wave;
    private int _randomRot;
    private int[] _normalLoot = 
        {
        70,     //Triple Shot
        70,     //Speed PowerUp
        50,     //Shield
        100,    //Energy Cell (Ammo)
        30,     //Repair Cell (Health)
        70,     //Burst Shot
        5       //EMP (Penalty)
        };
    private int[] _shieldLoot = 
        {
        70,     //Triple Shot
        70,     //Speed PowerUp
        10,     //Shield
        100,    //Energy Cell (Ammo)
        30,     //Repair Cell (Health)
        70,     //Burst Shot
        5       //EMP (Penalty)
        };
    private int[] _damageLoot = 
        {
        70,     //Triple Shot
        70,     //Speed PowerUp
        30,     //Shield
        100,    //Energy Cell (Ammo)
        50,     //Repair Cell (Health)
        70,     //Burst Shot
        5       //EMP (Penalty)
        };

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.Log("Player is NULL in SpawnManager.");
        }
        _canvas = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_canvas == null)
        {
            Debug.Log("Canvas is NULL in SpawnManager.");
        }
        _normalLoot[6] = _normalLoot[6] * _wave;
        _shieldLoot[6] = _shieldLoot[6] * _wave;
        _damageLoot[6] = _damageLoot[6] * _wave;
    }

    public void StartSpawning()
    {
        _stopSpawning = false;
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());

    }

    // Update is called once per frame
    void Update()
    {
        if (_player.KillCount() >= (_wave * 5))
        {
            _stopSpawning = true;
            StopCoroutine(SpawnEnemyRoutine());
            StopCoroutine(SpawnPowerupRoutine());
            _wave++;
            StartCoroutine(SpawnAsteroid());
            _canvas.UpdateWave(_wave);
            _canvas.ShowWave(true);
            _player.RestKillCount();
        }
    }

    //spawn gameobjects every 5 seconds
    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);        
        while (_stopSpawning == false)
        {
            float randomX = Random.Range(-10f, 10f);
            int randomEnemy = Random.Range(0, _wave);
            switch (_wave)
            {
                case 1:
                    _newEnemy = Instantiate(_enemyPrefab[0], new Vector3(randomX, 9, 0), Quaternion.identity);
                    break;
                case 2:
                    _randomRot = Random.Range(-45, 45);
                    if (randomEnemy != 0)
                    {
                        _randomRot = _randomRot - 180;
                    }
                    _newEnemy = Instantiate(_enemyPrefab[randomEnemy], new Vector3(randomX, 9, 0), Quaternion.Euler(0, 0, _randomRot));
                    break;
                case 3:
                    _randomRot = Random.Range(-45, 45);
                    if (randomEnemy != 0)
                    {
                        _randomRot = _randomRot - 180;
                    }
                    _newEnemy = Instantiate(_enemyPrefab[randomEnemy], new Vector3(randomX, 9, 0), Quaternion.Euler(0, 0, _randomRot));
                    break;
                default:
                    break;
            }
            _newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(Random.Range(5f,10f)/_wave);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            float randomX = Random.Range(-10f, 10f);
            float randomTime = Random.Range(3f, 7f);
            //int randomPowerUp = Random.Range(0, 7);
            GameObject newPowerup = Instantiate(_powerupPrefabs[RandomPowerUP()], new Vector3(randomX, 9, 0), Quaternion.identity);
            newPowerup.transform.parent = _powerupContainer.transform;
            yield return new WaitForSeconds(randomTime);
        }
    }

    IEnumerator SpawnAsteroid()
    {
        yield return new WaitForSeconds(5f);
        Instantiate(_asteroid, new Vector3(0, 7, 0), Quaternion.identity);
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    private int RandomPowerUP()
    {
        int[] table;
        int total = 0;
        int randomNum = 0;
        if(_player.ShipDamaged())
        {   table = _damageLoot; }
        else if (_player.ShieldActive())
        {   table = _shieldLoot; }
        else
        {   table = _normalLoot;}
        foreach(var item in table)
        { total += item; }
        randomNum = Random.Range(0, total);
        for (int i = 0; i < table.Length; i++)
        {
            if (randomNum <= table[i])
            {
                return i;
            }
            else
            {
                randomNum -= table[i];
            }
        }
        return 0;
    }

    public bool SpawningStopped()
    {
        return _stopSpawning;
    }

    public int Wave()
    {
        return _wave;
    }
}
