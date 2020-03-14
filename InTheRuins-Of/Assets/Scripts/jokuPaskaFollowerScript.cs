using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jokuPaskaFollowerScript : MonoBehaviour
{
    public Transform mTarget;
    float mSpeed = 10.0f;
    Vector3 mLookDirection;
    const float EPSILON = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        mLookDirection = (mTarget.position - transform.position).normalized;

        if ((transform.position - mTarget.position).magnitude > EPSILON)
        {
            transform.Translate(mLookDirection * Time.deltaTime * mSpeed);
        } 
    }
}
 