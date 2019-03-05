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
        public const string Bank = "[Bank]";
        public bool debug = false;
        public bool passive = false;
        public bool primed = false;
        public bool sale = false;
        public bool smalltank = true;
        public bool setup = false;

        private decimal = ;
        private int _PRICE = 0.22; //defauilt price.

        // GTH ICE COST = 0.227
        // Processing fee = 

        /**
         * Top connectors are set to SHARE WITH ALL. connect through bridge connector. 
         * INlet to tank, outlet to the customer's ship. 
         *      opened after Sale state completed;
         *      
         *      think of main line as 'the rest of their base'
         *      
         *      
         *      main line is OFF unless we're refilling the prime tank?
         *      
         *      Inlet for specified tank turns ON when the OUTLET is turned OFF
         *          determined by when tank reaches 0% OR Timer reach zero.
         *          
         *          (capacity == 0 || capacity >= 0 && 'timer elapsed')
         */

        //renamed from Setup; constructor to conform with PB syntax
        public Program()
        {

            //Instantiate variables 

            List<IMyGasTank> myTanks = new List<IMyGasTank>();
            List<IMyShipConnector> myConnectors = new List<IMyShipConnector>();
            List<IMyCargoContainer> myCargoContainers = new List<IMyCargoContainer>();

            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(myTanks);
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(myConnectors);
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(myCargoContainers);

            //check for each name

            //Hydrogen tanks
            IMyGasTank tankBig = GridTerminalSystem.GetBlockWithName(BT) as IMyGasTank;      //[Big Tank]
            IMyGasTank tankSmall = GridTerminalSystem.GetBlockWithName(ST) as IMyGasTank;    // [SMALL TANK]
            
            //Connectors 
            IMyShipConnector conSmallFuelTankInlet = GridTerminalSystem.GetBlockWithName(SI) as IMyShipConnector;  // Small Fuel Tank Inlet
            IMyShipConnector conLargeFuelTankInlet = GridTerminalSystem.GetBlockWithName(BI) as IMyShipConnector;  // Large Fuel Tank Inlet
            IMyShipConnector conSmallFuelTankOutlet = GridTerminalSystem.GetBlockWithName(SO) as IMyShipConnector; // Small Fuel Tank Outlet
            IMyShipConnector conLargeFuelTankOutlet = GridTerminalSystem.GetBlockWithName(BO) as IMyShipConnector; // Big Fuel Tank Outlet
            IMyShipConnector conSmallFuelPortInlet = GridTerminalSystem.GetBlockWithName(SFI) as IMyShipConnector; // Small Fuel Port Inlet
            IMyShipConnector conLargeFuelPortInlet = GridTerminalSystem.GetBlockWithName(LFI) as IMyShipConnector; // Large Fuel Port Inlet
            IMyShipConnector conCustomerFuelPort = GridTerminalSystem.GetBlockWithName("Connector [Fuel Port Inlet]") as IMyShipConnector;

            //Cargo containers
            IMyCargoContainer cargoBankBox = GridTerminalSystem.GetBlockWithName("Bank Box") as IMyCargoContainer;
            IMyCargoContainer cargoPaymentBox = GridTerminalSystem.GetBlockWithName("Payment Box") as IMyCargoContainer;

            //debug, fueling, primetankstatus

            IMyTextPanel debugLCD = GridTerminalSystem.GetBlockWithName("debug LCD") as IMyTextPanel;
            IMyTextPanel LCDSmFuel = GridTerminalSystem.GetBlockWithName("S fuel LCD") as IMyTextPanel;          //LCD Small Fuel
            IMyTextPanel LCDLgFuel = GridTerminalSystem.GetBlockWithName("L fuel LCD") as IMyTextPanel;          // LCD Large Fuel
            
            // SETUP ALL CONNECTORS TO DISCONNECT!!! NO STEALING FUEL!!!

            // DO NOT WORRY ABOUT STEALING FUEL UNTIL WE GET THE SYSTEM PUMPING FUEL INTO SHIPS

            foreach (IMyShipConnector connector in myConnectors)
            {
                //would powering off the connector be more certain to prevent unwanted connections?
                connector.Disconnect();
            }

            // replaced by the above loop
            /*
                conPrimeTankFill.Disconnect();
                conPrimeTankOutlet.Disconnect();
                conFTI.Disconnect();
                conFO.Disconnect();
                conFP.Disconnect();
                confpi.Disconnect();
            */


            // SETUP ALL TANKS TO NOT STOCKPILE!!! NO STEALING FUEL
            foreach (IMyGasTank hydrogenTank in myTanks)
            {
                hydrogenTank.Stockpile = false;
            }
            setup = true;  //is this supposed to be an indicator that the setup ran successfully?  if so, we should rename this. 
        }

        /// <summary>
        /// Entry point in the script.  
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="updateSource"></param>
        public void Main(string argument, UpdateType updateSource)
        {
            //probably use an if() or a case switch to figure out which state we're supposed to be in
            //we'll still need a default thing. 

            

            //handle updating of gas prices 
            if(debug

            // 250,000 units of hydrogen per Large Tank
            
        }



        //================================================================//
        /*                      NON-REQUIRED METHODS                      */
        //================================================================//
        
        private void UpdatePrice(int newPrice)
        {

        }

        private void UpdateProcessingFee(decimal )


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
                 *  System will NOT ALLOW Prime State (second state) until given tank is full.  ("given tank" being the one on the gas station.  Not the customer.)
                 *  use method to check percentage of tank to ensure transactions ar efull
                 *  after tank is full, turn Small tank stockpile OFF
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
             *  small inlet OFF from primed 
             *  transacation continues until small tank reads 0L and returns to passive state
             *
             *  if(small tank vol > 0L || time elapses && small tank vol >= 0L 
             *      return to passive state
             *
             *
             *
             *
             *
             */



            /**
             * transaction wil not occur if credits in small transaction box is LESS THAN transacation cost
                    when this IS true, move to SALES STATE
                    at the beginning of this method, run an extra check to verify once again 

             * check that there is a ship to refuel at the connector
             *  loop until we see one
             *  
             * when we see one, update LCD screen 
             *  "would you like to purchase fuel? current price: ..."
             *  
             *  wait to receive EXACT credits from DepositBox
             *  
             *  connect ship
             *  
             *  disconnect prime tank from Fuel tank
             *  
             *  connect fuel tank to fuel nozzle
             *  
             *  Remain in sale state until...
             *  if 
             *  
             */
        }

        public void UpdateLCD()
        {

        }
    }
}
