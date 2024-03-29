﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteriod : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 11.0f;
    [SerializeField]
    private GameObject _asteriodExplosion;
    private SpriteRenderer _spriteRenderer;
    private GameObject _explosion;
    private SpawnManager _spawnManager;
    private UIManager _canvas;
    [SerializeField]
    private AudioClip _explosionSound;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("spawnManager is NULL.");
        }
        _canvas = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_canvas == null)
        {
            Debug.Log("Canvas is NULL in Asteriod.");
        }
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.Log("SpriteRenederer is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= 3 )
        {
            transform.Translate(Vector3.down * Time.deltaTime);
        }
        else
        {
            transform.Rotate(0, 0, _rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            AudioSource audiosource = GetComponent<AudioSource>();
            _explosion = Instantiate(_asteriodExplosion, transform.position, Quaternion.identity);
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            audiosource.PlayOneShot(_explosionSound);
            StartCoroutine(AsteriodDestructionRoutine());
            _canvas.ShowWave(false);
        }
        new WaitForSeconds(2f);
        _spawnManager.StartSpawning();

    }

    IEnumerator AsteriodDestructionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            _spriteRenderer.enabled = false;
            Destroy(_explosion.gameObject, 3.0f);
            Destroy(this.gameObject,3f);
        }
    }
}
