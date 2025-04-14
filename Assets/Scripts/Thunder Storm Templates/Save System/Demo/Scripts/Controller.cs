using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    public float movementSpeed = 4f;
    public float rotationSpeed = 3f;
    public float jumpForce = 5.5f;
    [Space]
    public SavableObject savableSpawnTest;


    private void Update()
    {
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");

        transform.Translate(Vector3.forward * y * movementSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * x * rotationSpeed);

        if (Input.GetKeyDown(KeyCode.Space))
            GetComponent<Rigidbody>().linearVelocity += Vector3.up * jumpForce;


        if (Input.GetKeyDown(KeyCode.Q))
        {
            Instantiate(savableSpawnTest.gameObject, new Vector3(transform.position.x, transform.position.y + 10, transform.position.z), Quaternion.identity).GetComponent<SavableObject>();
        }

    }
}
