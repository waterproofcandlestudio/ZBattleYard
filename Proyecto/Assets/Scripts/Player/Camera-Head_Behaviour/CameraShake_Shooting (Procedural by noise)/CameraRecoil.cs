using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [Header("Hipfire recoil")]
    [SerializeField] float recoilX = -2;
    [SerializeField] float recoilY = 2;
    [SerializeField] float recoilZ = 0.35f;

    [Header("ADS recoil")]
    [SerializeField] float aimRecoilX = -1.5f;
    [SerializeField] float aimRecoilY = 1;
    [SerializeField] float aimRecoilZ = 0.3f;

    [Header("Settings")]
    [SerializeField] float snappiness = 6;
    [SerializeField] float returnSpeed = 2;

    bool isAiming;
    Vector3 currentRotation;
    Vector3 targetRotation;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime); // "Slerp()" works better with rotations, that's why I use it instead of "Lerp()"

        transform.localRotation = Quaternion.Euler(currentRotation); // Start Rotation
    }

    public void RecoilFire() // Recoil calculations with randomness
    {
        if(isAiming) 
            targetRotation += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), Random.Range(-aimRecoilZ, aimRecoilZ)); // Recoil While aiming
        else 
            targetRotation += new Vector3 (recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ)); // Normal Recoil
    }
}
