using System.Collections.Generic;

namespace EFC.SamuraiApp.Domain
{
    public class Samurai
    {
        public Samurai()
        {
            Quotes = new List<Quote>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Quote> Quotes { get; set; }
        //public int BattleId { get; set; }
        public IEnumerable<SamuraiBattle> SamuraiBattles { get; set; }
        public SecretIdentity SecretIdentity { get; set; }
    }
}
