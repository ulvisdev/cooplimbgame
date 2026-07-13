using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public int TargetPlayerCount { get; private set; }

    private readonly List<LobbyPlayer> players = new();
    private readonly List<LimbAssignment> assignments = new();

    public IReadOnlyList<LobbyPlayer> Players => players;
    public IReadOnlyList<LimbAssignment> Assignments => assignments;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void BeginLobby(int playerCount)
    {
        TargetPlayerCount = Mathf.Clamp(playerCount, 2, 4);
        assignments.Clear();

        // Remove players from a previous setup attempt.
        for (int i = players.Count - 1; i >= 0; i--)
        {
            if (players[i] != null)
                Destroy(players[i].gameObject);
        }

        players.Clear();
    }

    public void RegisterPlayer(LobbyPlayer player)
    {
        if (player != null && !players.Contains(player))
            players.Add(player);
    }

    public void UnregisterPlayer(LobbyPlayer player)
    {
        players.Remove(player);
        assignments.RemoveAll(assignment => assignment.player == player);
    }

    public void ClearAssignments()
    {
        assignments.Clear();
    }

    public bool TryAssignLimb(
        LimbId limb,
        LobbyPlayer player,
        ControlSide controlSide)
    {
        if (player == null || !players.Contains(player))
            return false;

        // Remove the previous assignment for this limb.
        assignments.RemoveAll(assignment => assignment.limb == limb);

        // The same side of one controller cannot operate two limbs.
        assignments.RemoveAll(assignment =>
            assignment.player == player &&
            assignment.controlSide == controlSide);

        assignments.Add(new LimbAssignment
        {
            limb = limb,
            player = player,
            controlSide = controlSide
        });

        return true;
    }

    public bool IsSetupValid()
    {
        if (players.Count != TargetPlayerCount)
            return false;

        if (assignments.Count != 4)
            return false;

        if (assignments.Select(a => a.limb).Distinct().Count() != 4)
            return false;

        int[] limbCounts = players
            .Select(player => assignments.Count(a => a.player == player))
            .OrderBy(count => count)
            .ToArray();

        return TargetPlayerCount switch
        {
            2 => limbCounts.SequenceEqual(new[] { 2, 2 }),
            3 => limbCounts.SequenceEqual(new[] { 1, 1, 2 }),
            4 => limbCounts.SequenceEqual(new[] { 1, 1, 1, 1 }),
            _ => false
        };
    }
}