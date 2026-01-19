using System;

public enum TriggeredEvent
{
    ButtonPressed,
    DestinationReached,
    ImmersalInitialised,
    FirstSuccessfulLocalization,
    AtWaypoint,
    LeavingWaypoint
}

static class TriggeredEventMethods
{
    public static String GetName(this TriggeredEvent eventName)
    {
        switch (eventName)
        {
            case TriggeredEvent.ButtonPressed:
                return "Button Pressed";
            case TriggeredEvent.DestinationReached:
                return "Destination Reached";
            case TriggeredEvent.ImmersalInitialised:
                return "[Immersal] SDK Initialised";
            case TriggeredEvent.FirstSuccessfulLocalization:
                return "[Immersal] First Successful Localization";
            case TriggeredEvent.AtWaypoint:
                return "At Waypoint";
            case TriggeredEvent.LeavingWaypoint:
                return "Leaving Waypoint";
            default:
                return "";
        }
    }
}