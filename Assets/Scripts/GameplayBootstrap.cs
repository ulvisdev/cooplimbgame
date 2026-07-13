using System.Collections.Generic;
using UnityEngine;

public class GameplayBootstrap : MonoBehaviour
{
    [SerializeField] private LimbInputSource leftArm;
    [SerializeField] private LimbInputSource rightArm;
    [SerializeField] private LimbInputSource leftLeg;
    [SerializeField] private LimbInputSource rightLeg;

    private void Start()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogError(
                "No GameSession exists. Start from PlayerSetup.");

            return;
        }

        Dictionary<LimbId, LimbInputSource> limbSources = new()
        {
            { LimbId.LeftArm, leftArm },
            { LimbId.RightArm, rightArm },
            { LimbId.LeftLeg, leftLeg },
            { LimbId.RightLeg, rightLeg }
        };

        foreach (LimbAssignment assignment
                 in GameSession.Instance.Assignments)
        {
            limbSources[assignment.limb].Configure(
                assignment.player.Input,
                assignment.controlSide);
        }
    }
}