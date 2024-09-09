using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public float swingSpeed = 5f; // Speed of the swing
    public float swingAngle = 60f;  // Maximum angle of the swing
    private bool isSwinging = false;
    private float currentSwingTime = 0f;
    private Vector3 initialRotation;

    private bool returning = false;

    void Start()
    {
        initialRotation = transform.localEulerAngles;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isSwinging)
        {
            isSwinging = true;
            returning = false;
            currentSwingTime = 0f;
        }

        if (isSwinging)
        {
            SwingWeapon();
        }
    }

    void SwingWeapon()
    {
        currentSwingTime += Time.deltaTime * swingSpeed;

        if (!returning)
        {
            float angle = Mathf.Lerp(0, swingAngle, currentSwingTime);
            transform.localEulerAngles = new Vector3(angle, initialRotation.y, initialRotation.z);

            if (currentSwingTime >= 1f)
            {
                currentSwingTime = 0f;
                returning = true;
            }
        }
        else
        {
            float angle = Mathf.Lerp(swingAngle, 0, currentSwingTime);
            transform.localEulerAngles = new Vector3(angle, initialRotation.y, initialRotation.z);

            if (currentSwingTime >= 1f)
            {
                transform.localEulerAngles = initialRotation; 
                isSwinging = false;
                returning = false;
            }
        }
    }
}

