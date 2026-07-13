using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class LobbyPlayer : MonoBehaviour
{
    public PlayerInput Input { get; private set; }

    public int PlayerIndex => Input.playerIndex;

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        DontDestroyOnLoad(gameObject);
    }
}