using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = Vector3.zero;
    [SerializeField] private float period;

    float movementFactor;   // 0 for not moved, 1 for fully moved.
    Vector3 startPos;
    
    // Use this for initialization
    void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        OnObstacleMoving();
    }

    private void OnObstacleMoving()
    {
        if (period >= Mathf.Epsilon)
        {
            float cycles = Time.time / period;  // Grows continually from 0.
            const float tau = Mathf.PI * 2f;    // about 6.28
            float rawSinWave = Mathf.Sin(cycles * tau);
            //movementFactor = (Mathf.Sin(Time.time * period) + 1) * 0.5f;
            movementFactor = rawSinWave * 0.5f + 0.5f;
            Vector3 offset = movementVector * movementFactor;
            transform.position = startPos + offset;
        }

        //switch (period > 0)
        //{
        //    case true:
        //        float cycles = Time.time / period;
        //        const float tau = Mathf.PI * 2f;
        //        float rawSinWave = Mathf.Sin(cycles * tau);
        //        //movementFactor = (Mathf.Sin(Time.time * period) + 1) * 0.5f;
        //        movementFactor = rawSinWave * 0.5f + 0.5f;
        //        Vector3 offset = movementVector * movementFactor;
        //        transform.position = startPos + offset;
        //        break;
        //    default:
        //        break;
        //}        
    }
}
