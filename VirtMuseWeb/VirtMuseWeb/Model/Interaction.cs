using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{
    /// <summary>
    /// Describes a possibe interaction with an  exhibit
    /// </summary>
    public class InteractionInfo
    {
        /// <summary>
        /// the name of the class, used client side
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// in detail description of what that description does
        /// </summary>
        public string Description { get; set; }

        public override string ToString()
        {
            return Name + ": " + Description;
        }
    }

    /// <summary>
    /// container class for all possible interactions
    /// </summary>
    public class Interactions
    {
        public InteractionInfo[] PossibleInteractions { get; set; }
        public override string ToString()
        {
            string st = "";

            foreach (InteractionInfo inf in PossibleInteractions)
                st += inf.ToString() + "\n";

            return st;
        }
    }
}
