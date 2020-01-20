using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless1.Models
{
    public class Pokemon
    {
        public int Id { get; set; }

        public string PokemonSpecies { get; set; }

        public string Nickname { get; set; }

        public int PokedexId { get; set; }

        public string PictureUrl { get; set; }

        public int KeyCaught { get; set; }

        public DateTime DateCaught { get; set; }
    }
}
