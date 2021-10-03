using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraTrackPlayers : MonoBehaviour
{
    public float maxSize = 55;
    public float minSize = 15;

    public float sizeChangeRate = 3;

    public float positionChangeRate = 2;

    public float extraSize = 5;


    public GameObject[] players
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("player");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float z = transform.position.z;
        Vector2 targetPosition = getTargetPosition();

        Vector2 lerped = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * positionChangeRate);

        if ((lerped - targetPosition).sqrMagnitude < 0.1)
        {
            lerped = targetPosition;
        }

        transform.position = new Vector3(lerped.x, lerped.y, z);


        float targetSize = getTargetSize();

        float lerpedSize = Mathf.Lerp(Camera.main.orthographicSize, targetSize, Time.deltaTime * sizeChangeRate);

        if (Mathf.Abs(targetSize - lerpedSize) < 0.1f)
        {
            lerpedSize = targetSize;
        }

        Camera.main.orthographicSize = lerpedSize;
    }

    Vector2 getTargetPosition()
    {
        var _players = players;
        if (_players == null || _players.Length == 0)
        {
            return Vector2.zero;
        }

        Vector2 averagePos = Vector2.zero;

        foreach (GameObject player in _players)
        {
            averagePos += new Vector2(player.transform.position.x, player.transform.position.y);
        }

        averagePos = averagePos / (_players.Length > 0 ? _players.Length : 1);

        return averagePos;
    }

    float getTargetSize()
    {
        var _players = players;
        if (_players == null || _players.Length == 0)
        {
            return maxSize;
        }

        if (_players.Length == 1)
        {
            return minSize;
        }

        float maxDistance = 0;

        for (int i = 0; i < _players.Length; ++i)
        {
            for (int j = 0; j < _players.Length; ++j)
            {
                if (i == j)
                {
                    continue;
                }

                float distance = (_players[i].transform.position - _players[j].transform.position).magnitude;

                maxDistance = Mathf.Max(distance, maxDistance);
            }
        }

        float sizeFromDistance = maxDistance / 2.5f + extraSize;

        return Mathf.Clamp(sizeFromDistance, minSize, maxSize);
    }

}
