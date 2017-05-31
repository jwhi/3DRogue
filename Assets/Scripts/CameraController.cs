// Modified code found here: http://answers.unity3d.com/questions/25965/camera-orbit-on-mouse-drag.html


using UnityEngine;
using System.Collections;
[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]

public class CameraController : MonoBehaviour
{
    public Transform target;
    public GameObject playerIcon;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    public float distanceMin = .5f;
    public float distanceMax = 15f;
    public float smoothTime = 2f;
    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;
    float velocityX = 0.0f;
    float velocityY = 0.0f;
    
    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = 89;
        
    }
    void LateUpdate()
    {
        if (target)
        {
            
            if (Input.GetMouseButton(0))
            {
                velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
            }
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                //distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

            // Rotate player object to face camera
            Vector3 n = transform.position - playerIcon.transform.position;
            //playerIcon.transform.rotation = Quaternion.LookRotation(n);

            // Quaternion newRotation = Quaternion.LookRotation(n);
            // newRotation.y = 0;
            // newRotation.z = 0;
            
            Quaternion look = Quaternion.LookRotation(n);

            float y = look.eulerAngles.y;
            float locked = Mathf.RoundToInt(y / 90.0f) * 90.0f;

            Quaternion newRotation = Quaternion.Euler(look.eulerAngles.x, locked, look.eulerAngles.z);
            playerIcon.transform.rotation = Quaternion.Slerp(playerIcon.transform.rotation, newRotation, Time.deltaTime * 12.0f);


        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}