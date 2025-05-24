using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    private CameraLook cam;

    public WindowHandler windowHandler;
    public bool invertHorizontal;
    public bool invertVertical;
    public bool lockSway;


    [Header("Position")]
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;

    [Header("Rotation")]
    public float rotationAmount = 4f;
    public float maxRotationAmount = 5f;
    public float smoothRotation = 12f;
    [Space]
    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;


    Vector3 initialPosition;
    Quaternion initialRotation;


    float InputX;
    float InputY;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<CameraLook>();
        windowHandler = GetComponentInParent<WindowHandler>();

        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        MoveSway();
        TiltSway();

        if (cam.canMove && cam.verticalRot > cam.verticalTopLimit && cam.verticalRot < cam.verticalBottomLimit && !lockSway && !lockSway && !GetComponentInParent<Weapon>().isAiming)
            CalculateVerticalSway();
        else
            InputY = 0;



        if (cam.canMove && !lockSway && !GetComponentInParent<Weapon>().isAiming && !windowHandler.isOpen)
            CalculateHorizontalSway();
        else
            InputX = 0;

    }

    void CalculateVerticalSway()
    {
        InputY = -Input.GetAxis("Mouse Y");
    }


    void CalculateHorizontalSway()
    { 
        InputX = -Input.GetAxis("Mouse X");
    }

    void MoveSway()
    {
        float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }

    void TiltSway()
    {
        float tiltY = Mathf.Clamp(InputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float tiltX = Mathf.Clamp(InputY * rotationAmount, -maxRotationAmount, maxRotationAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0f, rotationZ ? tiltY : 0));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothRotation);
    }
}
