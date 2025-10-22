using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Transform sword;
    public float tiltAngle = 60f; 
    public float moveDuration = 0.3f; 
    public float fastMoveDuration = 0.1f; 

    private bool isAnimating = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.R) && Input.GetMouseButtonDown(0) && !isAnimating)
        {
            StartCoroutine(PerformSwordAttack(-Camera.main.transform.right)); 
        }

        if (Input.GetKey(KeyCode.F) && Input.GetMouseButtonDown(0) && !isAnimating)
        {
            StartCoroutine(PerformSwordAttack(Camera.main.transform.right, 80f));
        }

        if (Input.GetKey(KeyCode.V) && Input.GetMouseButtonDown(0) && !isAnimating)
        {
            StartCoroutine(PerformForwardAttack(180f)); 
        }
    }

    private System.Collections.IEnumerator PerformSwordAttack(Vector3 tiltDirection, float customTiltAngle = -1f)
    {
        isAnimating = true;

        float angleToUse = customTiltAngle > 0 ? customTiltAngle : tiltAngle;

        Quaternion originalRotation = sword.localRotation;

        Quaternion targetRotation = Quaternion.AngleAxis(angleToUse, tiltDirection) * sword.rotation;

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            sword.rotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / moveDuration);
            yield return null;
        }

        sword.rotation = targetRotation;

        yield return new WaitForSeconds(0.1f);

        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            sword.localRotation = Quaternion.Slerp(targetRotation, originalRotation, elapsedTime / moveDuration);
            yield return null;
        }

        sword.localRotation = originalRotation;

        isAnimating = false;
    }

    private System.Collections.IEnumerator PerformForwardAttack(float forwardAngle)
    {
        isAnimating = true;

        Quaternion originalRotation = sword.localRotation;

        Quaternion forwardRotation = Quaternion.AngleAxis(forwardAngle, Camera.main.transform.up) * sword.rotation;

        float elapsedTime = 0f;
        while (elapsedTime < fastMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            sword.rotation = Quaternion.Slerp(originalRotation, forwardRotation, elapsedTime / fastMoveDuration);
            yield return null;
        }

        sword.rotation = forwardRotation;

        yield return new WaitForSeconds(0.05f);

        elapsedTime = 0f;
        while (elapsedTime < fastMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            sword.localRotation = Quaternion.Slerp(forwardRotation, originalRotation, elapsedTime / fastMoveDuration);
            yield return null;
        }

        sword.localRotation = originalRotation;

        isAnimating = false;
    }
}
