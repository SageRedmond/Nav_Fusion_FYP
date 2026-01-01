using System;

public enum TriggeredEvent
{
    ButtonPressed,
    DestinationReached,
    ImmersalInitialised,
    FirstSuccessfulLocalization
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
            default:
                return "";
        }
    }
}