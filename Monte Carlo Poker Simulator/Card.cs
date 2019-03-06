using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarlo
{
    class Card
    {
        #region fields
        private char suit;
        private int value;
        #endregion

        #region properties
        public char Suit { get => suit; set => suit = value; }
        public int Value { get => value; set => this.value = value; }
        #endregion

        #region constructor

        public Card(int value, char suit)
        {
            this.value = value;
            this.suit = suit;
        }

        #endregion

        #region methods
        public bool Equals(Card cardToCompare)
        {
            bool sameSuit = this.Suit.Equals(cardToCompare.Suit);
            bool sameValue = this.Value == cardToCompare.Value;

            if (sameSuit & sameValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
