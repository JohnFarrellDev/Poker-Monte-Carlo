using System;
using System.Collections.Generic;

namespace MonteCarlo
{
    class Monte_Carlo
    {
        #region fields
        private static int numberOfSimulations = 100000;

        private static Player ourPlayer;

        private static Random random = new Random();  //the random object is utilised when having to take an unknown card from the deck

        private static char[] cardSuit = new char[] { 'C', 'H', 'S', 'D' };
        private static int[] cardNumber = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

        private static List<Card> deckOfCardsList = new List<Card>(52);

        private static int numberOfOpponents;
        private static int numberOfFoldedOpponents;

        private static List<Card> knownSharedCards = new List<Card>(5);

        private static int winCount;
        private static int lossCount;
        private static int drawCount;
        #endregion

        #region properties
        public static int NumberOfFoldedOpponents { get => numberOfFoldedOpponents; set => numberOfFoldedOpponents = value; }
        public static int NumberOfSimulations { get => numberOfSimulations; set => numberOfSimulations = value; }
        public static char[] CardSuit { get => cardSuit; set => cardSuit = value; }
        public static int[] CardNumber { get => cardNumber; set => cardNumber = value; }
        internal static Player OurPlayer { get => ourPlayer; set => ourPlayer = value; }
        public static int NumberOfOpponents { get => numberOfOpponents; set => numberOfOpponents = value; }
        internal static List<Card> DeckOfCardsList { get => deckOfCardsList; set => deckOfCardsList = value; }
        internal static List<Card> KnownSharedCards { get => knownSharedCards; set => knownSharedCards = value; }
        public static int WinCount { get => winCount; set => winCount = value; }
        public static int LossCount { get => lossCount; set => lossCount = value; }
        public static int DrawCount { get => drawCount; set => drawCount = value; }
        #endregion

        static void Main(string[] args)
        {
            GenerateCards(); //creates all 52 unique Card objects found in a standard pack of cards and adds the Card objects to the list<Card> deckOfCardList

            UserInput(); //Get user input for their player cards, current stage of the game, any known shared cards, number of opponents in total and how many opponents have folded

            MonteCarloSimulation(NumberOfSimulations, DeckOfCardsList, KnownSharedCards, OurPlayer, NumberOfOpponents, NumberOfFoldedOpponents);

            ReturnChanceOfWinning();

            Console.ReadLine();
        }

        /// <summary>
        /// Creates every possible card combination in a 52 pack of playing cards and adds them to the List<Card> listOfAllCards and HashSet<Card> hashsetOfAllCards
        /// </summary>
        private static void GenerateCards()
        {
            char[] cardSuit = new char[] { 'C', 'H', 'S', 'D' }; //C represents clubs, H represents hearts, S represents spades, D represents diamononds
            int[] cardNumber = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }; //Jack represented by 11, Queen reprsented by 12, King represented by 13, Ace represented by 14
            foreach (char cardsuit in CardSuit)
            {
                foreach(int cardnumber in CardNumber)
                {
                    Card card = new Card(cardnumber, cardsuit);
                    DeckOfCardsList.Add(card);
                }
            }
        }

        /// <summary>
        /// switch a lot of these userinput sections into their own methods for neatness 
        /// </summary>
        private static void UserInput()
        {
            GetPlayersHand();

            int numberOfSharedCards = GetNumberOfSharedCards(); 

            GetKnownSharedCards(numberOfSharedCards); 

            GetOpponentInfo();  //TODO - need to do validation on user inputs
        }

