using RetroGame2091.Core.Interfaces;
using RetroGame2091.Core.Models;

namespace RetroGame2091.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IPlayerSaveService _playerSaveService;

        public ChapterService(IPlayerSaveService playerSaveService)
        {
            _playerSaveService = playerSaveService;
        }

        public Chapter? LoadChapter(string id)
        {
            return Chapter.LoadChapter(id);
        }

        public void SaveChapter(Chapter chapter)
        {
            chapter.SaveChapter();
        }

        public void CreateExampleChapters()
        {
            var chapterStart = new Chapter
            {
                Id = "init_inicio",
                Title = "O Despertar em 2091",
                Text = new List<string>
                {
                    "Você desperta em um apartamento minúsculo no 47º andar de um arranha-céu decadente.",
                    "O ano é 2091, e a cidade se estende infinitamente através da janela embaçada.",
                    "Neons piscam através da névoa tóxica que cobre as ruas lá embaixo.",
                    "Seu nome é {name}, e hoje algo vai mudar sua vida para sempre.",
                    "Um envelope vermelho foi deslizado sob sua porta durante a noite..."
                },
                Options = new List<Option>
                {
                    new Option { Text = "Pegar o envelope e lê-lo", NextChapter = "init_envelope" },
                    new Option { Text = "Ignorar o envelope e sair do apartamento", NextChapter = "rua" },
                    new Option { Text = "Olhar pela janela primeiro", NextChapter = "init_janela" },
                }
            };

            var chapterEnvelope = new Chapter
            {
                Id = "init_envelope",
                Title = "A Mensagem Misteriosa",
                Text = new List<string>
                {
                    "Você abre o envelope com mãos trêmulas.",
                    "Dentro há uma mensagem holográfica que se ativa ao toque:",
                    "'Se você está lendo isto, sua vida está em perigo.'",
                    "'Encontre-me no Café Nexus, Setor 7, às 15:00.'",
                    "'Confie em ninguém. Destrua esta mensagem.'",
                    "A holografia se auto-destrói em chamas azuis."
                },
                Options = new List<Option>
                {
                    new Option { Text = "Ir ao Café Nexus imediatamente", NextChapter = "cafe" },
                    new Option { Text = "Investigar quem enviou a mensagem", NextChapter = "investigar" }
                }
            };

            var chapterWindow = new Chapter
            {
                Id = "init_janela",
                Title = "A Cidade Desperta",
                Text = new List<string>
                {
                    "Pela janela, você vê a metrópole cyberpunk em toda sua glória sombria.",
                    "Carros voadores cortam entre os prédios como insetos luminosos.",
                    "Hologramas gigantes anunciam produtos que você nunca poderá comprar.",
                    "No reflexo do vidro, você nota algo estranho...",
                    "Há pessoas de casacos escuros observando seu prédio lá embaixo."
                },
                NextChapter = "init_inicio"
            };

            SaveChapter(chapterStart);
            SaveChapter(chapterEnvelope);
            SaveChapter(chapterWindow);
        }
    }
}