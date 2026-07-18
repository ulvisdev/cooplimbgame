using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSetupManager : MonoBehaviour
{
    [Header("Multiplayer")]
    [SerializeField] private PlayerInputManager playerInputManager;

    [Header("General UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button startButton;

    [Header("Limb Owner Dropdowns")]
    [Tooltip("Order: Left Arm, Right Arm, Left Leg, Right Leg")]
    [SerializeField] private TMP_Dropdown[] ownerDropdowns;

    [Header("Control Side Dropdowns")]
    [Tooltip("Order: Left Arm, Right Arm, Left Leg, Right Leg")]
    [SerializeField] private TMP_Dropdown[] sideDropdowns;

    private bool rebuildingDropdowns;

    private void Awake()
    {
        playerInputManager.DisableJoining();
    }

    private void Start()
    {
        SetupSideDropdowns();
        RefreshPlayerDropdowns();
        RefreshUI();
    }

    public void SelectPlayerCount(int playerCount)
    {
        playerInputManager.DisableJoining();

        GameSession.Instance.BeginLobby(playerCount);

        // maxPlayerCount is set to 4 in the Inspector.
        // We manually stop joining at the selected number.
        playerInputManager.EnableJoining();

        RefreshPlayerDropdowns();
        RefreshUI();
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        LobbyPlayer lobbyPlayer =
            playerInput.GetComponent<LobbyPlayer>();

        GameSession.Instance.RegisterPlayer(lobbyPlayer);

        if (GameSession.Instance.Players.Count >=
            GameSession.Instance.TargetPlayerCount)
        {
            playerInputManager.DisableJoining();
        }

        RefreshPlayerDropdowns();
        RefreshUI();
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        LobbyPlayer lobbyPlayer =
            playerInput.GetComponent<LobbyPlayer>();

        GameSession.Instance.UnregisterPlayer(lobbyPlayer);

        if (GameSession.Instance.TargetPlayerCount > 0 &&
            GameSession.Instance.Players.Count <
            GameSession.Instance.TargetPlayerCount)
        {
            playerInputManager.EnableJoining();
        }

        RefreshPlayerDropdowns();
        RefreshUI();
    }

    public void OnAssignmentsChanged(int ignoredValue)
    {
        if (rebuildingDropdowns)
            return;

        RebuildAssignments();
        RefreshUI();
    }

    public void StartGame()
    {
        if (!GameSession.Instance.IsSetupValid())
        {
            statusText.text =
                "The player and limb setup is not valid.";

            return;
        }

        SceneManager.LoadSceneAsync("Game");
    }

    private void SetupSideDropdowns()
    {
        rebuildingDropdowns = true;

        List<string> sideOptions = new()
        {
            "Left controls",
            "Right controls"
        };

        foreach (TMP_Dropdown dropdown in sideDropdowns)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(sideOptions);
            dropdown.SetValueWithoutNotify(0);
            dropdown.RefreshShownValue();
        }

        rebuildingDropdowns = false;
    }

    private void RefreshPlayerDropdowns()
    {
        rebuildingDropdowns = true;

        List<string> playerOptions = new()
        {
            "Unassigned"
        };

        for (int i = 0;
             i < GameSession.Instance.Players.Count;
             i++)
        {
            playerOptions.Add($"Player {i + 1}");
        }

        foreach (TMP_Dropdown dropdown in ownerDropdowns)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(playerOptions);
            dropdown.SetValueWithoutNotify(0);
            dropdown.RefreshShownValue();
        }

        GameSession.Instance.ClearAssignments();

        rebuildingDropdowns = false;
    }

    private void RebuildAssignments()
    {
        GameSession.Instance.ClearAssignments();

        for (int limbIndex = 0;
             limbIndex < ownerDropdowns.Length;
             limbIndex++)
        {
            // Dropdown value 0 means Unassigned.
            int playerListIndex =
                ownerDropdowns[limbIndex].value - 1;

            if (playerListIndex < 0 ||
                playerListIndex >=
                GameSession.Instance.Players.Count)
            {
                continue;
            }

            LobbyPlayer player =
                GameSession.Instance.Players[playerListIndex];

            ControlSide side =
                (ControlSide)sideDropdowns[limbIndex].value;

            GameSession.Instance.TryAssignLimb(
                (LimbId)limbIndex,
                player,
                side);
        }
    }

    private void RefreshUI()
    {
        int target = GameSession.Instance.TargetPlayerCount;
        int joined = GameSession.Instance.Players.Count;

        if (target == 0)
        {
            statusText.text =
                "Choose the number of players.";
        }
        else if (joined < target)
        {
            statusText.text =
                $"Players joined: {joined}/{target}. " +
                "Press A on your controller to join.";
        }
        else if (!GameSession.Instance.IsSetupValid())
        {
            statusText.text =
                $"Players joined: {joined}/{target}. " +
                "Assign every limb correctly.";
        }
        else
        {
            statusText.text =
                "Everyone is ready!";
        }

        startButton.interactable =
            GameSession.Instance.IsSetupValid();
    }
}