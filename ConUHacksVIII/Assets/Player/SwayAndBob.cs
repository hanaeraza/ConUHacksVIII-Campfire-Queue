using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Original script written by BuffaLou.
public class SwayAndBob : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [Header("Sway")]
    [SerializeField] float step = 0.01f;
    [SerializeField] float maxStepDistance = 0.06f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    [SerializeField] float rotationStep = 4f;
    [SerializeField] float maxRotationStep = 5f;
    Vector3 swayEulerRot; 

    [SerializeField] float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    [SerializeField] float speedCurve;
    float curveSin {get => Mathf.Sin(speedCurve);}
    float curveCos {get => Mathf.Cos(speedCurve);}

    [SerializeField] Vector3 travelLimit = Vector3.one * 0.025f;
    [SerializeField] Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobPosition;

    [SerializeField] float bobExaggeration = 1;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    // Update is called once per frame
    void Update()
    {
        GetInput();

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }


    Vector2 walkInput;
    Vector2 lookInput;

    void GetInput(){
        walkInput.x = Input.GetAxis("Horizontal");
        walkInput.y = Input.GetAxis("Vertical");
        walkInput = walkInput.normalized;

        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");
    }

    void Sway(){
        Vector3 invertLook = lookInput *-step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation(){
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation(){
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void BobOffset(){
        speedCurve += Time.deltaTime * (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * player.CurrentSpeed * bobExaggeration + 0.01f;
        bobPosition.x = (curveCos*bobLimit.x*(player.IsGrounded ? 1:0))-(walkInput.x * travelLimit.x);
        bobPosition.y = (curveSin*bobLimit.y)-(Input.GetAxis("Vertical") * travelLimit.y);
        bobPosition.z = -(walkInput.y * travelLimit.z);
    }

    void BobRotation(){
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2*speedCurve)) : multiplier.x * (Mathf.Sin(2*speedCurve) / 2));
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);
    }

}