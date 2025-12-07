using System;

public enum TriggeredEvent
{
    ButtonPressed,
    DestinationSelected
}

static class TriggeredEventMethods
{
    public static String GetName(this TriggeredEvent eventName)
    {
        switch (eventName)
        {
            case TriggeredEvent.ButtonPressed:
                return "Button Pressed";
            case TriggeredEvent.DestinationSelected:
                return "Destination Selected";
            default:
                return "";
        }
    }
}