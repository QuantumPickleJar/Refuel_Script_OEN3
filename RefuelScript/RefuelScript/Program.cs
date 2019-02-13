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

        



        string _State = string.Empty;
        public Program()
        {
            //maybe change this to 10?
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
            IMyTerminalBlock refuel_station = new IMyTerminalBlock();
            // Passive State:
            // [Small Tank] -> stockpile ON;
            // [Small Outlet] OFF; 
            // [Small Inlet] ON;
            // [Main Line] ON;
            // System will NOT allow Primed State unless Given Tank is full.
            // Uses a method to check on the percentage of tank to ensure transactions are disabled;
            // [Small Tank] is full = 100 %;
            // After tank is full, 
            // if([Small Tank].stored = 100]turns[Small Tank] Stockpile: OFF; Turn[Small Inlet] OFF, ([Small Outlet] is still OFF) Transferring to Primed State.

            if (IMyGasTank)
        //Primed State:
        //Need System that has a easily modifiable variable integer for Transaction Cost after system is in Primed State. Credit placed in [Small Transaction] need to meet Transaction Cost in order for transaction to occur.The remaining credit must not be pulled into the bank after transaction. Transaction Cost must be able to accumulate in separate stacks of Credit. LCD API must have Transaction Cost constant string, as well as total of Credit added into[Small Transaction Cargo]. Transaction will not occur if [Small Transaction] is < Transaction Cost. Once met, system Transfers to Sale State.

                //Sale State:
                //Once Transaction Cost is met, or gone over cost and player presses button panel or sensor activates the transaction: pull Transaction Cost into Bank and leave remaining balance in [Small Transaction]. [Small Outlet] = ON(Small Inlet still disabled from Priming State).Transaction continues until [Small Tank] Reads 0% volume return to Passive State.If[Small Tank] reads Volume > 0% for more than 10-15 minutes return to Passive State.
                //----




        }
    }
}