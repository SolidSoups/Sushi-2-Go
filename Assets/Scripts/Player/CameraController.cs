using Prototype.Scripts;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour, IControllable
{

    public Transform player;
    public float cameraSpeed;

    public float yOffset; // moves the camera lower or higher
    public float xOffset; // change how mutch the camera lags behind
    public float zOffset; // how close or far away from player the camera is

    private int currentLane;


    public void Initialize()
    {

        float[] lanePositions = WorldMover.Instance.GetConveyorXPositions();
        // find middle position
        float middle = lanePositions[lanePositions.Length-1];
        middle /= 2f;

        // move camera transform x to middle position
        Vector3 position = transform.position;
        position.x = middle;
        transform.position = position;
    }

    public void DoUpdate()
    {

    }

    public void DoFixedUpdate()
    {
        
    }

    public void Update()
    {
        // find current lane based on player x position
        currentLane = GetNearestLane(player.position.x);

        // calculate target position
        float targetX = CalculateTargetX(player.transform.position.x, currentLane);

        if (currentLane == 1)
        {
            SetTarget(0, targetX);
        }
        else if (currentLane <= 0)
        {

            SetTarget(xOffset, targetX);

        }
        else if(currentLane > 1)
        {
            float negativeXOffset = xOffset * -1;
            SetTarget(negativeXOffset, targetX);
        }

    }

    private void SetTarget(float xOffset, float targetX)
    {
        Vector3 targetPosition = new Vector3(targetX + xOffset, player.position.y + yOffset, zOffset);
        // move camera
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
    }

    // calculate the nearest lane index based on player position
    private int GetNearestLane(float playerX)
    {
        float[] lanePositions = WorldMover.Instance.GetConveyorXPositions();
        int nearestIndex = 0;
        float minDistance = Mathf.Abs(playerX - lanePositions[0]);

        for (int i = 1; i < lanePositions.Length; i++)
        {
            float distance = Mathf.Abs(playerX - lanePositions[i]);
            if(distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    // calculate the camera x target position
    private float CalculateTargetX(float playerX, int laneIndex)
    {
        float[] lanePositions = WorldMover.Instance.GetConveyorXPositions();
        float middle = lanePositions[lanePositions.Length - 1];
        middle /= 2f;

        float laneX = lanePositions[laneIndex];

        // calculate how far the player is from the middle
        float distanceFromMid = Mathf.Abs(playerX - middle);

        // interpolate the camera position between the middle and current lane
        float targetX = Mathf.Lerp(middle, laneX, distanceFromMid);
        return targetX;
    }

}
