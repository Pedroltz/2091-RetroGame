using Historia2092.Core.Models;

namespace Historia2092.Core.Interfaces
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