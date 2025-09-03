using RetroGame2091.Core.Models;

namespace RetroGame2091.Core.Interfaces
{
    public interface IPlayerSaveService
    {
        PlayerSave PlayerSave { get; }
        void SaveGame();
        void LoadSave();
        void UpdateCharacterName(string name);
        void UpdateAttribute(string attributeName, int value);
    }
}