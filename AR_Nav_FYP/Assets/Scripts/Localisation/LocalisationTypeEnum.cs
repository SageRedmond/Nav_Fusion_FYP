using System;
using System.ComponentModel;

/// <summary>
/// What and how rooms should be activated and deactivated based on beacon behavior
/// </summary>
public enum LocalisationType
{
    /// <summary> Only the <b>room</b> associated to the closest beacon is active </summary>
    BeaconRoomBased,
    /// <summary> Only the <b>floor</b> associated to the closest beacon is active </summary>
    FloorBased,
    /// <summary> The entire map is active, regardless of the closest beacon </summary>
    MapBased
}

public static class LocalisationState
{
    public static LocalisationType State { get; private set; } = LocalisationType.BeaconRoomBased;

    public static void SetState(LocalisationType newState) => State = newState;
}