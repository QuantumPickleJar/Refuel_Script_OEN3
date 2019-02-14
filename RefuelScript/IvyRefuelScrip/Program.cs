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
    partial class Program : MyGridProgram
    {
        // Isy's Ship Refueler
        // ===============
        // Version: 1.8.0
        // Date: 2018-09-24

        // =======================================================================================
        //                                                                            --- Configuration ---
        // =======================================================================================

        // -- Batteries --
        // =======================================================================================

        // By default, all batteries of the ship are monitored by the script. If you want to monitor specific ones,
        // group them together and put the group name here. Leave blank to monitor all.
        // Example: string batteryGroupName = "My Vehicle Batteries";
        string batteryGroupName = "";

        // Set batteries to discharge when not docked?
        // If this option is set to false, the batteries will be in In/Out mode (neither charge nor discharge)
        bool dischargeWhenUndocked = false;

        // Always keep one backup battery when docked?
        // The script will otherwise set all batteries to recharge if the ship is not controlled by a player.
        // This is highly recommended for fully automated, self docking drones
        bool keepOneBackupBattery = false;


        // -- Tanks --
        // =======================================================================================

        // If this is enabled, the script will switch tanks to stockpile, if you are docked.
        // That way the tanks will be filled very fast. Stockpile is deactivated when undocking.
        bool fillTanks = true;


        // -- Connectors --
        // =======================================================================================

        // By default, the script monitors all connectors of the ship for a connection.
        // If you only want to monitor specific ones, create a group and put the name here.
        string connectorGroupName = "";


        // -- Special connections --
        // =======================================================================================
        // If you don't want to perform a specific action when docked to a connector, add the keyword to its name.
        // The keyword must be added to the other connector's name, NOT your ship's connector!

        // -- No refuel --
        // This disables battery charging and tank filling (they are turned off).
        string noRefuelKeyword = "[No Refuel]";

        // -- Ignore --
        // This ignores a connection and leaves batteries and tanks the way they are.
        string ignoreKeyword = "[Ignore]";

        // -- No recharge --
        // This disables battery recharging (they are turned off).
        string noRechargeKeyword = "[No Recharge]";

        // -- Discharge at connector --
        // This discharges all batteries to a minimal value of dischargeLevel % charge (they are set to discharge).
        string dischargeKeyword = "[Discharge]";
        double dischargeLevel = 10;

        // -- No tank filling --
        // This disables tank filling of all tanks of the ship (they are turned off)
        string noTankFillKeyword = "[No Refill]";

        // -- No O2 filling --
        // This disables O2 tank filling (they are turned off)
        string noOxygenTankFillKeyword = "[No O2 Refill]";

        // -- No H2 filling --
        // This disables H2 tank filling (they are turned off)
        string noHydrogenTankFillKeyword = "[No H2 Refill]";


        // -- Emergency Reactors --
        // =======================================================================================

        // If your ship runs low on battery charge or the output gets overloaded, the script will
        // enable the reactors to help out. If everything is normal again, the reactors will be disabled.
        bool enableEmergencyReactors = true;
        bool activateOnLowBattery = true;
        bool activateOnOverload = true;
        bool activateOnDamagedBatteries = true;
        bool activateOnNoBatteries = true;

        // Tresholds for the reactor to kick in in percent
        double lowChargePercentageON = 10;
        double lowChargePercentageOFF = 15;
        double overloadPercentage = 90;


        // -- Light control --
        // =======================================================================================

        // If you want to switch the lights of your ship automatically, you can activate this feature here.
        // You need a solar panel to measure the light level. The light level is measured in % of the max output.
        bool lightControl = false;
        double lightLevel = 50;

        // By default, all solar panels of the ship are monitored. If you want to monitor specific solar panels,
        // declare groups for them.
        // Example: string[] solarPanelNames = { "My Solar Panel Group", "Ship Solar Panels" };
        string[] solarPanelGroups = { };

        // To only toggle specific lights, declare groups for them.
        // Example: string[] lightGroups = { "Interior Lights", "Spotlights", "Position Lights" }
        string[] lightGroups = { };


        // --- LCD panels ---
        // =======================================================================================

        // To display the main script informations, add the following keyword to any LCD name (default: !ShipRefuel).
        // You can enable or disable specific informations on the LCD by editing its custom data.
        string mainLCDKeyword = "!ShipRefuel";


        // --- Terminal statistics ---
        // =======================================================================================

        // The script can display informations in the names of the used blocks. The shown information is a percentage of
        // the current output (solar panels and reactors) or the fill level (batteries and tanks).
        // You can enable or disable single statistics or disable all using the master switch below.
        bool enableTerminalStatistics = true;

        bool showBatteryStats = true;
        bool showTankStats = true;
        bool showReactorStats = true;
        bool showSolarStats = true;


        // -- Timer trigger on event --
        // =======================================================================================

        // If you want to trigger a timer block to execute additional actions on specific events, you can
        // declare it here. The timer will be executed with the action "Trigger Now" if its delay is set to exactly 1 second and
        // "Start" if it is more than 1 second (1.x seconds or more)
        //
        // Events can be: "dock", "undock", a battery fill level like "25%" or a docking time in minutes:seconds like "5:30"
        //
        // Every event needs a timer block name in the exact same order as the events.
        // Calling the same timer block with multiple events requires it's name multiple times in the timers list!
        //
        // Example:
        // string[] events = { "dock", "undock", "25%" };
        // string[] timers = { "Timer 1", "Timer 2", "Timer 3" };
        // This will trigger "Timer 1" when docking, "Timer 2" when undocking and "Timer 3" when the battery level is at 25%
        string[] events = { };
        string[] timers = { };


        // =======================================================================================
        //                                                                      --- End of Configuration ---
        //                                                        Don't change anything beyond this point!
        // =======================================================================================

        // Base grid and connected grids
        IMyCubeGrid baseGrid = null;
        HashSet<IMyCubeGrid> connectedGrids = new HashSet<IMyCubeGrid>();

        // Lists
        List<IMyShipConnector> connectors = new List<IMyShipConnector>();
        List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
        List<IMyGasTank> allTanks = new List<IMyGasTank>();
        List<IMyGasTank> oxygenTanks = new List<IMyGasTank>();
        List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();
        List<IMySolarPanel> solarPanels = new List<IMySolarPanel>();
        List<IMyInteriorLight> lights = new List<IMyInteriorLight>();
        List<IMyReflectorLight> spotlights = new List<IMyReflectorLight>();
        List<IMyReactor> reactors = new List<IMyReactor>();
        List<IMyTextPanel> lcds = new List<IMyTextPanel>();
        List<IMyShipController> cockpits = new List<IMyShipController>();

        // Battery variables
        double storedPowerAll = 0;
        double storedPowerAllMax = 0;
        double currentInputAll = 0;
        double maxInputAll = 0;
        double currentOutputAll = 0;
        double maxOutputAll = 0;
        bool underControl = false;
        TimeSpan powerTime = new TimeSpan();
        DateTime balanceTime = DateTime.Now;

        // Tank variables
        double oxygenTankCapacity = 0;
        double oxygenTankFillLevel = 0;
        double hydrogenTankCapacity = 0;
        double hydrogenTankFillLevel = 0;

        // Timer blocks
        bool correctTimers = false;
        string lastEvent = "";

        // Solar panel variables
        double solarOutput = 0;
        double solarOutputMax = 0;

        // Reactor variables
        bool enableReactors = false;
        string lastActivation = "";
        string enableReactorReason = "";

        // Variable for connection
        IMyShipConnector dockedConnector = null;
        bool docked = false;
        bool refuel = true;
        bool ignore = false;
        bool recharge = true;
        bool discharge = false;
        bool refill = true;
        bool oxygenRefill = true;
        bool hydrogenRefill = true;
        string dockedTo = "nothing";
        DateTime dockingTime = DateTime.MinValue;
        TimeSpan timeSinceDock = new TimeSpan();

        // LCD variables
        Dictionary<long, List<int>> scroll = new Dictionary<long, List<int>>();
        string workingIndicator = "/";
        int workingCounter = 0;
        List<string> defaultCustomDataLCD = new List<string>{
    "showConnectionStatus=true",
    "showPowerTime=true",
    "showEmergencyReactor=true",
    "showBatteryStats=true",
    "showTankStats=true",
    "showLightStats=true"
};

        // Default CustomData
        List<string> defaultCustomDataPB = new List<string> {
    "solarPanelsCount=0",
    "outputMax=0.01",
    "docked=0",
    "dockingTime=" + DateTime.Now.ToOADate()
};

        // Error handling
        string error, warning;
        int errorCount = 0;
        int warningCount = 0;

        // Script timing variables
        DateTime lastRuntime = DateTime.Now;
        int execCounter = 1;
        bool firstRun = true;

        /// <summary>
        /// Pre-Run preparations
        /// </summary>
        public Program()
        {
            // Recalculate values
            lightLevel = (lightLevel % 100) / 100;
            dischargeLevel = (dischargeLevel % 100) / 100;

            // Set UpdateFrequency for starting the programmable block over and over again
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }


        /// <summary>
        /// Main method
        /// </summary>
        void Main()
        {
            // Script timing
            if ((DateTime.Now - lastRuntime).TotalMilliseconds < 200)
            {
                return;
            }
            else
            {
                lastRuntime = DateTime.Now;
            }

            // Init
            if (firstRun)
            {
                Echo(CreateBarString(1, 26, "Initializing..", execCounter, 20));
                if (execCounter == 1) GetBlocks();
                if (execCounter >= 2) Echo("Found: " + batteries.Count + " batteries");
                if (execCounter >= 3 && fillTanks) Echo("Found: " + allTanks.Count + " tanks");
                if (execCounter >= 4 && enableEmergencyReactors) Echo("Found: " + reactors.Count + " reactors");
                if (execCounter >= 5 && lightControl) Echo("Found: " + solarPanels.Count + " solar panels");
                if (execCounter >= 6 && lightControl) Echo("Found: " + lights.Count + " interior lights");
                if (execCounter >= 7 && lightControl) Echo("Found: " + spotlights.Count + " spotlights");
                if (execCounter >= 8) Echo("Found: " + connectors.Count + " connectors");
                if (execCounter >= 9) Echo("Found: " + lcds.Count + " LCDs");
                if (execCounter >= 10) Echo("\nStarting script..");

                execCounter += 1;
                if (execCounter >= 20)
                {
                    execCounter = 1;
                    firstRun = false;
                }

                return;
            }

            // Get all blocks, the script should use
            GetBlocks();

            if (error == null)
            {
                // Get the output of all blocks
                GetOutput();

                // Manage batteries, tanks and reactors
                ManageBlocks();

                // Trigger timer
                if (correctTimers && execCounter == 4)
                {
                    TriggerTimerBlock();
                }

                // Automatic Light Control
                if (lightControl && execCounter == 5)
                {
                    checkLightLevel();
                }
            }

            // Write the information to various channels
            Echo(CreateInformation(true));
            WriteLCD();

            // Update the script execution counter
            if (execCounter >= 5)
            {
                // Reset the counter
                execCounter = 1;

                // Reset errors and warnings if none were counted
                if (errorCount == 0)
                {
                    error = null;
                }

                if (warningCount == 0)
                {
                    warning = null;
                }
            }
            else
            {
                execCounter++;
            }

            // Update the working counter for the LCDs
            if (workingCounter >= 3)
            {
                workingCounter = 0;
            }
            else
            {
                workingCounter++;
            }
        }


        /// <summary>
        /// Gets all blocks that should be used by the script
        /// </summary>
        void GetBlocks()
        {
            // Get base grid of the PB
            if (baseGrid == null)
            {
                GetBaseGrid(Me.CubeGrid);
                GetConnectedGrids(baseGrid);
            }

            // Get connected grids every start of the script cycle
            if (execCounter == 1)
            {
                GetConnectedGrids(baseGrid, true);
            }

            // LCDs
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, l => connectedGrids.Contains(l.CubeGrid) && l.CustomName.Contains(mainLCDKeyword));

            // Get connectors
            if (connectorGroupName != "")
            {
                var connectorGroup = GridTerminalSystem.GetBlockGroupWithName(connectorGroupName);
                if (connectorGroup != null)
                {
                    connectorGroup.GetBlocksOfType<IMyShipConnector>(connectors);
                }
                else
                {
                    CreateError("Connector group not found:\n'" + connectorGroupName + "'");
                    return;
                }

                // Else add all connectors on the connected grids
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, c => connectedGrids.Contains(c.CubeGrid));
            }

            // Get batteries
            List<IMyBatteryBlock> batteriesTemp = new List<IMyBatteryBlock>();
            if (batteryGroupName != "")
            {
                var batteryGroup = GridTerminalSystem.GetBlockGroupWithName(batteryGroupName);
                if (batteryGroup != null)
                {
                    batteryGroup.GetBlocksOfType<IMyBatteryBlock>(batteriesTemp);
                }
                else
                {
                    CreateError("Battery group not found:\n'" + batteryGroupName + "'");
                    return;
                }

                // Else add all batteries on the connected grids
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteriesTemp, b => connectedGrids.Contains(b.CubeGrid));
            }

            // Order the batteries from lowest to highest charge
            TimeSpan balanceTimeSpan = DateTime.Now - balanceTime;
            if (balanceTimeSpan.TotalSeconds >= 10 || batteries.Count != batteriesTemp.Count)
            {
                batteries = batteriesTemp;

                if (batteries.Count >= 2 && storedPowerAll < storedPowerAllMax * 0.999)
                {
                    batteries.Sort((a, b) => a.CurrentStoredPower.CompareTo(b.CurrentStoredPower));
                }
                balanceTime = DateTime.Now;
            }

            // Get tanks
            if (fillTanks)
            {
                GridTerminalSystem.GetBlocksOfType<IMyGasTank>(oxygenTanks, t => connectedGrids.Contains(t.CubeGrid) && !t.BlockDefinition.SubtypeId.Contains("Hydrogen"));
                GridTerminalSystem.GetBlocksOfType<IMyGasTank>(hydrogenTanks, t => connectedGrids.Contains(t.CubeGrid) && t.BlockDefinition.SubtypeId.Contains("Hydrogen"));

                allTanks.Clear();
                allTanks.AddRange(oxygenTanks);
                allTanks.AddRange(hydrogenTanks);
            }

            // Get timer blocks
            foreach (var timer in timers)
            {
                correctTimers = true;
                var timerCheck = GridTerminalSystem.GetBlockWithName(timer) as IMyTimerBlock;
                if (timerCheck == null)
                {
                    CreateWarning("Timer block not found:\n'" + timer + "'\nTimer functions disabled!");
                    correctTimers = false;
                    break;
                }
            }

            // Get solar panels and lights
            if (lightControl)
            {
                // Solar Panels
                solarPanels.Clear();

                if (solarPanelGroups.Length > 0)
                {
                    foreach (var group in solarPanelGroups)
                    {
                        var panelGroup = GridTerminalSystem.GetBlockGroupWithName(group);
                        if (panelGroup != null)
                        {
                            var tempPanels = new List<IMySolarPanel>();
                            panelGroup.GetBlocksOfType<IMySolarPanel>(tempPanels);
                            solarPanels.AddRange(tempPanels);
                        }
                        else
                        {
                            CreateWarning("Solar panel group not found:\n'" + group + "'");
                        }
                    }
                }
                else
                {
                    GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(solarPanels, s => connectedGrids.Contains(s.CubeGrid));
                    if (solarPanels.Count == 0)
                    {
                        CreateWarning("No solar panel found!\nAdd one to use the light control!");
                    }
                }

                // Lights
                lights.Clear();
                spotlights.Clear();

                if (lightGroups.Length > 0)
                {
                    var tempLights = new List<IMyInteriorLight>();
                    var tempSpotlights = new List<IMyReflectorLight>();
                    foreach (var group in lightGroups)
                    {
                        var lightGroup = GridTerminalSystem.GetBlockGroupWithName(group);
                        if (lightGroup != null)
                        {
                            lightGroup.GetBlocksOfType<IMyInteriorLight>(tempLights);
                            lights.AddRange(tempLights);
                            lightGroup.GetBlocksOfType<IMyReflectorLight>(tempSpotlights);
                            spotlights.AddRange(tempSpotlights);
                        }
                        else
                        {
                            CreateWarning("Light group not found:\n'" + group + "'");
                        }
                    }

                    // Else search for all interior lights and spotlights and fill the groups
                }
                else
                {
                    GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(lights, l => connectedGrids.Contains(l.CubeGrid));
                    GridTerminalSystem.GetBlocksOfType<IMyReflectorLight>(spotlights, l => connectedGrids.Contains(baseGrid));
                }
            }
            654
            // Get reactors
            if (enableEmergencyReactors)
            {
                GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors, r => connectedGrids.Contains(r.CubeGrid));
            }

            // Get Cockpits
            GridTerminalSystem.GetBlocksOfType<IMyShipController>(cockpits, c => connectedGrids.Contains(c.CubeGrid));
        }


        void GetBaseGrid(IMyCubeGrid currentGrid)
        {
            List<IMyMotorStator> scanRotors = new List<IMyMotorStator>();
            List<IMyPistonBase> scanPistons = new List<IMyPistonBase>();
            GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(scanRotors, r => r.TopGrid == currentGrid);
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(scanPistons, p => p.TopGrid == currentGrid);

            if (scanRotors.Count == 0 && scanPistons.Count == 0)
            {
                baseGrid = currentGrid;
            }
            else
            {
                foreach (var rotor in scanRotors)
                {
                    GetBaseGrid(rotor.CubeGrid);
                    if (baseGrid != null) return;
                }

                foreach (var piston in scanPistons)
                {
                    GetBaseGrid(piston.CubeGrid);
                    if (baseGrid != null) return;
                }
            }
        }


        void GetConnectedGrids(IMyCubeGrid currentGrid, bool clearGridList = false)
        {
            if (clearGridList) connectedGrids.Clear();

            connectedGrids.Add(currentGrid);

            List<IMyMotorStator> scanRotors = new List<IMyMotorStator>();
            List<IMyPistonBase> scanPistons = new List<IMyPistonBase>();
            GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(scanRotors, r => r.CubeGrid == currentGrid);
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(scanPistons, p => p.CubeGrid == currentGrid);

            foreach (var rotor in scanRotors)
            {
                if (!connectedGrids.Contains(rotor.TopGrid))
                {
                    GetConnectedGrids(rotor.TopGrid);
                }
            }

            foreach (var piston in scanPistons)
            {
                if (!connectedGrids.Contains(piston.TopGrid))
                {
                    GetConnectedGrids(piston.TopGrid);
                }
            }
        }


        /// <summary>
        /// Gets the output of all used blocks
        /// </summary>
        void GetOutput()
        {
            // Batteries
            storedPowerAll = 0;
            storedPowerAllMax = 0;
            currentInputAll = 0;
            maxInputAll = 0;
            currentOutputAll = 0;
            maxOutputAll = 0;

            // Check all batteries
            foreach (var battery in batteries)
            {
                float storedPower = battery.CurrentStoredPower;
                float storedPowerMax = battery.MaxStoredPower;

                storedPowerAll += storedPower;
                storedPowerAllMax += storedPowerMax;
                currentInputAll += battery.CurrentInput;
                maxInputAll += battery.MaxInput;
                currentOutputAll += battery.CurrentOutput;
                maxOutputAll += battery.MaxOutput;
            }

            // Power time
            if (docked && currentInputAll > 0)
            {
                powerTime = TimeSpan.FromHours(((storedPowerAllMax - storedPowerAll) / currentInputAll));
            }
            else if (!docked && currentOutputAll > 0)
            {
                powerTime = TimeSpan.FromHours((storedPowerAll / currentOutputAll));
            }
            else
            {
                powerTime = TimeSpan.FromHours(0);
            }


            // Tanks
            if (fillTanks)
            {
                oxygenTankCapacity = 0;
                oxygenTankFillLevel = 0;
                hydrogenTankCapacity = 0;
                hydrogenTankFillLevel = 0;

                foreach (var tank in oxygenTanks)
                {
                    oxygenTankCapacity += tank.Capacity;
                    oxygenTankFillLevel += tank.Capacity * tank.FilledRatio;
                }

                foreach (var tank in hydrogenTanks)
                {
                    hydrogenTankCapacity += tank.Capacity;
                    hydrogenTankFillLevel += tank.Capacity * tank.FilledRatio;
                }
            }

            // Solar panels
            if (lightControl)
            {
                solarOutput = 0;
                solarOutputMax = ReadCustomDataPB("outputMax");

                // If the solar panel count is not equal to the saved amount, reset the highest output and write new solar panel count
                if (solarPanels.Count != ReadCustomDataPB("solarPanelsCount"))
                {
                    WriteCustomDataPB("solarPanelsCount", solarPanels.Count);
                    WriteCustomDataPB("outputMax", 0);
                }

                // Add the current max output of all panels
                foreach (var solarPanel in solarPanels)
                {
                    solarOutput += solarPanel.MaxOutput;

                    // Terminal solar stats
                    if (showSolarStats && enableTerminalStatistics)
                    {
                        double maxPanelOutput = 0;
                        double.TryParse(solarPanel.CustomData, out maxPanelOutput);

                        if (maxPanelOutput < solarPanel.MaxOutput)
                        {
                            maxPanelOutput = solarPanel.MaxOutput;
                            solarPanel.CustomData = maxPanelOutput.ToString();
                        }

                        AddStatusToName(solarPanel, true, "", solarPanel.MaxOutput, maxPanelOutput);
                    }
                }

                // If the output is higher than the saved output, save it
                if (solarOutput > solarOutputMax)
                {
                    solarOutputMax = solarOutput;
                    WriteCustomDataPB("outputMax", solarOutputMax);
                }
            }

            // Cockpits
            underControl = false;
            foreach (var cockpit in cockpits)
            {
                if (cockpit.IsUnderControl || keepOneBackupBattery) underControl = true;
            }
        }


        /// <summary>
        /// Manages batteries and tanks based on connection
        /// </summary>
        void ManageBlocks()
        {
            // Check if a connector is connected
            foreach (var connector in connectors)
            {
                if (!docked && connector.Status == MyShipConnectorStatus.Connected)
                {
                    docked = true;
                    dockedConnector = connector;
                    dockedTo = "Normal Connector";

                    refuel = true;
                    ignore = false;
                    recharge = true;
                    discharge = false;
                    refill = true;
                    oxygenRefill = true;
                    hydrogenRefill = true;

                    if (ReadCustomDataPB("docked") == 1)
                    {
                        dockingTime = DateTime.FromOADate(ReadCustomDataPB("dockingTime"));
                    }
                    else
                    {
                        dockingTime = DateTime.Now;
                        WriteCustomDataPB("dockingTime", dockingTime.ToOADate());
                    }

                    // Connected to special conntectors?
                    string otherConnector = connector.OtherConnector.CustomName;
                    List<string> dockedToSpecial = new List<string>();

                    if (otherConnector.Contains(noRefuelKeyword))
                    {
                        dockedToSpecial.Add("No Refuel");
                        refuel = false;
                    }
                    if (otherConnector.Contains(ignoreKeyword))
                    {
                        dockedToSpecial.Add("Ignored");
                        ignore = true;
                    }
                    if (otherConnector.Contains(noRechargeKeyword))
                    {
                        dockedToSpecial.Add("No Recharge");
                        recharge = false;
                    }
                    if (otherConnector.Contains(dischargeKeyword))
                    {
                        dockedToSpecial.Add("Discharge");
                        discharge = true;
                        recharge = false;
                    }
                    if (otherConnector.Contains(noTankFillKeyword))
                    {
                        dockedToSpecial.Add("No Refill");
                        refill = false;
                    }
                    if (otherConnector.Contains(noOxygenTankFillKeyword))
                    {
                        dockedToSpecial.Add("No O2 Refill");
                        oxygenRefill = false;
                    }
                    if (otherConnector.Contains(noHydrogenTankFillKeyword))
                    {
                        dockedToSpecial.Add("No H2 Refill");
                        hydrogenRefill = false;
                    }

                    if (dockedToSpecial.Count > 0)
                    {
                        dockedTo = String.Join(", ", dockedToSpecial) + " Connector";
                    }

                    // Execute timer on connect?
                    if (events.Length != 0 && ReadCustomDataPB("docked") == 0) TriggerTimerBlock("dock");

                    // Update custom data
                    WriteCustomDataPB("docked", 1);

                    break;
                }
            }

            // Check for disconnection of a former connected connector
            try
            {
                if (docked && dockedConnector.Status != MyShipConnectorStatus.Connected)
                {
                    docked = false;
                    dockingTime = DateTime.Now;

                    // Execute time on disconnect?
                    if (events.Length != 0 && ReadCustomDataPB("docked") == 1) TriggerTimerBlock("undock");

                    // Update custom data
                    WriteCustomDataPB("docked", 0);
                    WriteCustomDataPB("dockingTime", dockingTime.ToOADate());
                }
            }
            catch
            {
                docked = false;

                // Update custom data
                WriteCustomDataPB("docked", 0);
            }

            // Manage batteries
            if (dockingTime == DateTime.MinValue) dockingTime = DateTime.FromOADate(ReadCustomDataPB("dockingTime"));
            timeSinceDock = DateTime.Now - dockingTime;

            string status = "";
            string damagedBattery = null;

            foreach (var battery in batteries)
            {
                // Check damage for emercengy reactor
                if (!battery.CubeGrid.GetCubeBlock(battery.Position).IsFullIntegrity)
                {
                    damagedBattery = battery.CustomName + " is damaged!";
                }

                string io = ((double)battery.CurrentInput - battery.CurrentOutput).PowerString();
                double storedPower = battery.CurrentStoredPower;
                double storedPowerMax = battery.MaxStoredPower;
                battery.SemiautoEnabled = false;

                if (docked)
                {
                    if (ignore)
                    {
                        // do nothing
                    }
                    else if ((!refuel || !recharge) && !discharge)
                    {
                        // If the ship is docked to a noRefuelConnector or a noRechargeConnector
                        battery.OnlyRecharge = false;
                        battery.OnlyDischarge = false;

                        // The last battery will be a backup battery if ship is under control (highest charge)
                        if (underControl && battery == batteries[batteries.Count - 1])
                        {
                            status = "Backup @ " + io;
                            battery.Enabled = true;
                        }
                        else
                        {
                            status = "No Recharge";
                            battery.Enabled = false;
                        }
                    }
                    else if (recharge)
                    {
                        // If the ship is docked to a normal connector
                        battery.Enabled = true;
                        battery.OnlyDischarge = false;

                        // The last battery will be a backup battery if ship is under control (highest charge)
                        if (underControl && battery == batteries[batteries.Count - 1])
                        {
                            status = "Backup @ " + io;
                            battery.OnlyRecharge = false;
                        }
                        else if (storedPower >= storedPowerMax * 0.999)
                        {
                            status = "In/Out @ " + io;
                            battery.OnlyRecharge = false;
                        }
                        else
                        {
                            status = "Recharging @ " + io;
                            battery.OnlyRecharge = true;
                        }
                    }
                    else if (discharge)
                    {
                        // If the ship is docked to a dischargeConnector
                        battery.Enabled = true;
                        battery.OnlyRecharge = false;

                        // Discharge if charge is not less than dischargeLevel
                        if (storedPower <= storedPowerMax * dischargeLevel)
                        {
                            status = "Low Charge!";
                            battery.OnlyDischarge = false;
                        }
                        else
                        {
                            status = "Discharging @ " + io;
                            battery.OnlyDischarge = true;
                        }
                    }
                }
                else
                {
                    // If the ship is not docked, disable recharge
                    battery.Enabled = true;
                    battery.OnlyRecharge = false;

                    if (dischargeWhenUndocked && lastActivation != "lowCharge")
                    {
                        status = "Discharging @ " + io;
                        battery.OnlyDischarge = true;
                    }
                    else
                    {
                        status = "In/Out @ " + io;
                        battery.OnlyDischarge = false;
                    }
                }

                // Add the status to the battery name
                if (showBatteryStats && enableTerminalStatistics)
                {
                    AddStatusToName(battery, true, status, storedPower, storedPowerMax);
                }
            }

            // Manage tanks
            if (fillTanks)
            {
                foreach (var tank in allTanks)
                {
                    double tankCapacity = tank.Capacity;
                    double tankFillLevel = tank.Capacity * tank.FilledRatio;

                    if (docked)
                    {
                        if (ignore)
                        {
                            // do nothing
                        }
                        else if (!refuel || !refill)
                        {
                            // If the ship is docked to a noRefuelConnector or a noRefillConnector
                            status = "No Refill";
                            tank.Enabled = false;
                        }
                        else if (!oxygenRefill && !tank.BlockDefinition.SubtypeId.Contains("Hydrogen"))
                        {
                            // If the ship is docked to a noOxygenTankFillConnector
                            status = "No O2 Refill";
                            tank.Enabled = false;
                        }
                        else if (!hydrogenRefill && tank.BlockDefinition.SubtypeId.Contains("Hydrogen"))
                        {
                            // If the ship is docked to a noHydrogenTankFillConnector
                            status = "No H2 Refill";
                            tank.Enabled = false;
                        }
                        else if (tank.FilledRatio >= 0.999)
                        {
                            // If the tank is full
                            status = "";
                            tank.Stockpile = false;
                        }
                        else
                        {
                            status = "Refilling";
                            tank.Stockpile = true;
                        }
                    }
                    else
                    {
                        // If the ship is not docked, disable stockpile
                        status = "";
                        tank.Enabled = true;
                        tank.Stockpile = false;
                    }

                    // Add the status to the tank name
                    if (showTankStats && enableTerminalStatistics)
                    {
                        AddStatusToName(tank, true, status, tankFillLevel, tankCapacity);
                    }
                }
            }

            // Manage reactors
            if (enableEmergencyReactors && reactors.Count > 0)
            {
                double lowChargeOn = lowChargePercentageON % 100 / 100;
                double lowChargeOff = lowChargePercentageOFF % 100 / 100;
                double overload = overloadPercentage % 100 / 100;

                // Enable on low battery charg
                if (lastActivation == "lowCharge" || lastActivation == "")
                {
                    if (activateOnLowBattery && storedPowerAll < storedPowerAllMax * lowChargeOn)
                    {
                        enableReactors = true;
                        lastActivation = "lowCharge";
                        enableReactorReason = "LOW CHARGE!";
                    }
                    else if (activateOnLowBattery && storedPowerAll > storedPowerAllMax * lowChargeOff)
                    {
                        enableReactors = false;
                        lastActivation = "";
                    }
                }

                // Activate on overload
                if (lastActivation == "overload" || lastActivation == "")
                {
                    if (activateOnOverload && currentOutputAll > maxOutputAll * overload && !docked)
                    {
                        enableReactors = true;
                        lastActivation = "overload";
                        enableReactorReason = "OVERLOAD!";
                    }
                    else
                    {
                        enableReactors = false;
                        lastActivation = "";
                    }
                }

                // Activate on damaged batteries
                if (lastActivation == "damage" || lastActivation == "")
                {
                    if (activateOnDamagedBatteries && damagedBattery != null)
                    {
                        enableReactors = true;
                        lastActivation = "damage";
                        enableReactorReason = damagedBattery;
                    }
                    else
                    {
                        enableReactors = false;
                        lastActivation = "";
                    }
                }

                // Activate on no batteries
                if (lastActivation == "noBat" || lastActivation == "")
                {
                    if (activateOnNoBatteries && batteries.Count == 0)
                    {
                        enableReactors = true;
                        lastActivation = "noBat";
                        enableReactorReason = "NO BATTERIES!";
                    }
                    else
                    {
                        enableReactors = false;
                        lastActivation = "";
                    }
                }

                foreach (var reactor in reactors)
                {
                    if (enableReactors)
                    {
                        reactor.Enabled = true;
                        status = "Online";
                    }
                    else
                    {
                        reactor.Enabled = false;
                        status = "Standby";
                    }

                    // Add the status to the reactor name
                    if (showReactorStats && enableTerminalStatistics)
                    {
                        AddStatusToName(reactor, true, status, reactor.CurrentOutput, reactor.MaxOutput);
                    }
                }
            }
        }


        /// <summary>
        /// Trigger a timer block based on a set of different events
        /// </summary>
        void TriggerTimerBlock(string triggerEvent = "")
        {
            // Error management
            if (!correctTimers) return;

            if (events.Length == 0)
            {
                CreateWarning("No events for triggering specified!");
                return;
            }

            if (timers.Length == 0)
            {
                CreateWarning("No timers for triggering specified!");
                return;
            }

            if (events.Length != timers.Length)
            {
                CreateWarning("Every event needs a timer block name!\nFound " + events.Length + " events and " + timers.Length + " timers.");
                return;
            }

            int timerToTrigger = -1;

            // If no event was specified, check if battery level or docking time events apply
            if (triggerEvent == "")
            {
                for (int i = 0; i <= events.Length - 1; i++)
                {
                    if (events[i] == triggerEvent)
                    {
                        timerToTrigger = i;
                    }
                }
            }

            // Cycle through each entry in events and to get the corresponding timer
            for (int i = 0; i <= events.Length - 1; i++)
            {
                string currentEvent = events[i].ToLower();
                currentEvent = currentEvent.Replace(" ", "");

                // If triggerEvent was empty, see, if the currentEvent contains a percentage or a minute value
                if (triggerEvent == "")
                {
                    if (currentEvent == storedPowerAll.PercentOf(storedPowerAllMax))
                    {
                        triggerEvent = currentEvent;
                    }

                    if (docked && currentEvent == timeSinceDock.ToString(@"m\:ss"))
                    {
                        triggerEvent = currentEvent;
                    }
                }

                // Find the timer if the conditions match
                if (currentEvent == triggerEvent)
                {
                    timerToTrigger = i;
                    break;
                }
            }

            // Trigger the timer block if a event matches the current conditions
            if (timerToTrigger >= 0 && triggerEvent != lastEvent)
            {
                // Find the timer block
                var timer = GridTerminalSystem.GetBlockWithName(timers[timerToTrigger]) as IMyTimerBlock;

                if (timer != null)
                {
                    timer.Enabled = true;
                    if (timer.TriggerDelay == 1)
                    {
                        timer.Trigger();
                    }
                    else
                    {
                        timer.StartCountdown();
                    }
                }
            }

            lastEvent = triggerEvent;
        }


        /// <summary>
        /// Checks the light level in order to activate or deactivate the lights
        /// </summary>
        void checkLightLevel()
        {
            bool enableLight = true;

            // Check the solar panels
            if (solarOutput > solarOutputMax * lightLevel)
            {
                enableLight = false;
            }

            // Toggle all interior lights based on light level
            foreach (var light in lights)
            {
                light.Enabled = enableLight;
            }

            // Toggle all spotlights based on light level
            foreach (var spotLight in spotlights)
            {
                spotLight.Enabled = enableLight;
            }
        }


        /// <summary>
        /// Adds the current percentage and status at the end of the name
        /// </summary>
        /// <param name="block">Block to add status to as IMyTerminalBlock</param>
        /// <param name="addStatus">True adds a status, False removes it</param>
        /// <param name="status">Status as string</param>
        /// <param name="currentValue">Current value as double</param>
        /// <param name="maxValue">Max value as double</param>
        void AddStatusToName(IMyTerminalBlock block, bool addStatus = true, string status = "", double currentValue = 0, double maxValue = 0)
        {
            string newName = block.CustomName;
            string oldStatus = System.Text.RegularExpressions.Regex.Match(block.CustomName, @" *\(\d+\.*\d*%.*\)").Value;
            if (oldStatus != String.Empty)
            {
                newName = block.CustomName.Replace(oldStatus, "");
            }

            if (addStatus)
            {
                // Add percentages
                newName += " (" + currentValue.PercentOf(maxValue);

                // Add status
                if (status != "")
                {
                    newName += ", " + status;
                }

                // Add closing bracket
                newName += ")";
            }

            // Rename the block if the name has changed
            if (newName != block.CustomName)
            {
                block.CustomName = newName;
            }
        }


        /// <summary>
        /// Removes all terminal statistics
        /// </summary>
        void RemoveTerminalStatistics()
        {
            // Solar Panels
            foreach (var solarPanel in solarPanels)
            {
                solarPanel.CustomData = "";
                AddStatusToName(solarPanel, false);
            }

            // Batteries
            foreach (var battery in batteries)
            {
                AddStatusToName(battery, false);
            }

            // Tanks
            foreach (var tank in allTanks)
            {
                AddStatusToName(tank, false);
            }

            // Reactors
            foreach (var reactor in reactors)
            {
                AddStatusToName(reactor, false);
            }
        }


        /// <summary>
        /// Create the information string for terminal and LCD output
        /// </summary>
        string CreateInformation(bool terminal = false, float fontSize = 0.65f, int charsPerline = 26, bool showConnectionStatus = true, bool showPowerTime = true, bool showEmergencyReactor = true, bool showBatteryStats = true, bool showTankStats = true, bool showLightStats = true)
        {
            bool infoShown = false;
            string info = "";

            switch (workingCounter % 4)
            {
                case 0: workingIndicator = "/"; break;
                case 1: workingIndicator = "-"; break;
                case 2: workingIndicator = "\\"; break;
                case 3: workingIndicator = "|"; break;
            }

            // Terminal / LCD information string
            info = "Isy's Ship Refueler " + workingIndicator + "\n";
            info += "=================";
            if (!terminal) info += "====";
            info += "\n\n";

            // If any error occurs, show it
            if (error != null)
            {
                info += "Error!\n";
                info += error + "\n\n";
                info += "Script stopped!\n\n";

                return info;
            }

            // Add warning message for minor errors
            if (warning != null)
            {
                info += "Warning!\n";
                info += warning + "\n\n";
                infoShown = true;
            }

            // Connection
            if (showConnectionStatus)
            {
                if (docked)
                {
                    info += "Docked to: " + dockedTo + "\n";
                    info += "Docked for: " + timeSinceDock.ToString(@"hh\:mm\:ss") + "\n";
                }
                else
                {
                    info += "Undocked for: " + timeSinceDock.ToString(@"hh\:mm\:ss") + "\n";
                }
                infoShown = true;
            }

            // Power time
            if (showPowerTime && batteries.Count > 0)
            {
                if (docked && powerTime.TotalSeconds > 1)
                {
                    info += "Batteries FULL in: " + powerTime.ToString(@"hh\:mm\:ss") + "\n";
                }
                else if (docked)
                {
                    info += "Batteries are FULL!\n";
                }
                else if (storedPowerAll <= 0.1)
                {
                    info += "Batteries are EMPTY!\n";
                }
                else
                {
                    info += "Batteries EMPTY in: " + powerTime.ToString(@"hh\:mm\:ss") + "\n";
                }
                infoShown = true;
            }

            // Reactors
            if (showEmergencyReactor && enableReactors)
            {
                if (infoShown) info += "\n";
                info += enableReactorReason + "\nReactors ACTIVATED!\n";
                infoShown = true;
            }

            if (infoShown) info += "\n";

            // Batteries
            if (showBatteryStats && batteries.Count > 0)
            {
                info += "Stats for " + batteries.Count + " Batteries:\n";
                info += CreateBarString(fontSize, charsPerline, "Input", currentInputAll, maxInputAll, currentInputAll.PowerString(), maxInputAll.PowerString());
                info += CreateBarString(fontSize, charsPerline, "Output", currentOutputAll, maxOutputAll, currentOutputAll.PowerString(), maxOutputAll.PowerString());
                info += CreateBarString(fontSize, charsPerline, "Charge", storedPowerAll, storedPowerAllMax, storedPowerAll.PowerString(true), storedPowerAllMax.PowerString(true));
                info += "\n";
                infoShown = true;
            }

            // Tanks
            if (showTankStats && allTanks.Count > 0)
            {
                info += "Stats for " + allTanks.Count + " Tanks:\n";
                if (oxygenTanks.Count > 0)
                {
                    info += CreateBarString(fontSize, charsPerline, oxygenTanks.Count + "x Oxygen", oxygenTankFillLevel, oxygenTankCapacity, oxygenTankFillLevel.VolumeString(), oxygenTankCapacity.VolumeString());
                }

                if (hydrogenTanks.Count > 0)
                {
                    info += CreateBarString(fontSize, charsPerline, hydrogenTanks.Count + "x Hydrogen", hydrogenTankFillLevel, hydrogenTankCapacity, hydrogenTankFillLevel.VolumeString(), hydrogenTankCapacity.VolumeString());
                }
                info += "\n";
                infoShown = true;
            }

            // Lights
            if (showLightStats && lightControl && solarPanels.Count > 0)
            {
                string threshold = "< ";
                string onOff = "ON";

                if (solarOutput > solarOutputMax * lightLevel)
                {
                    threshold = "> ";
                    onOff = "OFF";
                }

                info += CreateBarString(fontSize, charsPerline, "Light Level", solarOutput, solarOutputMax, threshold + (lightLevel * 100) + "%", onOff);
                infoShown = true;
            }

            if (!infoShown)
            {
                info += "-- No informations to show --";
            }

            return info;
        }


        /// <summary>
        /// Creates a string with two lines containing heading, two values, the percentage of the two and a level bar in the second row
        /// </summary>
        /// <param name="heading">Heading (top left) as string</param>
        /// <param name="value">Value as double</param>
        /// <param name="valueMax">Max value as double</param>
        /// <param name="valueStr">Optional: a string instead of the double value</param>
        /// <param name="valueMaxStr">Optional: a string instead of the double value</param>
        /// <returns>See summary</returns>
        string CreateBarString(double fontSize, int charsPerLine, string heading, double value, double valueMax, string valueStr = null, string valueMaxStr = null, bool noBar = false)
        {
            string current = value.ToString();
            string max = valueMax.ToString();

            if (valueStr != null)
            {
                current = valueStr;
            }

            if (valueMaxStr != null)
            {
                max = valueMaxStr;
            }

            string percent = value.PercentOf(valueMax);
            string values = current + " / " + max;
            double level = value / valueMax >= 1 ? 1 : value / valueMax;
            int lcdWidth = (int)(charsPerLine / fontSize);

            StringBuilder firstLine = new StringBuilder(heading + " ");
            StringBuilder secondLine = new StringBuilder();

            if (fontSize <= 0.6 || (fontSize <= 1 && charsPerLine == 52))
            {
                firstLine.Append(' '.Repeat(lcdWidth / 2 - (firstLine.Length + current.Length)));
                firstLine.Append(current + " / " + max);
                firstLine.Append(' '.Repeat(lcdWidth - (firstLine.Length + percent.Length)));
                firstLine.Append(percent + "\n");

                if (!noBar)
                {
                    secondLine = new StringBuilder("[" + '.'.Repeat(lcdWidth - 2) + "]\n");
                    int fillLevel = (int)Math.Ceiling((lcdWidth - 2) * level);
                    try
                    {
                        secondLine.Replace(".", "I", 1, fillLevel);
                    }
                    catch { }
                }
            }
            else
            {
                firstLine.Append(' '.Repeat(lcdWidth - (firstLine.Length + values.Length)));
                firstLine.Append(values + "\n");

                if (!noBar)
                {
                    secondLine = new StringBuilder("[" + '.'.Repeat(lcdWidth - 8) + "]");
                    secondLine.Append(' '.Repeat(lcdWidth - (secondLine.Length + percent.Length)));
                    secondLine.Append(percent + "\n");

                    int fillLevel = (int)Math.Ceiling((lcdWidth - 8) * level);
                    try
                    {
                        secondLine.Replace(".", "I", 1, fillLevel);
                    }
                    catch { }
                }
            }

            return firstLine.Append(secondLine).ToString();
        }


        /// <summary>
        /// Creates a scrolling text for an LCD panel
        /// </summary>
        /// <param name="text">Text to display as string</param>
        /// <param name="lcd">LCD that should use the text as IMyTextPanel (this is just for saving the current scrolling)</param>
        /// <returns>Scrolled substring of the input text as string</returns>
        string CreateScrollingText(float fontSize, string text, IMyTextPanel lcd, int headingHeight = 3)
        {
            // Get the LCD EntityId
            long id = lcd.EntityId;

            // Create default entry for the LCD in the dictionary
            if (!scroll.ContainsKey(id))
            {
                scroll[id] = new List<int> { 1, 3, headingHeight, 0 };
            }

            int scrollDirection = scroll[id][0];
            int scrollWait = scroll[id][1];
            int lineStart = scroll[id][2];
            int scrollSecond = scroll[id][3];

            // Figure out the amount of lines for scrolling content
            var linesTemp = text.TrimEnd('\n').Split('\n');
            List<string> lines = new List<string>();
            int lcdHeight = (int)Math.Ceiling(17 / fontSize);
            int lcdWidth = (int)(26 / fontSize);
            string lcdText = "";

            // Adjust height for corner LCDs
            if (lcd.BlockDefinition.SubtypeName.Contains("Corner"))
            {
                if (lcd.CubeGrid.GridSize == 0.5)
                {
                    lcdHeight = (int)Math.Floor(5 / fontSize);
                }
                else
                {
                    lcdHeight = (int)Math.Floor(3 / fontSize);
                }
            }

            // Adjust width for wide LCDs
            if (lcd.BlockDefinition.SubtypeName.Contains("Wide"))
            {
                lcdWidth = (int)(52 / fontSize);
            }

            // Build the lines list out of lineTemp and add line breaks if text is too long for one line
            foreach (var line in linesTemp)
            {
                if (line.Length <= lcdWidth)
                {
                    lines.Add(line);
                }
                else
                {
                    try
                    {
                        string currentLine = "";
                        var words = line.Split(' ');

                        foreach (var word in words)
                        {
                            if ((currentLine + word).Length > lcdWidth)
                            {
                                lines.Add(currentLine);
                                currentLine = word + " ";
                            }
                            else
                            {
                                currentLine += word + " ";
                            }
                        }

                        lines.Add(currentLine);
                    }
                    catch
                    {
                        lines.Add(line);
                    }
                }
            }

            if (lines.Count > lcdHeight)
            {
                if (DateTime.Now.Second != scrollSecond)
                {
                    scrollSecond = DateTime.Now.Second;
                    if (scrollWait > 0) scrollWait--;
                    if (scrollWait <= 0) lineStart += scrollDirection;

                    if (lineStart + lcdHeight - headingHeight >= lines.Count && scrollWait <= 0)
                    {
                        scrollDirection = -1;
                        scrollWait = 3;
                    }
                    if (lineStart <= headingHeight && scrollWait <= 0)
                    {
                        scrollDirection = 1;
                        scrollWait = 3;
                    }
                }
            }
            else
            {
                lineStart = headingHeight;
                scrollDirection = 1;
                scrollWait = 3;
            }

            // Save the current scrolling in the dictionary
            scroll[id][0] = scrollDirection;
            scroll[id][1] = scrollWait;
            scroll[id][2] = lineStart;
            scroll[id][3] = scrollSecond;

            // Always create header
            for (var line = 0; line < headingHeight; line++)
            {
                lcdText += lines[line] + "\n";
            }

            // Create scrolling content based on the starting line
            for (var line = lineStart; line < lines.Count; line++)
            {
                lcdText += lines[line] + "\n";
            }

            return lcdText;
        }


        /// <summary>
        /// Write the informationsString on all specified LCDs
        /// </summary>
        void WriteLCD()
        {
            if (lcds.Count == 0) return;

            foreach (var lcd in lcds)
            {
                // Get the wanted statistics to show
                bool addConnectionStatus = ReadCustomDataLCD(lcd, "showConnectionStatus");
                bool addPowerTime = ReadCustomDataLCD(lcd, "showPowerTime");
                bool addEmergencyReactor = ReadCustomDataLCD(lcd, "showEmergencyReactor");
                bool addBatteryStats = ReadCustomDataLCD(lcd, "showBatteryStats");
                bool addTankStats = ReadCustomDataLCD(lcd, "showTankStats");
                bool addLightStats = ReadCustomDataLCD(lcd, "showLightStats");

                // Get the font size
                float fontSize = lcd.FontSize;
                int charsPerline = 26;
                if (lcd.BlockDefinition.SubtypeName.Contains("Wide")) charsPerline = 52;

                // Create the text
                string info = CreateInformation(false, fontSize, charsPerline, addConnectionStatus, addPowerTime, addEmergencyReactor, addBatteryStats, addTankStats, addLightStats);
                string lcdText = CreateScrollingText(fontSize, info, lcd);

                // Print contents to its public text
                lcd.WritePublicTitle("Isy's Ship Refueler");
                lcd.WritePublicText(lcdText, false);
                lcd.Font = "Monospace";
                lcd.ShowPublicTextOnScreen();
            }
        }


        double ReadCustomDataPB(string field)
        {
            CheckCustomDataPB();
            var customData = Me.CustomData.Split('\n').ToList();

            // Find entry
            int index = customData.FindIndex(i => i.Contains(field + "="));

            // Return value of entry if index was found
            if (index > -1)
            {
                return Convert.ToDouble(customData[index].Replace(field + "=", ""));
            }

            return 0;
        }


        void WriteCustomDataPB(string field, double value)
        {
            CheckCustomDataPB();
            var customData = Me.CustomData.Split('\n').ToList();

            // Find entry
            int index = customData.FindIndex(i => i.Contains(field + "="));

            // Write new entry if index was found
            if (index > -1)
            {
                customData[index] = field + "=" + value;
                Me.CustomData = String.Join("\n", customData);
            }
        }


        void CheckCustomDataPB()
        {
            var customData = Me.CustomData.Split('\n').ToList();

            // Create new default customData if a too short one is found
            if (customData.Count != defaultCustomDataPB.Count)
            {
                Me.CustomData = String.Join("\n", defaultCustomDataPB);
            }
        }


        bool ReadCustomDataLCD(IMyTextPanel lcd, string field)
        {
            CheckCustomDataLCD(lcd);
            var customData = lcd.CustomData.Split('\n').ToList();

            // Find entry
            int index = customData.FindIndex(i => i.Contains(field + "="));

            // Return value of entry if index was found
            if (index > -1)
            {
                try
                {
                    return Convert.ToBoolean(customData[index].Replace(field + "=", ""));
                }
                catch
                {
                    return true;
                }
            }

            return true;
        }


        void CheckCustomDataLCD(IMyTextPanel lcd)
        {
            var customData = lcd.CustomData.Split('\n').ToList();

            // Create new default customData if a too short one is found
            if (customData.Count != defaultCustomDataLCD.Count)
            {
                lcd.CustomData = String.Join("\n", defaultCustomDataLCD);
                lcd.FontSize = 0.8f;
            }
        }


        /// <summary>
        /// Creates an error with the given text and stops all rotors or gyros
        /// </summary>
        /// <param name="text">Errortext as string</param>
        void CreateError(string text)
        {
            if (error == null) error = text;
            errorCount++;
        }


        /// <summary>
        /// Creates a warning with the given text
        /// </summary>
        /// <param name="text">Warningtext as string</param>
        void CreateWarning(string text)
        {
            if (warning == null) warning = text;
            warningCount++;
        }


        /// <summary>
        /// Save method for recompiling the script or saving the world
        /// </summary>
        public void Save()
        {
            // Activate everything
            foreach (var battery in batteries)
            {
                battery.Enabled = true;
                battery.OnlyRecharge = false;
                battery.OnlyDischarge = false;
            }

            foreach (var reactor in reactors)
            {
                reactor.Enabled = true;
            }

            foreach (var tank in allTanks)
            {
                tank.Stockpile = false;
            }

            RemoveTerminalStatistics();
        }

    }

    public static class Extensions
    {
        /// <summary>
        /// Create a power string out of a double value
        /// </summary>
        /// <param name="value">Any double value</param>
        /// <param name="wattHours">Optional: true if you want "MWh" instead of "MW"</param>
        /// <returns>String like "5 MW"</returns>
        public static string PowerString(this double value, bool wattHours = false)
        {
            string minus = value < 0 ? "-" : "";
            value = Math.Abs(value);
            string unit = "MW";

            if (value < 1)
            {
                value *= 1000;
                unit = "kW";
            }
            else if (value >= 1000 && value < 1000000)
            {
                value /= 1000;
                unit = "GW";
            }
            else if (value >= 1000000 && value < 1000000000)
            {
                value /= 1000000;
                unit = "TW";
            }
            else if (value >= 1000000000)
            {
                value /= 1000000000;
                unit = "PW";
            }

            if (wattHours) unit += "h";

            return minus + Math.Round(value, 1) + " " + unit;
        }


        /// <summary>
        /// Returns a string of a tank volume with the correct unit (L, kL, ML, ...)
        /// </summary>
        /// <param name="value">Tank volume as double</param>
        /// <returns>A string like "100 L"</returns>
        public static string VolumeString(this double value)
        {
            string unit = "L";

            if (value >= 1000 && value < 1000000)
            {
                value /= 1000;
                unit = "KL";
            }
            else if (value >= 1000000 && value < 1000000000)
            {
                value /= 1000000;
                unit = "ML";
            }
            else if (value >= 1000000000)
            {
                value /= 1000000000;
                unit = "BL";
            }
            else if (value >= 1000000000000)
            {
                value /= 1000000000000;
                unit = "TL";
            }

            return Math.Round(value, 1) + " " + unit;
        }


        /// <summary>
        /// Create a percent string out of two double values
        /// </summary>
        /// <param name="numerator">Any double value</param>
        /// <param name="denominator">Any double value</param>
        /// <returns>String like "50%"</returns>
        public static string PercentOf(this double numerator, double denominator)
        {
            double percentage = Math.Round(numerator / denominator * 100, 1);
            if (denominator == 0)
            {
                return "0%";
            }
            else
            {
                return percentage + "%";
            }
        }


        /// <summary>
        /// Repeats a char a certain number of times and return it as a string
        /// </summary>
        /// <param name="charToRepeat">Char to repeat as char</param>
        /// <param name="numberOfRepetitions">Number of repetitions as int</param>
        /// <returns>Repeated char as string</returns>
        public static string Repeat(this char charToRepeat, int numberOfRepetitions)
        {
            if (numberOfRepetitions <= 0)
            {
                return "";
            }
            return new string(charToRepeat, numberOfRepetitions);
        }
    }
}