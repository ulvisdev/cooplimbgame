using System;

public enum LimbId
{
    LeftArm,
    RightArm,
    LeftLeg,
    RightLeg
}

public enum ControlSide
{
    Left,
    Right
}

[Serializable]
public class LimbAssignment
{
    public LimbId limb;
    public LobbyPlayer player;
    public ControlSide controlSide;
}