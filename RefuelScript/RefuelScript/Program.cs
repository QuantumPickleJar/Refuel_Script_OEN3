using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    //DO NOT RENAME
    partial class Program : MyGridProgram
    {

        public const string ML = "[Main Line]";  // NAME OF MAIN LINE CONNECTOR (Default: "Prime Tank Fill")
        public const string PO = "[Prime Outlet]"; // NAME OF Prime tank Outlet
        public const string BI = "[Big Inlet]";  // NAME OF BIG INLET CONNECTOR Large Fuel Tank Inlet
        public const string BO = "[Big Outlet]"; // NAME OF BIG OUTLET CONNECTOR Large Fuel Tank Outlet
        public const string SI = "[Small Inlet]"; // NAME OF SMALL OUTLET CONNECTOR Small Fuel Tank Inlet
        public const string SO = "[Small Outlet]"; // NAME OF SMALL INLET CONNECTOR Small Fuel Tank Outlet
        public const string BT = "[Big Tank]";  // NAME OF LARGE HYD SALE TANK
        public const string ST = "[Small Tank]"; // NAME OF SMALL HYD SALE TANK
        public const string PT = "[PRIME TANK]"; // NAME OF PRIME TANK
        public const string SFI = "[Small Fuel Inlet]"; // Name of Small Fuel Port Inlet Connector
        public const string LFI = "[Large Fuel Inlet]"; // Name of Large Fuel Port Inlet Connector
        public const string LCDDEBUG = "LCD [DEBUG]";
        public const string LCDSS = "LCD [SMALL SALE]";
        public const string LCDLS = "LCD [LARGE SALE]";
        public const string PTSTAT = "LCD [PRIME TANK STATUS]";
        public const string TRANS = "[Transaction]";
        public const string BANK = "[Bank]";
        public bool debug = false;
        public bool passive = false;
        public bool primed = false;
        public bool sale = false;
        public bool smalltank = true;
        public bool setupComplete = false;

        private decimal processingFee = (decimal)0.05;
        //private double icePrice = 0.05;
        private double _PRICE = 0.22; //defauilt price.


        // GTH ICE COST = 0.227
        // Processing fee = 1.05

        /**
         * Top connectors are set to SHARE WITH ALL. connect through bridge connector. 
         * INlet to tank, outlet to the customer's ship. 
         *      Outlet opened after Sale state completed;
         *      
         *      think of main line as 'the rest of their base'
         *      
         *      
         *      main line is OFF unless we're refilling the Transaction tank? YES
         *      
         *      Inlet for specified tank turns ON ONLY when the OUTLET is turned OFF FIRST
         *          determined by when tank reaches 0% OR Tank > 0% & Timer reach zero.
         *          
         *          (capacity == 0 || capacity >= 0 && 'timer elapsed')
         */
            List<IMyGasTank> myTanks = new List<IMyGasTank>();
            List<IMyGasTank> myOxygenTanks= new List<IMyGasTank>();
            List<IMyGasTank> myHydrogenTanks= new List<IMyGasTank>();
            List<IMyShipConnector> myConnectors = new List<IMyShipConnector>();
            List<IMyCargoContainer> myCargoContainers = new List<IMyCargoContainer>();


        //publicly declare variables 
        

        public IMyGridTerminalSystem GTSystem;
        public IMyCubeGrid Grid;

        public IMyGasTank tankSmall;
        public IMyGasTank tankBig;
        
        public IMyShipConnector conSmallFuelTankOutlet;
        public IMyShipConnector conLargeFuelTankOutlet;
        public IMyShipConnector conSmallFuelPortInlet;
        public IMyShipConnector conLargeFuelPortInlet;
        public IMyShipConnector conSmallFuelTankInlet;
        public IMyShipConnector conLargeFuelTankInlet;
        public IMyShipConnector conCustomerFuelPort;

        public IMyCargoContainer cargoBankBox;
        public IMyCargoContainer cargoPaymentBox;
        
        public IMyTextPanel debugLCD;
        public IMyTextPanel LCDSmFuel;
        public IMyTextPanel LCDLgFuel;


        public Program()
        {

            setupComplete = false;
            //Instantiate variables 
            SetBlocks();

            //add runtime stuff here later - VTM
        }


        /// <summary>
        /// Entry point in the script.  
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="updateSource"></param>
        public void Main(string argument, UpdateType updateSource)
        {
            //TODO: add case: switch or if() structure to determine which state we need to be in
            //Can also be converted to a method 

            //DetermineState();

            //setup 
            Setup();

            //======================= ARGUMENT HANDLING ==========================//

            //DEBUG METHOD:
            switch (argument)
            {
                //print # of credits in [Transaction]
                case "check":
                    Echo("[Transaction]: " + GetNumberOfCredits(TRANS).ToString());
                    return;

                //print # credits in [Bank]
                case "vault":
                    Echo("[Bank]: " + GetNumberOfCredits(BANK).ToString());
                    return;

                //print current price and processing fee
                case "prices":
                    GetPriceSummary();
                    break;
                default:
                    break;
            }
            //update price of ice (syntax: "ice_[integer]")
            if (argument.Contains("ice_"))
            {
                int i;
                if (int.TryParse(argument.Substring(argument.IndexOf("_") + 1), out i))
                {
                    UpdatePrice(i);
                }
                else
                {
                    Echo("Invalid price. ('ice_PRICE')");
                    return;
                }
            }
            //change price of ice (will change this to be done through an LCD's CustomData later)

            //=======================   MAIN METHOD  =========================//



            //


            //handle updating of prices 


            // 250,000 units of hydrogen per Large Tank

        }

        /// <summary>
        /// Sets all tanks to no-stockpile, and disconnects all connectors.
        /// </summary>
        private void Setup()
        {
            foreach (IMyShipConnector connector in myConnectors)
            {
                connector.Disconnect();
                //Echo(connector.Name + " disconnected." + "\n");

            }
            //Echo("All connectors unlocked.");


            //Set every Hydrogen and Oxygen tank's Stockpiling to Off
            foreach (IMyGasTank tank in myTanks)
            {
                Echo("Name: " + tank.CustomName + "\n" +
                    "Dinfo: " + tank.DetailedInfo + "\n" 
                    /*+"Cinfo: " + tank.CustomInfo*/);

                tank.Stockpile = false;
            }
            setupComplete = true;
            //Echo("All Hydrogen tanks set to no stockpile.");
        }

        /// <summary>
        /// Checks how many Credits are present in cargo container by name
        /// </summary>
        /// <param name="name">Name of the cargo container</param>
        /// <returns>number of credits, -1 if cargo container couldn't be found.</returns>
        private int GetNumberOfCredits(string name)
        {
            //Verify that the specified container exists
            GTSystem.GetBlocksOfType<IMyCargoContainer>(myCargoContainers, block => block.CustomName.EndsWith(name));
            if (myCargoContainers.Count < 1)
            {
                //if it doesn't exist, output to Programming Block
                Echo("Container " + name + " not found.");
                return -1;
            }
            //Retrieve the inventory of the container
            IMyInventory invTrans = myCargoContainers[0].GetInventory(0);

            //set the inventory of the transaction cargo 
            List<MyInventoryItem> itemlistTrans = new List<MyInventoryItem>();

            //get complete list of items from the transaction cargo regardless of item properties 
            invTrans.GetItems(itemlistTrans, null);
            
            //return how many items of ItemType "Credit" as an int (converted from long) to save memory 
            return invTrans.GetItemAmount(MyItemType.MakeIngot("Credit")).ToIntSafe();
        }




        //================================================================//
        /*                      NON-REQUIRED METHODS                      */
        //================================================================//


        private void SetBlocks()
        {
            GTSystem = GridTerminalSystem;

            //inventory

            //Lists
            GTSystem.GetBlocksOfType<IMyGasTank>(myTanks);
            GTSystem.GetBlocksOfType<IMyGasTank>(myOxygenTanks, tank => tank.DetailedInfo.Contains("Oxygen"));
            GTSystem.GetBlocksOfType<IMyGasTank>(myHydrogenTanks, tank => tank.DetailedInfo.Contains("Hydrogen") && 
                                                                         (tank.CustomName == ST || tank.CustomName == BT));
            if(myHydrogenTanks.Count < 1)
            {
                Echo("Critical Error: \n" + "Hydrogen tanks " + ST + " or " + BT + "not found.");
                return;
            }

            GTSystem.GetBlocksOfType<IMyShipConnector>(myConnectors);
            GTSystem.GetBlocksOfType<IMyCargoContainer>(myCargoContainers);

            //Hydrogen tanks
            tankBig = GTSystem.GetBlockWithName(BT) as IMyGasTank;      //[Big Tank]
            tankSmall = GTSystem.GetBlockWithName(ST) as IMyGasTank;    // [SMALL TANK]


            //Connectors 
            conSmallFuelTankInlet = GTSystem.GetBlockWithName(SI) as IMyShipConnector;  // Small Fuel Tank Inlet
            conLargeFuelTankInlet = GTSystem.GetBlockWithName(BI) as IMyShipConnector;  // Large Fuel Tank Inlet 
            conSmallFuelTankOutlet = GTSystem.GetBlockWithName(SO) as IMyShipConnector; // Small Fuel Tank Outlet
            conLargeFuelTankOutlet = GTSystem.GetBlockWithName(BO) as IMyShipConnector; // Big Fuel Tank Outlet
            conSmallFuelPortInlet = GTSystem.GetBlockWithName(SFI) as IMyShipConnector; // Small Fuel Port Inlet
            conLargeFuelPortInlet = GTSystem.GetBlockWithName(LFI) as IMyShipConnector; // Large Fuel Port Inlet
            try
            {
                conCustomerFuelPort = GridTerminalSystem.GetBlockWithName("Connector [Fuel Port Inlet]") as IMyShipConnector;
                //Cargo containers
                cargoBankBox = GTSystem.GetBlockWithName("Bank Box") as IMyCargoContainer;
                cargoPaymentBox = GTSystem.GetBlockWithName("Payment Box") as IMyCargoContainer;

                //debug, fueling, primetankstatus

                debugLCD = GTSystem.GetBlockWithName("debug LCD") as IMyTextPanel;
                LCDSmFuel = GTSystem.GetBlockWithName("S fuel LCD") as IMyTextPanel;          //LCD Small Fuel
                LCDLgFuel = GTSystem.GetBlockWithName("L fuel LCD") as IMyTextPanel;          // LCD Large Fuel
            }
            catch (Exception e)
            {
                Echo("Error setting blocks. Verify correct names.");
            }
        }

        /// <summary>
        /// Changes the cost of 1L of Hydrogen
        /// </summary>
        /// <param name="newPrice"></param>
        private void UpdatePrice(int newPrice)
        {
            this._PRICE = (double)newPrice;
            Echo("Price updated to " + _PRICE.ToString() + " Cr");
        }


           /// <summary>
           /// Changes processing fee
           /// </summary>
           /// <param name="newFee"></param>
        private void UpdateProcessingFee(decimal newFee)
        {
            this.processingFee = newFee;
            string visualPf;
            //determine what kind of format was used
            if (processingFee > 0 && processingFee < 1)
            {
                visualPf = (processingFee * 100).ToString();
            }
            else { visualPf = processingFee.ToString(); }

            Echo("Processing fee updated to " + visualPf + "%"); 
        }

        private void GetPriceSummary()
        {
            string visualPf;
            //determine what kind of format was used
            if (processingFee > 0 && processingFee < 1)
            {
                visualPf = (processingFee * 100).ToString();
            }
            else { visualPf = processingFee.ToString(); }

            Echo("Price: " + _PRICE.ToString() + " Cr");
            Echo("Processing fee: " + visualPf + "%");
        }

        /// <summary>
        /// Method to set the system in Passive mode 
        /// </summary>
        public void SetPassiveState()   
        {
            /**
             *  Tasks:
             *  
                 *  Small tank is ON for stockpile
                 *  small outlet OFF
                 *  small inelt ON
                 *  main line ON
                 *  System will NOT ALLOW Prime State (second state) until Transaction Tank (Large or Small for Refueling)
                 *  use method to check percentage of tank to ensure transactions are full
                 *  after tank is full, turn Transaction Tank stockpile OFF
                 *  turn small inlet off
                 *  small outlet STILL off
                 *  after all of that, transfer to PRIME STATE
                 *  
             *  modifiable system INTEGER for price of fuel. 
             
             *      passive      ->         Prime             ->         sale
             *     refill pump        ready for transaction         emptying pump          
             */
        }
        
        //DO NOT AUTOMATE STOCKPILING OF CUSTOMER

        public void PrimeTanks()
        {

            /**
             * 
             * PRIME
             *  credit placed in small transaction box
             *      ^needs to meet transaction COST
             *       remaining credit must NOT be pulled into Bank
             *  
             *  transaction cost must be able to accumulate via separate stacks within small transaction box.
             *  
             *                                  EXAMPLE
             *     { ex cost: 100:                                                       }
             *     { customer puts in 50, not enough.                                    }
             *     { customer puts in another 50, most consolidate the stacks.           }
             *     {                                                                     }
             *     { of 110 is in, but the cost is 100, create new stack of 10 to return }
             * 
             * ~~~~Other requirements~~~
             *  LCD api must have transaction cost as transaction constant string
             *      total of credit added into the container
             *      - display to customer how much is left to pay,
             *      
             */
        }


        /// <summary>
        /// Method that starts the Sales state 
        /// </summary>
        public void BeginTransaction()
        {
            //FUELING PROCESS DOES NOT BEGIN IN SALE STATE

            /** 
             *  Once transacation  is met and player presses button/sensor:
             *  
             *  Tasks:
             *  
             *  pulls credits inside box into BANK, leaving remaining balance in small transaction box
             * 
             *  small outlet turns ON
             *  small inlet OFF (Still OFF from Primed State)
             *  transacation continues until transaction tank reaches 0% OR Tank > 0% & Timer reach zero.
             *  After Transaction, System returns to Passive State.
             *
             *  if(small tank vol > 0L || time elapses && small tank vol >= 0L 
             *      return to passive state
             */


            /**
             * transaction will not occur if credits in small transaction box is LESS THAN transacation cost
                    when this IS true, move to SALES STATE (Pulls Credits from Transaction Cargo, into [Bank], Prevents Thievery)
                    at the beginning of this method, run an extra check to verify once again 

             */
        }
       
        /// <summary>
        /// Echoes text to an LCD by name
        /// </summary>
        /// <param name="lcdName">name of the LCD block to update</param>
        public void UpdateLCD(string lcdName)
        {
            
        }
    }
}
