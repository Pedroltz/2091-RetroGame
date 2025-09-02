namespace Historia2092.Core.Models
{
    public class Attributes
    {
        public int Saude { get; set; } = 50;           // Pontos de vida
        public int Psicologia { get; set; } = 50;      // Pontos de Sanidade  
        public int Forca { get; set; } = 50;           // Pontos de Força
        public int Inteligencia { get; set; } = 50;    // Capacidade de Hackear
        public int Conversacao { get; set; } = 50;     // Habilidade de Conversação
        
        public int MaxSaude { get; set; } = 100;
        public int MaxPsicologia { get; set; } = 100;
        public int MaxForca { get; set; } = 100;
        public int MaxInteligencia { get; set; } = 100;
        public int MaxConversacao { get; set; } = 100;
        
        public int PointsToDistribute { get; set; } = 50; // Pontos para distribuir
    }
}