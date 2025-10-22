using UnityEngine;

public class CubeBehaviorWithMultipleEffects : MonoBehaviour
{
    public Transform character;
    public float detectionDistance = 5f; 
    private bool isProcessing = false; 
    public float returnSpeed = 2f; 
    public float teleportDistance = 1f; 
    public int cubeHP = 20; 

    private Vector3 originalPosition;
    private Vector3 originalSize; 
    private Renderer cubeRenderer; 
    private Vector3[] sizeOptions = new Vector3[]
    {
        new Vector3(8, 2, 2),
        new Vector3(2, 8, 8),
        new Vector3(4, 4, 4)
    };

    private bool flashRed = false; 
    private bool isDestroyed = false; 

    void Start()
    {
        originalPosition = transform.position;
        originalSize = transform.localScale;
        cubeRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        float distanceToCharacter = Vector3.Distance(transform.position, character.position);

        if (distanceToCharacter <= detectionDistance && !isProcessing && !isDestroyed)
        {
            // Start the process
            isProcessing = true;
            StartCoroutine(ProcessBehavior());
        }

        if (Input.GetKey(KeyCode.R) && Input.GetMouseButton(0) && transform.localScale == new Vector3(8, 2, 2))
        {
            flashRed = true;
        }
        else if (Input.GetKey(KeyCode.F) && Input.GetMouseButton(0) && transform.localScale == new Vector3(2, 8, 8))
        {
            flashRed = true;
        }
        else if (Input.GetKey(KeyCode.V) && Input.GetMouseButton(0) && transform.localScale == new Vector3(4, 4, 4))
        {
            flashRed = true;
        }
    }

    System.Collections.IEnumerator ProcessBehavior()
    {
        FaceCharacter();

        yield return new WaitForSeconds(3f);
        ChangeSize();

        yield return new WaitForSeconds(1f);
        TeleportInFrontOfPlayer();

        yield return new WaitForSeconds(1f);
        yield return SmoothReturn(originalPosition, originalSize);

        isProcessing = false;
    }

    void FaceCharacter()
    {
        Vector3 directionToCharacter = character.position - transform.position;
        directionToCharacter.y = 0;

        if (directionToCharacter != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToCharacter);
        }
    }

    void ChangeSize()
    {
        Vector3 newSize = sizeOptions[Random.Range(0, sizeOptions.Length)];
        transform.localScale = newSize;
    }

    void TeleportInFrontOfPlayer()
    {
        Vector3 directionToMove = character.forward.normalized;
        Vector3 targetPosition = character.position + directionToMove * teleportDistance;

        targetPosition.y = transform.position.y;

        if (flashRed)
        {
            StartCoroutine(FlashRed());
        }

        transform.position = targetPosition;
    }

    System.Collections.IEnumerator SmoothReturn(Vector3 targetPosition, Vector3 targetSize)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f || Vector3.Distance(transform.localScale, targetSize) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, returnSpeed * Time.deltaTime);

            transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, returnSpeed * Time.deltaTime);

            yield return null; 
        }


        transform.position = targetPosition;
        transform.localScale = targetSize;
    }

    System.Collections.IEnumerator FlashRed()
    {

        Color originalColor = cubeRenderer.material.color;


        cubeRenderer.material.color = Color.red;


        cubeHP--;
        Debug.Log("Cube HP: " + cubeHP);


        yield return new WaitForSeconds(0.5f);


        cubeRenderer.material.color = originalColor;

        flashRed = false;


        if (cubeHP <= 0 && !isDestroyed)
        {
            Debug.Log("Cube HP is 0! Teleporting...");
            isDestroyed = true;
            StartCoroutine(TeleportToHiddenAndReturn());
        }
    }

    System.Collections.IEnumerator TeleportToHiddenAndReturn()
    {

        transform.position = new Vector3(100, 100, 100);


        yield return new WaitForSeconds(60f);

        cubeHP = 20;
        Debug.Log("Cube returned to original position with full HP.");
        yield return SmoothReturn(originalPosition, originalSize);

        isDestroyed = false;
    }
}

