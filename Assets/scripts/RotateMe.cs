using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotateMe : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject blackHole;

    public float rotationSpeed = 6;


    public bool isRotationDisabled = false;

    public Vector2 right
    {
        get
        {

            return -new Vector2(transform.right.x, transform.right.y).normalized;
        }
    }

    public Vector2 up
    {
        get
        {
            return new Vector2(transform.up.x, transform.up.y).normalized;
        }
    }

    public Vector2 toBlackhole
    {
        get
        {

            Vector2 dir = (blackHole.transform.position - transform.position).normalized;

            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            float angle2 = Mathf.RoundToInt(angle / gameManager.rotationRegionSizeInDegrees) * gameManager.rotationRegionSizeInDegrees;

            float x = Mathf.Cos(angle2 * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle2 * Mathf.Deg2Rad);

            return new Vector2(x, y).normalized;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        blackHole = GameObject.FindWithTag("blackHole");
        gameManager = GameObject.FindWithTag("gameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isRotationDisabled)
        {
            Vector3 newUp = -toBlackhole;
            Vector3 up = new Vector3(transform.up.x, transform.up.y, 0);

            Quaternion deltaRotation = Quaternion.FromToRotation(up, newUp);

            if (Application.isPlaying)
            {

                transform.rotation = Quaternion.Slerp(transform.rotation, deltaRotation * transform.rotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                transform.rotation = deltaRotation * transform.rotation;
            }
        }
    }
}
