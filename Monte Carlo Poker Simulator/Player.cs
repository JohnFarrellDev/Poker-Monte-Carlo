using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarlo
{
    class Player
    {
        #region fields
        private List<Card> playersHand = new List<Card>(7);
        private int bestHand;
        private int[] highestCards = new int[5]; //highest card in 0th index, second highest card in 1st index and so on

        //Dictionary to convery the integer best hand representation to it's string name
        private static Dictionary<int, string> bestHandIntToString = new Dictionary<int, string>()
        {
            {10, "Royal Flush" },
            {9, "Straight Flush" },
            {8, "Four of a Kind" },
            {7, "Full House" },
            {6, "Flush" },
            {5, "Straight" },
            {4, "Three of a Kind" },
            {3, "Two Pair" },
            {2, "Pair" },
            {1, "High Card" }
        };
        #endregion

        #region properties
        public List<Card> PlayersHand { get => playersHand; set => playersHand = value; }
        public int[] HighestCards { get => highestCards; set => highestCards = value; }
        public int BestHand { get => bestHand; set => bestHand = value; }
        public Dictionary<int, string> BestHandIntToString { get => bestHandIntToString; set => bestHandIntToString = value; }
        #endregion

        #region constructor
        public Player(Card firstCard, Card secondCard) //now will take two Card objects
        {
            PlayersHand.Add(firstCard);
            PlayersHand.Add(secondCard);
        }

        public Player(List<Card> cardsPlayerHas) //create a player object from a list of cards
        {
            foreach(Card card in cardsPlayerHas)
            {
                playersHand.Add(card);
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Takes cardToAdd and ands the Card object to the player's playersHand List<Card>
        /// </summary>
        /// <param name="cardToAdd"></param>
        public void Add(Card cardToAdd)
        {
            PlayersHand.Add(cardToAdd);
        }

        /// <summary>
        /// Method to adjust the integer value of bestHand (int) and to also store the highest card value in order in the highestCards (int[])
        /// 10 in bestHand represents a royal flush
        /// 9 in bestHand represents a straight flush
        /// 8 in bestHand represents a four of a kind
        /// 7 in bestHand represents a full house
        /// 6 in bestHand represents a flush
        /// 5 in bestHand represents a straight
        /// 4 in bestHand represents a three of a kind
        /// 3 in bestHand represents a two pair
        /// 2 in bestHand represents a pair
        /// 1 in bestHand represents a high card
        /// </summary>
        /// 
        internal void GetBestHand(List<Card> playersHand)
        {
            CheckForRoyalFlush(playersHand);
            if (BestHand == 10) { return; } //we have a royal flush so no need to check for any other potential hands as they are all worse!

            CheckForStraightFlush(playersHand);
            if (BestHand == 9) { return; } //we have a straight flush so no need to check for any other potential hands as they are all worse!

            CheckForFourOfAKind(playersHand);
            if (BestHand == 8) { return; } //we have a four of a kind so no need to check for any other potential hands as they are all worse!

            CheckForFullHouse(playersHand);
            if (BestHand == 7) { return; } //we have a full house so no need to check for any other potential hands as they are all worse!

            CheckForFlush(playersHand);
            if (BestHand == 6) { return; } //we have a flush so no need to check for any other potential hands as they are all worse!

            CheckForStraight(playersHand);
            if (BestHand == 5) { return; } //we have a straight so no need to check for any other potential hands as they are all worse!

            CheckForThreeOfAKind(playersHand);
            if (BestHand == 4) { return; } //we have a three of a kind so no need to check for any other potential hands as they are all worse!

            CheckForTwoPair(playersHand);
            if (BestHand == 3) { return; } //we have a two pair so no need to check for any other potential hands as they are all worse!

            CheckForPair(playersHand);
            if (BestHand == 2) { return; } //we have a pair so no need to check for any other potential hands as they are all worse!

            CheckForHighCard(playersHand);
        }

        /// <summary>
        /// Checks if the hand contains a royal flush, A, K, Q, J, 10, all the same suit. 
        /// </summary>
        private void CheckForRoyalFlush(List<Card> playersHandCheckForRoyalFlush) //DONE - works
        {
            //hashset of all the requiredCardValues needed for a royal flush (also must all belong to same suit)
            HashSet<int> requiredCardValues = new HashSet<int>() { 10, 11, 12, 13, 14 };

            //count how many time each suit appears in the players hand
            Dictionary<char, int> cardSuitCount = CardSuitCount(playersHandCheckForRoyalFlush);

            //check if any suit appears more than four timea making a flush possible
            foreach (char suitValue in cardSuitCount.Keys)
            {
                if (cardSuitCount[suitValue] > 4)
                {
                    //check if the value of the cards in the flush make it possible for a royal flush
                    foreach(Card playersCard in playersHandCheckForRoyalFlush)
                    {
                        if(suitValue.Equals(playersCard.Suit))
                        {
                            if (requiredCardValues.Contains(playersCard.Value))
                            {
                                requiredCardValues.Remove(playersCard.Value);
                            }
                        }
                    }
                }
            }

            if (requiredCardValues.Count == 0)
            {
                //we have a royal flush
                BestHand = 10;
                highestCards[0] = 14;
                highestCards[1] = 13;
                highestCards[2] = 12;
                highestCards[3] = 11;
                highestCards[4] = 10;
            }
        }

        /// <summary>
        /// Checks if the hand contains a straight flush, Five cards in a sequence, all in the same suit. 
        /// </summary>
        private void CheckForStraightFlush(List<Card> playersHandCheckForStraightFlush) //DONE - works
        {
            Dictionary<char, int> cardSuitCount = CardSuitCount(playersHandCheckForStraightFlush);     

            bool haveAFlush = false;
            //check if any suit appears more than four timea making a flush possible
            foreach (int SuitCount in cardSuitCount.Values)
            {
                if (SuitCount > 4)
                {
                    haveAFlush = true;
                }

            }

            List<int> valuesOfFlushCard = new List<int>();
            if (haveAFlush)
            {
                foreach(Card playersCard in playersHandCheckForStraightFlush)
                {
                    if(cardSuitCount[playersCard.Suit] > 4)
                    {
                        valuesOfFlushCard.Add(playersCard.Value);
                        if(playersCard.Value == 14)
                        {
                            valuesOfFlushCard.Add(1);
                        }
                    }
                }
            }
            
            if (valuesOfFlushCard.Count > 4) //here we now check if it contains a straight
            {
                valuesOfFlushCard.Sort();

                int count = 1;
                for (int i = 0; i < valuesOfFlushCard.Count - 1; i++)
                {
                    if (valuesOfFlushCard[i] + 1 == valuesOfFlushCard[i + 1])
                    {
                        count++;
                    }
                    else
                    {
                        count = 1;
                    }

                    if (count > 4)
                    {
                        BestHand = 9;
                        HighestCards[0] = valuesOfFlushCard[i + 1];
                        HighestCards[1] = valuesOfFlushCard[i];
                        HighestCards[2] = valuesOfFlushCard[i - 1];
                        HighestCards[3] = valuesOfFlushCard[i - 2];
                        HighestCards[4] = valuesOfFlushCard[i - 3];
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the hand contains four of a kind, four cards with the same value.
        /// </summary>
        private void CheckForFourOfAKind(List<Card> checkPlayersHandForFourOfAKind) //DONE - works
        {
            //count how often each card value appears in our hand
            Dictionary<int, int> cardValueCount = CardValueCount(checkPlayersHandForFourOfAKind);
            bool haveFourOfAKind = false;

            //check if any of the card values appear four times meaning we have a four of a king in our hand
            foreach (int cardValue in cardValueCount.Values)
            {
                if (cardValue == 4)
                {
                    haveFourOfAKind = true;
                }
            }

            if (haveFourOfAKind)
            {
                BestHand = 8;
                foreach (int cardValue in cardValueCount.Keys)
                {
                    if(cardValueCount[cardValue] == 4)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            HighestCards[i] = cardValue;
                        }
                        //need to find 5th card that is highest card not in the four of a kind
                        HighestCards[4] = 0;
                        List<int> cardValues = ListOfValues(checkPlayersHandForFourOfAKind);
                        cardValues.Sort();
                        int startingPoint = cardValues.Count -1;

                        while (HighestCards[4] == 0)
                        {
                            if(cardValues[startingPoint] != highestCards[0])
                            {
                                highestCards[4] = cardValues[startingPoint];
                            }
                            else
                            {
                                startingPoint--;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the hand contains a full house, Three of a kind with a pair. 
        /// </summary>
        private void CheckForFullHouse(List<Card> checkPLayersHandforFullHouse) //DONE - works
        {
            //get a count of how often each value appears
            Dictionary<int, int> valueCount = CardValueCount(checkPLayersHandforFullHouse);
            bool haveFullHouse = false;

            foreach(int cardValue in valueCount.Keys)
            {
                if(valueCount[cardValue] == 3)
                {
                    foreach(int checkForPairOrTriple in valueCount.Keys)
                    {
                        if((valueCount[checkForPairOrTriple] == 2 || valueCount[checkForPairOrTriple] ==3) && (!checkForPairOrTriple.Equals(cardValue)))
                        {
                            haveFullHouse = true;
                            break;
                        }
                    }
                }
            }

            if (haveFullHouse)
            {
                BestHand = 7;
                List<int> trippleCardValues = new List<int>();
                foreach (int cardNumber in valueCount.Keys)
                {
                    if (valueCount[cardNumber] == 3)
                    {
                        trippleCardValues.Add(cardNumber);
                    }
                }
                trippleCardValues.Sort();
                trippleCardValues.Reverse();
                HighestCards[0] = trippleCardValues[0];
                HighestCards[1] = trippleCardValues[0];
                HighestCards[2] = trippleCardValues[0];

                List<int> cardValuesThatMakeAPairAndNotInTripple = new List<int>();
                foreach (int cardNumberNotTripple in valueCount.Keys)
                {
                    if((valueCount[cardNumberNotTripple] > 1) && (cardNumberNotTripple != HighestCards[0]))
                    {
                        cardValuesThatMakeAPairAndNotInTripple.Add(cardNumberNotTripple);
                    }
                }

                cardValuesThatMakeAPairAndNotInTripple.Sort();
                cardValuesThatMakeAPairAndNotInTripple.Reverse();

                HighestCards[3] = cardValuesThatMakeAPairAndNotInTripple[0];
                HighestCards[4] = cardValuesThatMakeAPairAndNotInTripple[0];
            }
        }

        /// <summary>
        /// Check if the hand contains a flush, five cards of the same suit.
        /// </summary>
        private void CheckForFlush(List<Card> checkPlayersHandForFlush) //DONE - works
        {
            Dictionary<char, int> suitCount = CardSuitCount(checkPlayersHandForFlush);
            bool flushPossible = false;

            foreach(int numberOfTimeSuitAppears in suitCount.Values)
            {
                if(numberOfTimeSuitAppears > 4)
                {
                    flushPossible = true;
                }
            }

            if (flushPossible)
            {
                BestHand = 6;
                List<int> flushCardsCardValue = new List<int>();
                //check which suit appears more than 4 times, add all cards of that suits value to a lost, sort it, add top 5 cards to HighCards
                foreach (char cardSuitCount in suitCount.Keys)
                {
                    if (suitCount[cardSuitCount] > 4)
                    {
                        foreach(Card playersCards in checkPlayersHandForFlush)
                        {
                            if (playersCards.Suit.Equals(cardSuitCount))
                            {
                                flushCardsCardValue.Add(playersCards.Value);
                            }
                        }
                    }
                }
                flushCardsCardValue.Sort();
                flushCardsCardValue.Reverse();

                for(int i = 0; i < 5; i++)
                {
                    HighestCards[i] = flushCardsCardValue[i];
                }
            }
        }

        /// <summary>
        /// Check if the hand contains a straight, five cards whoo's values are in an ordered seqeunce.
        /// </summary>
        private void CheckForStraight(List<Card> checkPLayersHandForStraight) //Done - works
        {
            List<int> allValues = new List<int>();
            foreach(int cardValue in CardValueCount(checkPLayersHandForStraight).Keys) //use dictionary keys to remove any duplicates otherwise the logic would break when doing the count addition
            {
                allValues.Add(cardValue); 
                if(cardValue == 14)
                {
                    allValues.Add(1);
                }
            }
            allValues.Sort();

            int count = 1;
            for (int i = 0; i < allValues.Count - 1; i++)
            {
                if (allValues[i] + 1 == allValues[i + 1])
                {
                    count++;
                }
                else
                {
                    count = 1;
                }

                if (count > 4)
                {
                    BestHand = 5;
                    HighestCards[0] = allValues[i + 1];
                    HighestCards[1] = allValues[i];
                    HighestCards[2] = allValues[i - 1];
                    HighestCards[3] = allValues[i - 2];
                    HighestCards[4] = allValues[i - 3];
                }
            }
        }

        /// <summary>
        /// checks for three of a kind, three cards with the same value.
        /// </summary>
        private void CheckForThreeOfAKind(List<Card> checkPlayersHandForThreeOfAKind)  //DONE - works
        {
            //get a count of how often each value appears
            Dictionary<int, int> valueCount = CardValueCount(checkPlayersHandForThreeOfAKind);
            bool haveThreeOfAKind = false;

            foreach (int countOfCardValue in valueCount.Values)
            {
                if(countOfCardValue ==  3)
                {
                    haveThreeOfAKind = true;
                }
            }

            if (haveThreeOfAKind)
            {
                BestHand = 4;
                foreach (int cardValue in valueCount.Keys)
                {
                    if(valueCount[cardValue] == 3)
                    {
                        HighestCards[0] = cardValue;
                        HighestCards[1] = cardValue;
                        HighestCards[2] = cardValue;

                        List<int> cardsInHandThatAreNotThreeOfAKind = new List<int>();

                        foreach (int cardNotInThreeOfAKind in valueCount.Keys)
                        {
                            if(cardNotInThreeOfAKind != HighestCards[0])
                            {
                                int cardNumberInt = cardNotInThreeOfAKind;
                                cardsInHandThatAreNotThreeOfAKind.Add(cardNumberInt);
                            }
                        }

                        cardsInHandThatAreNotThreeOfAKind.Sort();
                        cardsInHandThatAreNotThreeOfAKind.Reverse();
                        HighestCards[3] = cardsInHandThatAreNotThreeOfAKind[0];
                        HighestCards[4] = cardsInHandThatAreNotThreeOfAKind[1];
                    }
                }
            }
        }

        /// <summary>
        /// Checks for two pair, hand contains two pairs, a pair is two cards that have the same value.
        /// </summary>
        private void CheckForTwoPair(List<Card> checkPlayersHandForTwoPair)  //DONE - works
        {
            Dictionary<int, int> valueCount = CardValueCount(checkPlayersHandForTwoPair);
            bool haveTwoPair = false;
            int pairCount = 0;

            //find if there are more than one pair (possibly can be 3), take the two highest pairs, then find the highest card in the hand not part of the pair
            foreach (int countOfCardValue in valueCount.Values)
            {
                if(countOfCardValue == 2)
                {
                    pairCount++;
                }
            }

            if(pairCount > 1)
            {
                haveTwoPair = true;
            }

            if (haveTwoPair)
            {
                BestHand = 3;
                List<int> pairCardValues = new List<int>();

                foreach (int cardNumber in valueCount.Keys)
                {
                    
                    if (valueCount[cardNumber] == 2)
                    {
                        pairCardValues.Add(cardNumber);
                    }
                }

                pairCardValues.Sort();
                pairCardValues.Reverse();
                HighestCards[0] = pairCardValues[0];
                HighestCards[1] = pairCardValues[0];
                HighestCards[2] = pairCardValues[1];
                HighestCards[3] = pairCardValues[1];

                HighestCards[4] = 0;
                foreach(int cardNumberNotPairs in valueCount.Keys)
                {
                    if(cardNumberNotPairs != HighestCards[0] && cardNumberNotPairs != HighestCards[2])
                    {
                        if(cardNumberNotPairs > HighestCards[4])
                        {
                            HighestCards[4] = cardNumberNotPairs;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks for a pair, a pair is two cards with the same value.
        /// </summary>
        private void CheckForPair(List<Card> checkPlayersHandForPair)  //DONE - works
        {
            //get a count of how often each value appears
            Dictionary<int, int> valueCount = CardValueCount(checkPlayersHandForPair);
            bool havePair = false;

            foreach(int countOfCardValue in valueCount.Values)
            {
                if(countOfCardValue == 2)
                {
                    havePair = true;
                }
            }

            if (havePair)
            {
                BestHand = 2;
                foreach (int cardValueOfPair in valueCount.Keys)
                {
                    if (valueCount[cardValueOfPair] == 2)
                    {
                        HighestCards[0] = cardValueOfPair;
                        HighestCards[1] = cardValueOfPair;

                        List<int> cardsInHandThatAreNotPair = new List<int>();

                        foreach(int cardNumber in valueCount.Keys)
                        {
                            if(cardNumber != cardValueOfPair)
                            {
                                cardsInHandThatAreNotPair.Add(cardNumber);
                            }
                        }

                        cardsInHandThatAreNotPair.Sort();
                        cardsInHandThatAreNotPair.Reverse();

                        HighestCards[2] = cardsInHandThatAreNotPair[0];
                        HighestCards[3] = cardsInHandThatAreNotPair[1];
                        HighestCards[4] = cardsInHandThatAreNotPair[2];
                    }
                }
            }
        }

        /// <summary>
        /// If no other hands are possible then the player only has a high card, the weakest hand.
        /// </summary>
        private void CheckForHighCard(List<Card> checkPlayersHandForHighCard) //DONE - works
        {
            List<int> allValues = new List<int>();
            foreach (Card card in checkPlayersHandForHighCard)
            {
                allValues.Add(card.Value);
            }
            allValues.Sort();
            allValues.Reverse();

            BestHand = 1;
            for(int i = 0; i < 5; i++)
            {
                HighestCards[i] = allValues[i];
            }
        }

        /// <summary>
        /// Method to return a dictionary counting how often each card value appears in a player's hand
        /// </summary>
        /// <param name="playersCards"></param>
        /// <returns></returns>
        private Dictionary<int, int> CardValueCount(List<Card> playersCards)
        {
            Dictionary<int, int> valueCount = new Dictionary<int, int>();

            foreach (Card playersCard in playersCards)
            {
                if (valueCount.ContainsKey(playersCard.Value))
                {
                    valueCount[playersCard.Value]++;
                }
                else
                {
                    valueCount.Add(playersCard.Value, 1);
                }
            }
            return valueCount;
        }

        /// <summary>
        /// Method to return a dictionary counting how often each card suit appears in a player's hand
        /// </summary>
        /// <param name="playersCards"></param>
        /// <returns></returns>
        private Dictionary<char, int> CardSuitCount(List<Card> playersCards)
        {
            Dictionary<char, int> suitCount = new Dictionary<char, int>();

            foreach (Card playersCard in playersCards)
            {
                if (suitCount.ContainsKey(playersCard.Suit))
                {
                    suitCount[playersCard.Suit]++;
                }
                else
                {
                    suitCount.Add(playersCard.Suit, 1);
                }
            }
            return suitCount;
        }

        /// <summary>
        /// A method to quickly take a player's hand and return a list of all their card values represented as ints, handles convesion of face cards (jack, queen, king and ace) to representative int value
        /// </summary>
        /// <param name="playersCards"></param>
        /// <returns></returns>
        private List<int> ListOfValues(List<Card> playersCards)
        {
            List<int> listOfAllValues = new List<int>();
            foreach (Card playersCard in playersCards)
            {
                listOfAllValues.Add(playersCard.Value);
            }

            return listOfAllValues;
        }
        #endregion
    }
}
