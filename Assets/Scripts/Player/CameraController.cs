using System;
using Controllers;
using State_Machine;
using State_Machine.GameStates;
using Unity.VisualScripting;
using UnityEngine;
using State = State_Machine.State;

namespace Player
{
    public class CameraController : MonoBehaviour, IStateAware
    {

        public Transform player;
        public float cameraSpeed;

        public float yOffset; // moves the camera lower or higher
        public float xOffset; // change how mutch the camera lags behind
        public float zOffset; // how close or far away from player the camera is

        private int currentLane;
        private float[] _lanePositions;
        
        public bool IsInitialized { get; private set; }

        public void Initialize(Component sender, object lanePositions)
        {
            if (sender is not ConveyorBelt || lanePositions is not float[])
                return;
            
            _lanePositions = (float[])lanePositions;
            
            // find middle position
            float middle = _lanePositions[^1];
            middle /= 2f;

            // move camera transform x to middle position
            Vector3 position = transform.position;
            position.x = middle;
            transform.position = position;
            
            Debug.Log("Initialized Camera Controller");
            IsInitialized = true;
        }

        public void Update()
        {
            if (!IsInitialized)
                return;
            
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
            int nearestIndex = 0;
            float minDistance = Mathf.Abs(playerX - _lanePositions[0]);

            for (int i = 1; i < _lanePositions.Length; i++)
            {
                float distance = Mathf.Abs(playerX - _lanePositions[i]);
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
            float middle = _lanePositions[^1];
            middle /= 2f;

            float laneX = _lanePositions[laneIndex];

            // calculate how far the player is from the middle
            float distanceFromMid = Mathf.Abs(playerX - middle);

            // interpolate the camera position between the middle and current lane
            float targetX = Mathf.Lerp(middle, laneX, distanceFromMid);
            return targetX;
        }

        public void OnEnterState(State state)
        {
            if (state.GetType() == typeof(PlayingState))
            {
                enabled = true;
            }
        }

        public void OnExitState(State state)
        {
            if (state.GetType() == typeof(PlayingState))
            {
                enabled = false;
            }
            
        }
    }
}
