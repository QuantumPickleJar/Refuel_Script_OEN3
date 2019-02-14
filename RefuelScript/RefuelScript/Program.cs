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


        //  Declarations:
        //  Scan grid for blocks named[Main Line], [Big Inlet], [Big outlet], [Small Inlet], [Small Outlet], [Big Tank], [Small Tank]. Each PB will have a seperate cargo for transaction, [Small Transaction]; [Large Transaction].
        //  use a central container named[Bank] for storing credits after transaction 
        // ## are we using physical credits?

            /// <summary>
            /// Property FuelPrice (set with _fuelPrice)
            /// </summary>
        private float _fuelPrice;
        public float FuelPrice { get { return this._fuelPrice; } private set { this._fuelPrice = value; } }

        private bool _TRANSACTION_ALLOWED = false;
        
        string _State = string.Empty;

        public Program()
        {
            //maybe change this to 10?
            // code involving transactions should probably be here since this is called before the Main() method
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

        }

        public void Save()
        {
            //

            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {

            //[[[[[[[[[[[[[[[[[[[[[ DECLARATIONS ]]]]]]]]]]]]]]]]]]]]]]]]]

            // scan for blocks named:
            // [Main Line], [Big Inlet], [Big outlet], [Small Inlet], [Small Outlet], [Big Tank], [Small Tank],[TB - Timeout]
            // [System Boot] for sensor to boot PB when Player arrives station.
            // Boot sequence begins Timer[TB - Timeout]; 


            // After 1 hour if NoPlayer detected, system shuts down both PB's [Small PB], [Large PB] and sets [Main Line] OFF. [Small Actuator] and [Large Actuator] labels to define the sensor or button to activate Sale State.
            // 
            // 
            //IMyCubeGrid refuel_station = new IMyCubeGrid();

            //finish later 
            //IMyProgrammableBlock smPB;
            //smPB = GridTerminalSystem.GetBlockWithName("[Small PB]");

            // Passive State:
            // [Small Tank] -> stockpile ON;
            // [Small Outlet] enabled -> OFF; 
            // [Small Inlet] enabled -> ON;
            // [Main Line] emnabled -> ON;

            // System will NOT allow Primed State unless Given Tank is full.
            // ## what is Given Tank, the tank of a connected ship we're refuellng?  if so, which tank?
            // ## might need a for-each loop in that case 

            // Uses a method to check on the percentage of tank to ensure transactions are disabled;
            // ## why can't we just use an if()?

            // [Small Tank] is full = 100 %;
            // After tank is full, 
            // if([Small Tank].stored = 100]turns[Small Tank] Stockpile: OFF; Turn[Small Inlet] OFF, ([Small Outlet] is still OFF) Transferring to Primed State.

            ////if (IMyGasTank is full)
            //{
            //    //turn smTank.stockpile OFF; 
            //}


            //Primed State:
            //Need System that has a easily modifiable variable integer for Transaction Cost after system is in Primed State. Credit placed in [Small Transaction] need to meet Transaction Cost in order for transaction to occur.The remaining credit must not be pulled into the bank after transaction. Transaction Cost must be able to accumulate in separate stacks of Credit. LCD API must have Transaction Cost constant string, as well as total of Credit added into[Small Transaction Cargo]. Transaction will not occur if [Small Transaction] is < Transaction Cost. Once met, system Transfers to Sale State
            //only allow this variable to be changed 
            //Sale State:
            //Once Transaction Cost is met, or gone over cost and player presses button panel or sensor activates the transaction: pull Transaction Cost into Bank and leave remaining balance in [Small Transaction]. [Small Outlet] = ON(Small Inlet still disabled from Priming State).Transaction continues until [Small Tank] Reads 0% volume return to Passive State.If[Small Tank] reads Volume > 0% for more than 10-15 minutes return to Passive State.
            //----




        }
    }
}