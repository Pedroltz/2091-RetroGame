namespace Historia2092.Core.Interfaces
{
    public interface IMusicService
    {
        void StartTitleMusic();
        void StopMusic();
        bool IsMusicPlaying();
    }
}