using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowPlayer : MonoBehaviour
{
    float targetX;
    float targetY;
    public Transform player;
    public bool hasMultiplePlayers;
    Vector3 targetPosition;
    Transform newPlayer;
    int numPlayers = 0;
    public float offset;
    public float cameraMoveSpeed;

    public void OnPlayerJoined(PlayerInput input)
    {
        numPlayers++;
        Debug.Log("New player joined: " + input.name); // 👈 Add this
        if (numPlayers > 1)
            hasMultiplePlayers = true;
        newPlayer = input.transform;
    }

    private void Update()
    {
        if (hasMultiplePlayers)
            GetComponent<Camera>().orthographicSize = (newPlayer.position - player.position).magnitude + offset;

        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, 5.2f, 12f);
    }

    private void LateUpdate()
    {
        if (!hasMultiplePlayers)
        {
            targetPosition = player.position;

        }
        else
        {
            targetPosition = (newPlayer.position + player.position) / numPlayers;
        }

        targetX = Mathf.Lerp(targetX, targetPosition.x, Time.deltaTime * cameraMoveSpeed);
        targetY = Mathf.Lerp(targetY, targetPosition.y, Time.deltaTime * cameraMoveSpeed);
        transform.position = new Vector3(targetX, targetY, -10f);
    }
}
