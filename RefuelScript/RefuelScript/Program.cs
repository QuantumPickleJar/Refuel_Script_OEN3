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


        public void Setup()
        {
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
            IMyGasTank tankPrime = GridTerminalSystem.GetBlockWithName(PT) as IMyGasTank;    // Prime Tank

            //Connectors 
            IMyShipConnector conPrimeTankFill = GridTerminalSystem.GetBlockWithName(ML) as IMyShipConnector;       //Prime Tank Fill Connector
            IMyShipConnector conPrimeTankOutlet = GridTerminalSystem.GetBlockWithName(BO) as IMyShipConnector;     // Prime Tank Outlet Connector
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
            IMyTextPanel LCDSFuel = GridTerminalSystem.GetBlockWithName("S fuel LCD") as IMyTextPanel;   //LCD Small Fuel
            IMyTextPanel LCDLFuel = GridTerminalSystem.GetBlockWithName("L fuel LCD") as IMyTextPanel;   // LCD Large Fuel
            IMyTextPanel primeTankLCD = GridTerminalSystem.GetBlockWithName("PrimeTank LCD") as IMyTextPanel;

            // SETUP ALL CONNECTORS TO DISCONNECT!!! NO STEALING FUEL!!!

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

        public void Checks()
        {
            //figure out what we're checking 
        }

        public void Connection()
        {
            //I don't know if we need 
        }

        public void Disconnect()
        {

        }

        public void Passive()   //First Check state of Connectors, Then if any are wrong reset connectors. 
        {
            //
            if (tankPrime.filledvalue < 1)
            {
                pt.stockpile = true;
                conPTFill.Connect();
                Checks();
                LCDSFuel.writeto("Status: Filling Prime Tank Please wait!");
                LCDLFuel.writeto("Status: Filling Prime Tank Please wait!");

            }

            else if ((PrimeTankF.filledvalue == 1)
            {
                conPTFill.Disconnect();
                LCDSFuel.writeto("Status: Filling Prime Tank Please wait!");
                LCDLFuel.writeto("Status: Filling Prime Tank Please wait!");
                Checks();

            }

        }

        public void Primed()
        {

        }

        public void Sale()
        {

        }

        public void UpdateLCD()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!setup) Setup();

        }

    }
}
}