        private static void GetPlayersHand()
        {
            Card card1;
            Card card2;
            string[] userCards = new string[2];

            bool validCardsEntered = false;
            while (!validCardsEntered)
            {
                validCardsEntered = true;

                Console.WriteLine("Please enter your cards in the form AH (ace of hearts) 8D (8 of diamonds), card value followed by card suit, seperate cards with a space, Jack = J, Queen = Q, King = K, Ace = A, Diamonds = D, Hearts = H, Spades = S, Clubs = C");
                string userHand = Console.ReadLine();
                if (!CheckUserCardInputIsValid(2, userHand))
                {
                    validCardsEntered = false;
                }

                userCards = userHand.Split();
            }

            card1 = StringToCard(userCards[0]); 
            card2 = StringToCard(userCards[1]);

            //add the user's hand cards to the Player object ourPlayer
            OurPlayer = new Player(card1, card2);

            RemovedCardFromDeck(card1);
            RemovedCardFromDeck(card2);
        }

        private static int GetNumberOfSharedCards()
        {
            bool numberOfSharedCardsGiven = false;
            int numberOfSharedCards = 0;

            while (!numberOfSharedCardsGiven)
            {
                //ask player if we're pre flop, post flop, river or break, use number input to match game positon
                Console.WriteLine("Plese indicate how many cards are on the table for everyone, 0 if pre-flop, 3 if flop, 4 if turn or five if the river.");
                numberOfSharedCards = Convert.ToInt32(Console.ReadLine()); //requires error handling for correct format

                if (numberOfSharedCards == 0 || numberOfSharedCards == 3 || numberOfSharedCards == 4 || numberOfSharedCards == 5)
                {
                    numberOfSharedCardsGiven = true;
                }
            }

            return numberOfSharedCards;
        }

