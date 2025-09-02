using Historia2092.Core.Interfaces;
using Historia2092.Core.Models;

namespace Historia2092.Services
{
    public class PlayerSaveService : IPlayerSaveService
    {
        private PlayerSave _playerSave;

        public PlayerSave PlayerSave => _playerSave;

        public PlayerSaveService()
        {
            _playerSave = new PlayerSave();
            LoadSave();
        }

        public void LoadSave()
        {
            _playerSave = PlayerSave.LoadSave();
        }

        public void SaveGame()
        {
            _playerSave.SaveGame();
        }

        public void UpdateCharacterName(string name)
        {
            _playerSave.Character.Name = name;
            SaveGame();
        }

        public void UpdateAttribute(string attributeName, int value)
        {
            switch (attributeName.ToLower())
            {
                case "saude":
                    _playerSave.Character.Attributes.Saude = value;
                    break;
                case "psicologia":
                    _playerSave.Character.Attributes.Psicologia = value;
                    break;
                case "forca":
                    _playerSave.Character.Attributes.Forca = value;
                    break;
                case "inteligencia":
                    _playerSave.Character.Attributes.Inteligencia = value;
                    break;
            }
            SaveGame();
        }
    }
}