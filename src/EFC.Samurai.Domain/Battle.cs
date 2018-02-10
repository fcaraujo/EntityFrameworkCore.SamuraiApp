using System;
using System.Collections.Generic;

namespace EFC.SamuraiApp.Domain
{
    public class Battle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public IEnumerable<Samurai> Samurais { get; set; }
        public IEnumerable<SamuraiBattle> SamuraiBattles { get; set; }
    }
}