        private static void GetKnownSharedCards(int numberOfSharedCards) 
        {

            switch (numberOfSharedCards)
            {
                case 0:
                    break;
                case 3:
                    bool validCardsEnteredFlop = false;
                    while (!validCardsEnteredFlop)
                    {
                        validCardsEnteredFlop = true;

                        Console.WriteLine("Please enter the three shared cards in the form AH (ace of hearts) 8D (8 of diamonds), card value followed by card suit, seperate cards with a space");
                        string flopCards = Console.ReadLine();
                        if (!CheckUserCardInputIsValid(3, flopCards))
                        {
                            validCardsEnteredFlop = false;
                        }
                        else
                        {
                            AddSharedCards(flopCards);
                        }
                    }
                    break;
                case 4:
                    bool validCardsEnteredTurn = false;
                    while (!validCardsEnteredTurn)
                    {
                        validCardsEnteredTurn = true;

                        Console.WriteLine("Please enter the four shared cards in the form AH (ace of hearts) 8D (8 of diamonds), card value followed by card suit, seperate cards with a space");
                        string turnCards = Console.ReadLine();
                        if (!CheckUserCardInputIsValid(4, turnCards))
                        {
                            validCardsEnteredTurn = false;
                        }
                        else
                        {
                            AddSharedCards(turnCards);
                        }
                    }
                    break;
                case 5:
                    bool validCardsEnteredRiver = false;
                    while (!validCardsEnteredRiver)
                    {
                        validCardsEnteredRiver = true;

                        Console.WriteLine("Please enter the five shared cards in the form AH (ace of hearts) 8D (8 of diamonds), card value followed by card suit, seperate cards with a space");
                        string riverCards = Console.ReadLine();
                        if (!CheckUserCardInputIsValid(5, riverCards))
                        {
                            validCardsEnteredRiver = false;
                        }
                        else
                        {
                            AddSharedCards(riverCards);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private static void AddSharedCards(string sharedCards)
        {
            string[] sharedArray = sharedCards.Split();

            foreach (string card in sharedArray)
            {
                Card sharedCard = StringToCard(card);
                KnownSharedCards.Add(sharedCard);
                RemovedCardFromDeck(sharedCard);
            }
        }

        private static void GetOpponentInfo()
        {
            //ask player how many opponents there are
            //validate user input
            Console.WriteLine("How many Opponents are you playing against, must be at least 1 and no more than 9.");
            NumberOfOpponents = Convert.ToInt32(Console.ReadLine()); //TODO validate input!!

            Console.WriteLine("How many of your opponents have folded their hands");
            NumberOfFoldedOpponents = Convert.ToInt32(Console.ReadLine()); //TODO validate input!!
        }

        /// <summary>
        /// Extensive method used multiple times to check that the user input from the command line is valid and can be converted into Card objects
        /// </summary>
        /// <param name="numberOfCards"></param>
        /// <param name="userCardInput"></param>
        /// <returns></returns>
        private static bool CheckUserCardInputIsValid(int numberOfCards, string userCardInput)
        {
            HashSet<int> allowedCardValues = new HashSet<int>() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14};
            HashSet<char> allowedCardSuits = new HashSet<char>() { 'C', 'S', 'H', 'D'};

            string[] userCards = userCardInput.Split();
            HashSet<string> userCardsInHashSet = new HashSet<string>(userCards); //this is to check that no identical cards are added by comparing count vs userCards length

            //foreach (string cardInput in userCards)
            //{
            //    if (cardInput.Length != 2 && ((cardInput[cardInput.Length-1]).Equals("0") && (cardInput[cardInput.Length - 2]).Equals("1")))
            //    {
            //        Console.WriteLine("Cards are represented by only two characters");
            //        return false;
            //    }
            //}

            //Check the number of cards entered by the user matches how many cards should be added
            bool correctNumberOfCards = CheckUserInputCorrectNumberOfCards(numberOfCards, userCards);

            //Checks if multiple cards are entered that none of the cards are identical, compare last length to hashset count
            bool cardsAreUnique = CheckUserInputOnlyUniqueCards(userCards, userCardsInHashSet);

            //check the cards entered can be made into a card, correct size etc, new Card wont break the program with these values, try statement???


            //Check the cards being entered exist in the card deck
            bool cardsExistInDeck = CheckCardsEnteredExistsInDeck(userCards);

            return (correctNumberOfCards && cardsAreUnique && cardsExistInDeck);  //return true if all the checks have passed
        }

        /// <summary>
        /// Checks that the user has entered the correct number of cards
        /// </summary>
        /// <param name="numberOfCards"></param>
        /// <param name="userInsertedCards"></param>
        /// <returns></returns>
        private static bool CheckUserInputCorrectNumberOfCards(int numberOfCards, string[] userInsertedCards)
        {
            if(numberOfCards != userInsertedCards.Length)
            {
                Console.WriteLine("Enter the correct amount of cards.");
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Runs the simulation, randomly selects unknown cards to determine a winner, runs simulation many times to get a rough estimate at chance of winning
        /// </summary>
        /// <param name="numnberOfSimulations"></param>
        /// <param name="cardsStillInDeckOfCardList"></param>
        /// <param name="knownSharedCards"></param>
        /// <param name="player"></param>
        private static void MonteCarloSimulation(int numnberOfSimulations, List<Card> cardsStillInDeckOfCardList, List<Card> knownSharedCards, Player player, int numberOfOppoents, int numberOfFoldedOpponents)
        {
            for (int i = 0; i < numnberOfSimulations; i++)
            {
                //copies are created because we are running "n" simulations, certain known cards need to be persistent, unknown cards must be randomly chosen each simulation
                //lists are mutable so must create copies to not make any changes to the persistent lists for the actual known cards and not randomly selected simulated cards
                List<Card> copyOfDeckOfCardList = new List<Card>(cardsStillInDeckOfCardList);

                List<Card> copyOfKnownSharedCards = new List<Card>(knownSharedCards);

                List<Card> copyOfOurPlayersCards = new List<Card>(player.PlayersHand);

                //remove cards for folded players from the deck, each folder player has 2 cards in their hand that can be discounted from the pack, these cards are unknown so random cards removed each new simulation
                for (int handsToFold = 0; handsToFold < NumberOfFoldedOpponents; handsToFold++)
                {
                    copyOfDeckOfCardList.Remove(copyOfDeckOfCardList[random.Next(copyOfDeckOfCardList.Count - 1)]);
                    copyOfDeckOfCardList.Remove(copyOfDeckOfCardList[random.Next(copyOfDeckOfCardList.Count - 1)]);
                }

                //assign random cards to fill out the shared cards, unknown shared cards are picked randomly each simulation, ourPLayer and opponent has access to these cards in their "hand"
                for (int amountOfCardsToAdd = 0; amountOfCardsToAdd < (5 - KnownSharedCards.Count); amountOfCardsToAdd++)
                {
                    Card randomCardToAdd = copyOfDeckOfCardList[random.Next(copyOfDeckOfCardList.Count - 1)];
                    copyOfKnownSharedCards.Add(randomCardToAdd);
                    copyOfDeckOfCardList.Remove(randomCardToAdd);
                }

                //Add known and randomly selected shared cards to our players 7-card hand (only 5 best cards are chosen but have access to 7 cards in our hand
                foreach (Card allSharedCards in copyOfKnownSharedCards)
                {
                    copyOfOurPlayersCards.Add(allSharedCards);
                }

                Player finalPlayersHand = new Player(copyOfOurPlayersCards);

                //find out out players best hand and the highest cards in order of that hand
                finalPlayersHand.GetBestHand(finalPlayersHand.PlayersHand);

                bool handNotOver = true;

                int numberOfOpponetsBeaten = 0;

                int numberOfActiveOpponents = NumberOfOpponents - NumberOfFoldedOpponents;
                for (int opponentsFaced = 0; (opponentsFaced < numberOfActiveOpponents) & handNotOver; opponentsFaced++)
                {
                    Card randomCardForOpponent1 = copyOfDeckOfCardList[random.Next(copyOfDeckOfCardList.Count - 1)];
                    copyOfDeckOfCardList.Remove(randomCardForOpponent1);

                    Card randomCardForOpponent2 = copyOfDeckOfCardList[random.Next(copyOfDeckOfCardList.Count - 1)];
                    copyOfDeckOfCardList.Remove(randomCardForOpponent2);

                    Player opponentToFace = new Player(randomCardForOpponent1, randomCardForOpponent2);

                    foreach (Card allSharedCards in copyOfKnownSharedCards)
                    {
                        opponentToFace.Add(allSharedCards);
                    }

                    opponentToFace.GetBestHand(opponentToFace.PlayersHand); //optimisation, as soon as best possible hand is worse than finalPLayersHand.BestHand can stop checking the other hands and assign a win to ourPlayer

                    if(finalPlayersHand.BestHand > opponentToFace.BestHand)
                    {
                        numberOfOpponetsBeaten++;
                        if(numberOfOpponetsBeaten == numberOfActiveOpponents) //only add one to WinCount if you have beaten every opponent
                        {
                            WinCount++;
                        }
                        continue;
                    }
                    else if(finalPlayersHand.BestHand < opponentToFace.BestHand) //only need to lose to one opponent so can break once losing and add one to LossCount
                    {
                        LossCount++;
                        break;
                    }
                    else //have the same hand value so need to check who has the highest cards, also need to consider possibility of a draw (can be a later update)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            if(finalPlayersHand.HighestCards[x] > opponentToFace.HighestCards[x])
                            {
                                numberOfOpponetsBeaten++;
                                if (numberOfOpponetsBeaten == numberOfActiveOpponents) //only add one to WinCount if you have beaten every opponent
                                {
                                    WinCount++;
                                    handNotOver = false;
                                }
                                break;
                            }
                            else if(finalPlayersHand.HighestCards[x] < opponentToFace.HighestCards[x])
                            {
                                LossCount++;  //need a conditional here to break the prior for loop to so that we dont get duplicate losses from one hand
                                handNotOver = false;
                                break;
                            }
                        }
                    }

                }
            }
        }

        private static void ReturnChanceOfWinning()
        {
            int numberOfWins = WinCount;
            int numberOfLosses = LossCount;
            int numberOfDraws = DrawCount;

            float percentageOfWinning = ((float)numberOfWins / (float)NumberOfSimulations)*100;
            Console.WriteLine();
            Console.WriteLine("Your chance of winning is: {0}%", percentageOfWinning.ToString());

        }

        /// <summary>
        /// Checks if multiple cards are entered that none of the cards are identical, compare last length to hashset count
        /// </summary>
        /// <param name="userInsertedCards"></param>
        /// <param name="hashsetOfUserInsertedCards"></param>
        /// <returns></returns>
        private static bool CheckUserInputOnlyUniqueCards(string[] userInsertedCards, HashSet<string> hashsetOfUserInsertedCards)
        {
            if (userInsertedCards.Length != hashsetOfUserInsertedCards.Count)
            {
                Console.WriteLine("Enter the correct amount of cards or only cards that are available in the deck, duplicates are not allowed.");
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Checks that every card enterred by the user is a possible card by checking the card exists in the list of all cards (deckOfCardsList)
        /// </summary>
        /// <param name="userInsertedCards"></param>
        /// <returns></returns>
        private static bool CheckCardsEnteredExistsInDeck(string[] userInsertedCards)
        {
            bool cardExistsInDesk = false;
            foreach (string card in userInsertedCards)
            {
                cardExistsInDesk = false;

                Card cardToCheckForAvailability = StringToCard(card);  //new method to check string can become card before making it one!!!, this will break if we give it terrible info currently because string to card has no validation on the input!!!
                foreach (Card cardFromDeckList in DeckOfCardsList)
                {
                    if (cardFromDeckList.Equals(cardToCheckForAvailability))
                    {
                        cardExistsInDesk = true;
                    }
                }

                if (!cardExistsInDesk)
                {
                    Console.WriteLine("Only enter possible available cards.");
                    return false;
                }
                
            }
            return cardExistsInDesk;
        }

        /// <summary>
        /// Method that can take a string object that in the correct format can be modified so generateCard can be called to produce a Card object
        /// </summary>
        /// <param name="cardInformation"></param>
        /// <returns></returns>
        private static Card StringToCard(string cardInformation)
        {
            //Dictionary<string,int> to convert the string face value of a card to an int, useful for representing the face cards (jack, queen, king, ace) and checking for a straight
            Dictionary<string, int> cardValueStringToInt = new Dictionary<string, int>()
            {
                {"2", 2 },
                {"3", 3 },
                {"4", 4 },
                {"5", 5 },
                {"6", 6 },
                {"7", 7 },
                {"8", 8 },
                {"9", 9 },
                {"10", 10 },
                {"J", 11 },
                {"Q", 12 },
                {"K", 13 },
                {"A", 14 },
            };

            int cardValue;
            char cardSuit;

            if (cardInformation.Length == 2)
            {
                cardValue = cardValueStringToInt[cardInformation[0].ToString()];
                cardSuit = cardInformation[1];
            }
            else
            {
                cardValue = 10;
                cardSuit = cardInformation[2];
            }

            return GenerateCard(cardValue, cardSuit);
        }

        /// <summary>
        /// A method that takes cardValue (int) and carSuit(char) and generate a Card object based on the card value and the card suit
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private static Card GenerateCard(int cardValue, char cardSuit)
        {
            return new Card(cardValue, cardSuit);
        }

        /// <summary>
        /// //remove input cards from cardHandler list of Cards
        /// </summary>
        /// <param name="cardToRemove"></param>
        private static void RemovedCardFromDeck(Card cardToRemove)
        {
            foreach (Card cardFromDeckList in DeckOfCardsList)
            {
                if (cardFromDeckList.Equals(cardToRemove))
                {
                    DeckOfCardsList.Remove(cardFromDeckList);
                    break;
                }
            }
        }
    }
}
