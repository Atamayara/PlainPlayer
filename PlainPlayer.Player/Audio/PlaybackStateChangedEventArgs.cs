namespace PlainPlayer.Player.Audio;

public class PlayingStateChangedEventArgs(PlayingState newState, PlayingReason reason, string? message) : EventArgs
{

    public PlayingState NewState { get; } = newState;
    public PlayingReason Reason { get; } = reason;
    public string? Message { get; } = message;
}

public enum PlayingReason
{
    UserAction,
    SystemAction,
    Finished,
    Error
}

public enum PlayingState
{
    Playing,
    Paused,
    Stopped,
    RequestPrevious
}