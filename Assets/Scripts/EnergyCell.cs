using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCell : MonoBehaviour
{
    private int _rightLight = 4;
    private int _leftLight = -4;
    private bool _isMovingRight = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float positionX = transform.localPosition.x;
        if (_isMovingRight)
        {
            if (positionX < _rightLight)
            {
                transform.Translate(Vector3.right * .75f * Time.deltaTime);
            }
            else if (positionX >= _rightLight)
            {
                _isMovingRight = false;
            }
        }
        if (!_isMovingRight)
        {
            if (positionX > _leftLight)
            {
                transform.Translate(Vector3.left * .75f * Time.deltaTime);
            }
            else if (positionX <= _leftLight)
            {
                _isMovingRight = true;
            }
        }
        Debug.Log(positionX);
    }
}
