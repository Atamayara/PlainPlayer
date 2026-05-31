using ManagedBass;
using PlainPlayer.Player.Exceptions;

namespace PlainPlayer.Player.Audio;

public class AudioPlayer
{
    public event EventHandler<PlayingStateChangedEventArgs>? StateChanged;
    private int stream = 0;

    public AudioPlayer()
    {
        if (!Bass.Init())
            throw new InitializationFailedException($"プレイヤーの初期化に失敗しました: {Bass.LastError}");

    }

    public void Load(string path)
    {
        if (stream != 0) Stop();
        stream = Bass.CreateStream(path);
        if (stream == 0)
        {
            throw new StreamCreateFailedException($"ストリームの作成に失敗しました: {Bass.LastError}");
        }
    }
    public void PlayOrPause()
    {
        if (stream == 0)
        {
            OnStateChanged(PlayingState.Stopped, PlayingReason.Error, "再生ストリームがありません");
            return;
        }
        switch (Bass.ChannelIsActive(stream))
        {
            case PlaybackState.Playing:
            case PlaybackState.Stalled:
                Pause();
                break;

            default:
                Play();
                break;
        }
    }

    public void Play()
    {
        if (stream == 0)
        {
            OnStateChanged(PlayingState.Stopped, PlayingReason.Error, "再生ストリームがありません");
            Bass.StreamFree(stream);
            return;
        }
        Bass.ChannelPlay(stream);

        OnStateChanged(PlayingState.Playing, PlayingReason.UserAction);
    }

    public void Pause()
    {
        if (stream == 0)
        {
            OnStateChanged(PlayingState.Stopped, PlayingReason.Error, "再生ストリームがありません");
            return;
        }
        Bass.ChannelPause(stream);
        OnStateChanged(PlayingState.Paused, PlayingReason.UserAction);
    }

    public void Stop(bool isSystemAction = false)
    {
        if (stream == 0)
        {
            OnStateChanged(PlayingState.Stopped, PlayingReason.Error, "再生ストリームがありません");
            return;
        }
        Bass.ChannelStop(stream);
        Bass.StreamFree(stream);
        OnStateChanged(PlayingState.Stopped, isSystemAction ? PlayingReason.SystemAction : PlayingReason.UserAction);
    }

    public void Skip()
    {
        if (stream == 0)
        {
            OnStateChanged(PlayingState.Stopped, PlayingReason.Error, "再生ストリームがありません");
            return;
        }
        Stop(true);
        OnStateChanged(PlayingState.Stopped, PlayingReason.Finished);
    }

    public void Prev()
    {
        if (stream == 0)
        {
            OnStateChanged(PlayingState.Stopped, PlayingReason.Error, "再生ストリームがありません");
            return;
        }
        if (Bass.ChannelBytes2Seconds(stream, Bass.ChannelGetPosition(stream)) < 3)
        {
            OnStateChanged(PlayingState.RequestPrevious, PlayingReason.SystemAction);
        }
        Bass.ChannelSetPosition(stream, 1);
        Play();
    }

    private void OnStateChanged(PlayingState newState, PlayingReason reason, string? message = null)
    {
        StateChanged?.Invoke(this, new PlayingStateChangedEventArgs(newState, reason, message));
    }

}