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
        //strings
        public string 
            outputPanelName = "Inventory Panel",
            nLne = Environment.NewLine,
            orePanelName = "Ore Panel",
            ingotPanelName = "Ingot Panel",
            componentPanelName = "Component Panel",
            thoughtLine = "-----------------------------------------",
            bptPfx = "MyObjectBuilder_BlueprintDefinition/",
            toolPanelName = "Tool Panel",
            oreStorageKeyword = "ore",
            ingotStorageKeyword = "ingot",

            componentStorageKeyword = "component",
            toolStorageKeyword = "tool",
            cargoPanelName = "Cargo Panel",
            queuePanelName = "Queue Panel",
            modPanelName = "Mod Panel",
            LPanelDef = "MyObjectBuilder_TextPanel/LargeLCDPanel",
            WPanelDef = "MyObjectBuilder_TextPanel/LargeLCDPanelWide",
            XLPanelDef = "MyObjectBuilder_TextPanel/LargeLCDPanel2x2",

            TPanelDef = "MyObjectBuilder_TextPanel/LargeTextPanel",
            SPanelDef = "MyObjectBuilder_TextPanel/SmallLCDPanel",
            SWPanelDef = "MyObjectBuilder_TextPanel/SmallLCDPanelWide", STPanelDef = "MyObjectBuilder_TextPanel/SmallTextPanel", cptTyp = "MyObjectBuilder_Component", orTyp = "MyObjectBuilder_Ore", igtTyp = "MyObjectBuilder_Ingot",
            tlTyp = "MyObjectBuilder_PhysicalGunObject",
            hdBtTyp = "MyObjectBuilder_GasContainerObject",
            oxBtTyp = "MyObjectBuilder_OxygenContainerObject", amTyp = "MyObjectBuilder_AmmoMagazine", srSbTyp = "GravelRefinery", cornerPanelDef = "blockcorner", exclusionKeyword = "exclude", autoConveyorKeyword = "autoConveyor", tTR = "", scriptName = "NDS Inventory Manager", crossGridKeyword = "crossGrid", noShowKeyword = "noShow", manualAssemblyKeyword = "manualAssembly", tgSs = "";

        // List<string>
        public List<string> otLT = new List<string>(),
            fIdLT = new List<string>(),
            outIdList = new List<string>(),
            otDtLst = new List<string>(),
            rcdIms = new List<string>(), 
            rcdBps = new List<string>(), 
            stTgs = new List<string>(),
            odBps = new List<string>();

        public SortedList<string, ItemDefinition> tDL = new SortedList<string,ItemDefinition>(),
            iDL = new SortedList<string, ItemDefinition>(),
            oDL = new SortedList<string, ItemDefinition>(),
            cDL = new SortedList<string, ItemDefinition>(),
            cmDL = new SortedList<string, ItemDefinition>();

        public SortedList<string, IMyTerminalBlock> sBks = new SortedList<string,IMyTerminalBlock>();
        public List<IMyTerminalBlock> aBL = new List<IMyTerminalBlock>(),
            tBL = new List<IMyTerminalBlock>(),
            tdBL = new List<IMyTerminalBlock>(),
            bkLT = new List<IMyTerminalBlock>(),
            bKs = new List<IMyTerminalBlock>();
        public List<int> icPB = new List<int>(),
            orPB = new List<int>(),
            stBK = new List<int>(),
            amPBK = new List<int>(),
            rcBK = new List<int>(),
            tSB = new List<int>(),
            oSB = new List<int>(),
            iSB = new List<int>(),
            cSB = new List<int>(),
            cIDS = new List<int>(),
            idleList = new List<int>();
         public int outputLimit = 25,
            tST = 0,
            tID = 0,
            graphLength = 4,
            WPanelWidth = 1575,
            WPanelHeight = 735,
            LPanelWidth = 787,
            LPanelHeight = 739,
            charWidth = 30,
            charHeight = 42,
            nameWidth = 19,
            SCornerHeight = 193,
            SFCornerHeight = 226,
            graphLengthWide = 8,
            cHG = 109,
            cFH = 127,
            updateFrequency = 1,
            gIIX = 0,
            spIX = 0,
            quMD = 0,
            quIX = 0,
            quCT = 0,
            refineryMode = 0,
            sM = 0,
            smQuIx = 0,
            iCIX = 0,
            ivBkCt = 0,
            scrIx = 0,
            stIx = 0,
            stoneOreToIngotBasic = 5,
            curInv = 0,
            assCount = 0,
            flCnsInt = 0,
            fillCanCycles = 2,
            oQuI = 0,
            sgIdx = 0,
            bpReqs = 0,
            dCt = 0, nBIX = 0,
            wkIdx = 0,
            alter = 0,
            fnCt = 0,
            tFnCt = 0,
            idIdx = 0,
            cCI = 0,
            ctMde = 0,
            ammoQuota = 2,
            mxActs = 0,
            hGsT = 0;
        //DateTimes
        public DateTime invScanTime = DateTime.Now.AddSeconds(-1.0),
            tickStartTime = DateTime.Now,
            asPtTm = DateTime.Now,
            scourTime = DateTime.Now,
            eAsTm = DateTime.Now,
            hpDLy = DateTime.Now.AddSeconds(15),
            nextOut = DateTime.Now,
            rsTm = DateTime.Now.AddSeconds(15);

        //Doubles
        public double scanDelay = 5,
            actionLimiterMultiplier = 0.007,
            runTimeLimiter = 1.5,
            lowRunTime = 200.0,
            highRunTime = 0.0,
            emptyAssemblerDelay = 8,
            sortAndDistributeDelay = 8,
            fuelQuota = 5,
            assemblerProductionRange = 0.02,
            iceQuota = 500,
            outputDelay = 0.15,
            overrideDelay = 15,
            version = 4.22;

        //Bools
        public bool firstCycle = true,
            conveyorControl = true,
            sameGridOnly = false,
            spOre = false,
            spIgt = false,
            reQU = false,
            quotasBelow = false,
            spCpt = false,
            spTl = false,
            ftSCN = true,
            splitOutput = false,
            fdSPs = false,
            rmTGS = false,
            itRH = true,
            active = true,
            hRQS = false,
            recordItems = false,
            flCns = true,
            survivalKitAssembly = true,
            rsTb = false,
            rSv = false,
            doubleLineSettings = false, displayQuotas = true,
            itSk = true,
            inGTs = false,
            cmPTs = false,
            oREs = false,
            tOLs = false,
            countLoadoutItems = true,
            countItemsAndBlueprints = true,
            dSub = false,
            queueBlueprints = true,
            sortItems = true,
            distributeItems = true,
            removeExcessBlueprintsAssembly = false,
            emptyAssemblers = true,
            arrangeBlueprints = true,
            refineStone = true,
            removeExcessBlueprintsDisassembly = true,
            spreadBlueprints = true,
            spreadRefineries = true,
            arrangeRefineries = true,
            spreadGasGenerators = true,
            spreadReactors = true,
            spreadAmmo = true,
            queueDisassembly = true,
            mergeBlueprints = true,
            doLoadouts = true,
            scnG = false,
            distributeLoadoutItems = false;


        //Block declarations
        public IMyTerminalBlock tBlk;
        public IMyInventory tInv;

        //Assemblers
        public MyAssemblerMode asMd = MyAssemblerMode.Assembly, dsMd = MyAssemblerMode.Disassembly;

        public Random rnd = new Random();
        public IMyGridTerminalSystem gSys;
        public IMyCubeGrid Grid;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            gSys = GridTerminalSystem;
            Grid = Me.CubeGrid;
            FillDict();
            LoadData();
            Save();
        }

        public void Save() { }

        //======================    MAIN METHOD    ===============================//
        
        public void Main(string argument, UpdateType updateSource)
        {
            tickStartTime = DateTime.Now;
            int itSg = sM;
            outIdList.Clear();
            otDtLst.Clear();
            scnG = false; //scan grid?
            try
            {
                if (argument != "")
                    Cmds(argument);
            }
            catch
            {
                OutP("Error Caught Running: " + argument);
            }

            if (rmTGS) RemTags(); try { if (active) MScript(); else SetConveyorUsage(true); } catch { OutP("Error Caught In Main"); }
            if (Runtime.CurrentInstructionCount > mxActs) { mxActs = Runtime.CurrentInstructionCount; hGsT = itSg; }
            Save();
        }
        public void MScript()
        {
            if (!firstCycle && DateTime.Now < invScanTime)
            {
                if (Runtime.LastRunTimeMs > highRunTime)
                    highRunTime = Runtime.LastRunTimeMs;
                if (Runtime.LastRunTimeMs < lowRunTime)
                    lowRunTime = Runtime.LastRunTimeMs;
            }
            bool tmpA = alter == 0 || !splitOutput || DateTime.Now < nextOut, tmpB = alter == 1 || !splitOutput; if (splitOutput) { if (alter == 0) alter = 1; else alter = 0; }
            if (tmpA) try
                {
                    Script();
                }
                catch
                {
                    OutP("Error Caught In Script");
                }
            if (tmpB)
            {
                try { if (DateTime.Now >= nextOut) MainOuts(); }
                catch
                {
                    OutP("Error Caught Creating Outputs");
                }
                if (DateTime.Now >= nextOut) PaintOutput();
            }
        }
        public void RemTags()
        {
            if (tBL.Count == 0) gSys.GetBlocks(tBL); string name = ""; int index = 0; while (tBL.Count > 0)
            {
                name = tBL[0].CustomName; if (name.ToLower().Contains(tTR) && (!sameGridOnly || tBL[0].CustomData.ToLower().Contains(crossGridKeyword.ToLower()) || tBL[0].CubeGrid == Grid))
                {
                    index = name.ToLower().IndexOf(tTR); tBL[0].CustomName = name.Substring(0, index);
                }
                tBL.RemoveAt(0); if (!AvailableActions()) break;
            }
            if (tBL.Count == 0) { rmTGS = false; OutP("Tags removed"); }
        }
        public void MainOuts()
        {
            OtCnts(oDL, orePanelName); OtCnts(iDL, ingotPanelName); OtCnts(cDL, componentPanelName, false, false, true); OtCnts(tDL, toolPanelName, false, false, true);
            OtCnts(cmDL, modPanelName, true); OutCargo(); OutQus(); if (hpDLy > DateTime.Now)
            {
                Echo("~Help ~ Echo commands for 30 seconds" + nLne + (hpDLy - DateTime.Now).TotalSeconds.ToString("N0") + " remaining" + nLne + "~Toggle" + nLne + "Toggle entire script" + nLne + "~Enable All" + nLne + "Enable all stages" + nLne + "~Disable All" + nLne + "Disable all stages" + nLne + "~Save" + nLne +
"Save settings to custom data" + nLne + "~Load" + nLne + "Load settings from custom data" + nLne + "~Clear Queue" + nLne + "Clear assembly queues" + nLne + "~Set Component X" + nLne + "All component quotas to X" + nLne + "~Set Tool X" + nLne + "All tool" + ", " + "ammo" + ", " + "and canister quotas to X" + nLne + "~Set Ingot X" + nLne + "All ingot quotas to X" + nLne + "~Set Ore X" + nLne +
"All ore quotas to X" + nLne + "~Set All X" + nLne + "All quotas to X" + nLne + "~Merge A : B" + nLne + "Merge and save mod items A and B");
            }
        }
        public bool AvailableActions()
        {
            return (Runtime.CurrentInstructionCount < (Runtime.MaxInstructionCount * actionLimiterMultiplier) && 
                (DateTime.Now - tickStartTime).TotalMilliseconds < runTimeLimiter);
        }
        public void Cmds(string argument)
        {
            bool sDA = false; string arg = argument.ToLower().Replace(" ", ""); if (arg.StartsWith("setcomponent")) { sDA = true; int taMT = (int)double.Parse(arg.Substring(12, arg.Length - 12)); if (taMT == -2147483648) taMT = 2147483647; SetQ(taMT, ref cDL); }
            else if (arg.StartsWith("settool"))
            {
                sDA = true; int taMT = (int)double.Parse(arg.Substring(7, arg.Length - 7)); if (taMT == -2147483648)
                    taMT = 2147483647; SetQ(taMT, ref tDL);
            }
            else if (arg.StartsWith("setingot")) { sDA = true; int taMT = (int)double.Parse(arg.Substring(8, arg.Length - 8)); if (taMT == -2147483648) taMT = 2147483647; SetQ(taMT, ref iDL); }
            else if (arg.StartsWith("setore"))
            {
                sDA = true; int taMT = (int)double.Parse(arg.Substring(6, arg.Length - 6)); if (taMT == -2147483648) taMT = 2147483647; SetQ(taMT,
ref oDL);
            }
            else if (arg.StartsWith("setall")) { sDA = true; int taMT = (int)double.Parse(arg.Substring(6, arg.Length - 6)); if (taMT == -2147483648) taMT = 2147483647; SetQ(taMT, ref oDL); SetQ(taMT, ref iDL); SetQ(taMT, ref tDL); SetQ(taMT, ref cDL); }
            else if (arg.StartsWith("merge"))
            {
                string tmpB = arg.Substring(5, (arg.Length - 5)); int taMT = int.Parse(tmpB.Substring(0,
tmpB.IndexOf(":"))); tmpB = tmpB.Substring((tmpB.IndexOf(":") + 1), tmpB.Length - (tmpB.IndexOf(":") + 1)); int tmpC = int.Parse(tmpB); MergeAndAdd(cmDL.Values[taMT], cmDL.Values[tmpC]); if (tmpC > taMT) { cmDL.RemoveAt(tmpC); cmDL.RemoveAt(taMT); } else { cmDL.RemoveAt(taMT); cmDL.RemoveAt(tmpC); }
            }
            else if (arg.StartsWith("removetags") && arg.Length > 10)
            {
                tTR = arg.Substring(10,
arg.Length - 10); rmTGS = true;
            }
            else switch (arg) { case "clearqueue": ClearQueue(); break; case "save": SaveData(); break; case "load": LoadData(); break; case "help": hpDLy = DateTime.Now.AddSeconds(30); break; case "toggle": active = !active; break; case "disableall": Toggle(false); break; case "enableall": Toggle(true); break; }
            if (sDA) SaveData();
        }
        public void Toggle(bool bl)
        {
            countItemsAndBlueprints = bl;
            queueBlueprints = bl;
            sortItems = bl;
            distributeItems = bl;
            emptyAssemblers = bl;
            arrangeBlueprints = bl;
            spreadBlueprints = bl;
            spreadRefineries = bl;
            arrangeRefineries = bl;
            spreadGasGenerators = bl;
            spreadReactors = bl;
            spreadAmmo = bl;
            queueDisassembly = bl;
            mergeBlueprints = bl;
            doLoadouts = bl;
            SaveData();
        }
        public void SetQ(int amt, ref SortedList<string,
ItemDefinition> list)
        { for (int i = 0; i < list.Count; i++) list.Values[i].dAMT = amt; }


        public void Script()
        {
            if (DateTime.Now >= invScanTime)
            {
                if (DateTime.Now >= hpDLy)
                    Echo("Inventory scan");
                if (ResearchInventory())
                    invScanTime = DateTime.Now.AddSeconds(scanDelay);
            }
            else ScriptSwitch();
        }

        public void ScriptSwitch()
        {
            int tmpA = sM;
            if (DateTime.Now >= hpDLy) Echo("Script stage: " + sM);
            switch (sM)
            {
                case 0: if (countItemsAndBlueprints) { do { CountItemsFromColl(); } while (AvailableActions() && sM == 0); } else sM++; break;
                case 1: if (queueBlueprints) { do { QueueItemsByNext(); } while (AvailableActions() && sM == 1); } else sM++; break;
                case 2: if (sortItems) { do { ScourInventories(false); } while (AvailableActions() && sM == 2); } else sM++; break;
                case 3:
                    if (distributeItems)
                    {
                        do
                        {
                            ScourInventories(true);
                        } while (AvailableActions() && sM == 3);
                    }
                    else sM++; break;
                case 4: if (emptyAssemblers) { do { EmptyUnusedAssemblers(); } while (AvailableActions() && sM == 4); } else sM++; break;
                case 5: if (arrangeBlueprints) { do { MoveBadRequests(); } while (AvailableActions() && sM == 5); } else sM++; break;
                case 6:
                    if (spreadBlueprints)
                    {
                        do { WorkIdleAssemblers();}
                        while (AvailableActions() && sM == 7);
                    }
                    else sM++; break;
                case 7: if (spreadRefineries) { do { WorkIdleRefineries(); } while (AvailableActions() && sM == 8); } else sM++; break;
                case 8: if (arrangeRefineries) { do { OrderRefineries(); } while (AvailableActions() && sM == 9); } else sM++; break;
                case 9: if (spreadGasGenerators) { do { WorkIdle(true); } while (AvailableActions() && sM == 10); } else sM++; break;
                case 10: if (spreadReactors) { do { WorkIdle(false, true); } while (AvailableActions() && sM == 11); } else sM++; break;
                case 11: if (spreadAmmo) { do { WorkIdle(false, false, true); } while (AvailableActions() && sM == 12); } else sM++; break;
                case 12: if (queueDisassembly) { try { QueueDisassembly(); } catch { OutP("Error Queueing Disassembly"); } } else sM++; break;
                case 13:
                    if (mergeBlueprints)
                    { try { MergeOccurrences(); } catch { OutP("Error Merging Queue Stacks"); } }
                    else sM++; break;
                case 14: if (doLoadouts) { try { Stock(); } catch { OutP("Error filling stocks"); } } else sM++; break;
            }
            dSub = false; if (sM != tmpA) { rsTm = DateTime.Now.AddSeconds(overrideDelay); rsTb = false; }
            if (DateTime.Now >= rsTm) rsTb = true; if (sM > 14)
            {
                sM = 0; if (flCnsInt >= fillCanCycles)
                {
                    flCns = true; flCnsInt = 0;
                }
                else flCnsInt++; firstCycle = false;
            }
        }
        public void StockTags(ref string tag, ref bool inGT, ref bool cmPT, ref bool oRE, ref bool tOL)
        {
            tag = tag.ToLower(); int tmpA = tag.Length; if (tag.Contains(ingotStorageKeyword.ToLower() + ":")) { tag = tag.Replace(ingotStorageKeyword.ToLower() + ":", ""); inGT = true; }
            if (tag.Contains(componentStorageKeyword.ToLower() + ":"))
            {
                tag = tag.Replace(componentStorageKeyword.ToLower() + ":", ""); cmPT = true;
            }
            if (tag.Contains(oreStorageKeyword.ToLower() + ":")) { tag = tag.Replace(oreStorageKeyword.ToLower() + ":", ""); oRE = true; }
            if (tag.Contains(toolStorageKeyword.ToLower() + ":")) { tag = tag.Replace(toolStorageKeyword.ToLower() + ":", ""); tOL = true; }
            if (!inGT && !cmPT && !oRE && !tOL)
            {
                if (tag.Contains(":*"))
                {
                    if (
tag.Contains(":*:*")) tag = tag.Replace(":*:*", ":*");
                    else if (tmpA - tag.Replace(":", "").Length == 2) tag = tag.Replace(":*", ""); tOL = true; oRE = true; cmPT = true; inGT = true;
                }
                else cmPT = true;
            }
        }
        public void Stock()
        {
            try
            {
                while (scrIx < aBL.Count)
                {
                    if (stTgs.Count == 0) GetTags(ref stTgs, aBL[scrIx].CustomData, "{", "}"); while (stTgs.Count > 0)
                    {
                        if (itSk)
                        {
                            itSk = false; tgSs = stTgs[0];
                            StockTags(ref tgSs, ref inGTs, ref cmPTs, ref oREs, ref tOLs); if (!inGTs && !cmPTs && !oREs && !tOLs) cmPTs = true;
                        }
                        string tag = tgSs; bool tmpA = false; if (tag.Contains(":"))
                        {
                            double dAMT = double.Parse(tag.Substring(0, tag.IndexOf(":"))); int index = tag.IndexOf(":") + 1; tag = tag.Substring(index, tag.Length - index); if (cmPTs) { StkItm(dAMT, cDL, cSB, tag); cmPTs = false; }
                            else if (
inGTs) { StkItm(dAMT, iDL, iSB, tag); inGTs = false; }
                            else if (oREs) { StkItm(dAMT, oDL, oSB, tag); oREs = false; } else if (tOLs) { StkItm(dAMT, tDL, tSB, tag); tOLs = false; }
                        }
                        else tmpA = true; if (tmpA || (!cmPTs && !inGTs && !oREs && !tOLs)) { stTgs.RemoveAt(0); itSk = true; }
                        if (!AvailableActions()) break;
                    }
                    if (stTgs.Count == 0) scrIx++; if (!AvailableActions()) break;
                }
            }
            catch
            {
                OutP(
"Error parsing loadout of " + aBL[scrIx].CustomName);
            }
            if (scrIx >= aBL.Count || rsTb) { scrIx = 0; sM++; }
        }
        
        /// <summary>
        /// Stacks items
        /// </summary>
        /// <param name="dAMT">Amount of items</param>
        /// <param name="defList"></param>
        /// <param name="indices"></param>
        /// <param name="tag"></param>
        public void StkItm(double dAMT, SortedList<string, ItemDefinition> defList, List<int> indices, string tag)
        {
            try
            {
                List<MyInventoryItem> items = new List<MyInventoryItem>(); for (int i = 0; i < defList.Count; i++)
                {
                    if (tag == "*" || defList.Values[i].oN.ToLower().Replace(" ", "").StartsWith(tag))
                    {
                        string itemId = defList.Values[i].ItemId(), iTY = defList.Values[i].iTY, iST = defList.Values[i].iST; IMyInventory loInv = aBL[scrIx].GetInventory(0); double amt = CntInInv(itemId, loInv), tmpA = 0.0; if (amt < dAMT)
                        {
                            for (int x = 0; x < indices.Count; x++)
                            {
                                IMyInventory stInv = aBL[indices[x]].GetInventory(0);
                                if (aBL[indices[x]].Position != aBL[scrIx].Position && CntInInv(itemId, stInv) > 0.0)
                                {
                                    stInv.GetItems(items, (t => t.Type.ToString() == itemId));

                                    for (int b = 0; b < items.Count; b++)
                                    {
                                        if (!LoadoutHome(aBL[indices[x]].CustomData.ToLower(), iTY, iST))
                                        { TPot(loInv, stInv, items[b], dAMT - amt, ref tmpA); amt += tmpA; if (amt >= dAMT) break;
                                            break;
                                        }
                                    }
                                }
                                if (amt >= dAMT) break;
                            }
                        }
                        else if (amt > dAMT)
                        {
                            for (int x = 0; x < indices.Count; x++)
                            {
                                if (aBL[indices[x]].Position != aBL[scrIx].Position)
                                {
                                    IMyInventory stInv = aBL[indices[x]].GetInventory(0);
                                    loInv.GetItems(items, (t => t.Type.ToString() == itemId));
                                    foreach (MyInventoryItem item in items)
                                    {
                                        TPot(stInv, loInv, item, amt - dAMT, ref tmpA); amt -= tmpA; if (amt <= dAMT)
                                            break;
                                        break;
                                        //not sure why there are two breaks
                                    }
                                }
                                if (amt <= dAMT) break;
                            }
                        }
                    }
                }
            }
            catch { }
        }
        public void WorkIdleRefineries()
        {
            if (
refineryMode == 0) { WorkIdle(false, false); if (sM == 9 && stBK.Count > 0) { sM = 8; refineryMode = 1; } }
            else { WorkIdle(false, false, false, true); if (sM == 9) refineryMode = 0; }
        }
        public string CData(IMyTerminalBlock block) { return block.CustomData.ToLower(); }
        public void AddBlueprintDefinition(MyDefinitionId blueprintId, VRage.MyFixedPoint aMT)
        {
            bool found = false; for (int i = 0;
i < mdQuLst.Count; i++) if (mdQuLst[i].blueprintId.ToString() == blueprintId.ToString()) { found = true; mdQuLst[i].aMT = (VRage.MyFixedPoint)((double)mdQuLst[i].aMT + (double)aMT); break; }
            if (!found) { BlueprintDefinition bDef = new BlueprintDefinition(); bDef.blueprintId = blueprintId; bDef.aMT = aMT; mdQuLst.Add(bDef); }
            RecordBlueprint(blueprintId);
        }
        public bool VerifyBlueprint(
string bST, SortedList<string, ItemDefinition> list)
        { if (bST == "StoneOreToIngotBasic") return true; for (int i = 0; i < list.Count; i++) { if (bST == list.Values[i].bST) return true; } return false; }


        public void RecordBlueprint(MyDefinitionId blueprintId)
        {
            if (!rcdBps.Contains(blueprintId.ToString()))
            {
                rcdBps.Add(blueprintId.ToString()); string iST = blueprintId.SubtypeName;
                bool found = VerifyBlueprint(iST, cDL); if (!found) found = VerifyBlueprint(iST, tDL); if (!found) for (int i = 0; i < cmDL.Count; i++) { if (iST == cmDL.Values[i].iST) { found = true; cmDL.Values[i].bST = iST; AddItemDefinition(cmDL.Values[i]); cmDL.RemoveAt(i); SaveData(); break; } }
                if (!found)
                {
                    ItemDefinition def = new ItemDefinition(); def.bST = iST; def.oN = def.bST; def.mod = true;
                    cmDL[iST] = def;
                }
            }
        }
        public void MergeAndAdd(ItemDefinition defA, ItemDefinition defB)
        {
            try
            {
                ItemDefinition def = new ItemDefinition(); if (defA.bST == "") def.iST = defA.iST; else if (defB.bST == "") def.iST = defB.iST; if (defA.iTY != "") def.iTY = defA.iTY; else if (defB.iTY != "") def.iTY = defB.iTY; if (defA.bST != "") def.bST = defA.bST; else if (defB.bST != "") def.bST = defB.bST; if (
defA.vRt != 0.5) def.vRt = defA.vRt;
                else if (defB.vRt != 0.5) def.vRt = defB.vRt; def.mod = true; def.oN = def.iST; def.aMT += defA.aMT + defB.aMT; def.qAMT += defA.qAMT + defB.qAMT; AddItemDefinition(def); SaveData();
            }
            catch { OutP("Error caught merging mod items"); }
        }
        public bool VerifyItem(MyInventoryItem item)
        {
            string iTY = item.Type.TypeId, iST = item.Type.SubtypeId; if (iTY == igtTyp)
                return iDL.ContainsKey(iST); if (iTY == orTyp) return oDL.ContainsKey(iST); if (iTY == cptTyp) return cDL.ContainsKey(iST); if (ToolType(iTY)) return tDL.ContainsKey(iST); return false;
        }
        public void RecordItem(MyInventoryItem item, IMyTerminalBlock block, IMyInventory origInv)
        {
            string itemName = item.Type.ToString(); if (recordItems && !rcdIms.Contains(itemName))
            {
                string iST = item.Type.SubtypeId; rcdIms.Add(itemName); bool found = false; found = VerifyItem(item); if (!found) for (int i = 0; i < cmDL.Count; i++)
                    {
                        if (iST == cmDL.Values[i].bST)
                        {
                            found = true; cmDL.Values[i].iST = iST; cmDL.Values[i].iTY = item.Type.TypeId; cmDL.Values[i].vRt = GetItemVolume(item, block, origInv); AddItemDefinition(cmDL.Values[i]); cmDL.RemoveAt(i); SaveData(); break;
                        }
                    }
                if (!found) { ItemDefinition def = new ItemDefinition(); def.iST = iST; def.oN = iST; def.iTY = item.Type.TypeId; def.vRt = GetItemVolume(item, block, origInv); def.mod = true; if (def.iTY == igtTyp || def.iTY == orTyp) { def.bST = "None"; AddItemDefinition(def); SaveData(); } else cmDL[iST] = def; }
            }
        }
        public double GetItemVolume(MyInventoryItem item, IMyTerminalBlock block,
IMyInventory origInv)
        {
            double tmpD = 0.5; bool tmpC = true; tBlk = block; if (cIDS.Count > 0) tBlk = aBL[cIDS[0]]; else tmpC = false; if (block.Position == tBlk.Position) { if (cIDS.Count > 1) tBlk = aBL[cIDS[1]]; else tmpC = false; }
            if (tmpC)
            {
                double tmpB = CrVol(origInv); VRage.MyFixedPoint aMT = (VRage.MyFixedPoint)1; if (1.0 > (double)item.Amount) aMT = item.Amount; tBlk.GetInventory(0)
.TransferItemFrom(origInv, item, aMT); tmpD = ((tmpB - CrVol(origInv)) * 1000.0 / (double)aMT);
            }
            else OutP("Insufficient cargo containers to record item volume"); if (tmpD == 0.0) tmpD = 15.0; return tmpD;
        }
        public List<MyProductionItem> mgQuLst = new List<MyProductionItem>(); public List<BlueprintDefinition> mdQuLst = new List<BlueprintDefinition>();
        public void MergeOccurrences()
        {
            try
            {
                if (bkLT.Count == 0) gSys.GetBlocksOfType<IMyAssembler>(bkLT, (p => !((IMyAssembler)p).IsQueueEmpty && (!sameGridOnly || p.CustomData.ToLower().Contains(crossGridKeyword.ToLower()) || p.CubeGrid == Grid))); while (bkLT.Count > 0)
                {
                    if (ftSCN) { ftSCN = false; ((IMyAssembler)bkLT[0]).GetQueue(mgQuLst); quCT = mgQuLst.Count; } while (mgQuLst.Count > 0)
                    {
                        AddBlueprintDefinition(mgQuLst[0].BlueprintId, mgQuLst[0].Amount); mgQuLst.RemoveAt(0); if (!AvailableActions()) break;
                    }
                    if (mgQuLst.Count == 0)
                    {
                        if (quCT != mdQuLst.Count || reQU)
                        {
                            if (!reQU) { reQU = true; ((IMyAssembler)bkLT[0]).ClearQueue(); } while (mdQuLst.Count > 0)
                            {
                                ((IMyAssembler)bkLT[0]).AddQueueItem(mdQuLst[0].blueprintId, mdQuLst[0].aMT); mdQuLst.RemoveAt(0); if (
!AvailableActions()) break;
                            }
                        }
                        else mdQuLst.Clear(); if (mdQuLst.Count == 0) { bkLT.RemoveAt(0); reQU = false; ftSCN = true; }
                    }
                    if (!AvailableActions()) break;
                }
            }
            catch { }
            if (bkLT.Count == 0 || rsTb) { sM++; bkLT.Clear(); mdQuLst.Clear(); mgQuLst.Clear(); }
        }
        public void QueueDisassembly()
        {
            try
            {
                if (DateTime.Now >= hpDLy && !dSub) { Echo("Script substage: " + quMD); dSub = true; }
                switch (quMD)
                {
                    case 0: bool tmpB = false; if (!hRQS) { string bST = ""; int taMT = 0; do { bST = ""; taMT = 0; if (NextBlueprint(ref bST, ref taMT, ref tmpB, true)) QueueItems(bST, taMT, true); } while (!tmpB && AvailableActions()); } else tmpB = true; if (tmpB) quMD++; break;
                    case 1:
                        if (removeExcessBlueprintsDisassembly)
                        {
                            for (int i = quIX; i < (tDL.Count + cDL.Count); i++)
                            {
                                double tmpA = 0.0; quIX++; if (
i < tDL.Count) { tmpA = tDL.Values[i].ExQued(true); if (tmpA > 0.0) RemBp(tDL.Values[i].bST, true, tmpA); }
                                else { tmpA = cDL.Values[(i - tDL.Count)].ExQued(true); if (tmpA > 0.0) RemBp(cDL.Values[(i - tDL.Count)].bST, true, tmpA); }
                                if (!AvailableActions()) break;
                            }
                            if (quIX >= (tDL.Count + cDL.Count) || rsTb) { quIX = 0; quMD = 0; sM++; }
                        }
                        else { quMD = 0; sM++; }
                        break;
                }
            }
            catch { }
        }
        public void QueueItemsByNext() { try { QueueSwitch(); } catch { OutP("Error Caught Queueing Items"); } }
        public void QueueSwitch()
        {
            if (DateTime.Now >= hpDLy && !dSub) { Echo("Script substage: " + quMD); dSub = true; }
            switch (quMD)
            {
                case 0:
                    string bST = ""; int taMT = 0; bool tmpB = false; for (int i = 0; i < 150; i++)
                    {
                        bST = ""; taMT = 0; if (NextBlueprint(ref bST, ref taMT, ref tmpB))
                        {
                            QueueItems(bST,
taMT);
                        }
                        else if (tmpB) break; if (!AvailableActions() || tmpB) break;
                    }
                    if (tmpB) quMD++; break;
                case 1:
                    if (removeExcessBlueprintsAssembly)
                    {
                        int a = quIX; double tmpA = 0.0; if (a < tDL.Count) { tmpA = tDL.Values[a].ExQued(false, assemblerProductionRange); if (tmpA > 0.0) RemBp(tDL.Values[a].bST, false, tmpA); }
                        else if ((a - tDL.Count) < cDL.Count)
                        {
                            a -= tDL.Count; tmpA = cDL.Values[a].ExQued(
false, assemblerProductionRange); if (tmpA > 0.0) RemBp(cDL.Values[a].bST, false, tmpA);
                        }
                        quIX++; if (quIX >= (tDL.Count + cDL.Count) || rsTb) { quIX = 0; quMD++; }
                    }
                    else quMD++; break;
                case 2:
                    if (stoneOreToIngotBasic > 0)
                    {
                        List<IMyTerminalBlock> tmpC = new List<IMyTerminalBlock>(); MyDefinitionId id = MyDefinitionId.Parse(bptPfx + "StoneOreToIngotBasic");
                        gSys.GetBlocksOfType<IMyAssembler>(tmpC, (p => (p.BlockDefinition.ToString().ToLower().Contains("survival") && !p.CustomData.ToLower().Contains(exclusionKeyword.ToLower()) && p.IsFunctional && (!sameGridOnly || p.CustomData.ToLower().Contains(crossGridKeyword.ToLower()) || p.CubeGrid == Grid)) && !p.CustomData.ToLower().Contains(manualAssemblyKeyword.ToLower())));
                        VRage.MyFixedPoint aMT = (VRage.MyFixedPoint)stoneOreToIngotBasic; for (int i = 0; i < tmpC.Count; i++) { if (CountInQueue(tmpC[i], id) == 0) ((IMyAssembler)tmpC[i]).AddQueueItem(id, aMT); }
                    }
                    quMD = 0; sM++; break;
            }
        }
        public bool GetIndices()
        {
            if (gIIX == 0) { spOre = false; spCpt = false; spIgt = false; spTl = false; }
            if (fdSPs || CheckSpecifics())
            {
                for (int i = gIIX; i < aBL.Count; i++)
                {
                    gIIX++; try
                    {
                        AddIndex(aBL[i], i);
                    }
                    catch { OutP("Error Caught Getting Indices"); }
                }
                if (gIIX >= aBL.Count) { if (oSB.Count == 0 || cSB.Count == 0 || iSB.Count == 0 || tSB.Count == 0) foreach (KeyValuePair<int, IMyTerminalBlock> kvp in iBks) AddIndex(kvp.Value, kvp.Key, true); iBks.Clear(); gIIX = 0; fdSPs = false; return true; }
            }
            return false;
        }
        
        public SortedDictionary<int,IMyTerminalBlock> iBks = new SortedDictionary<int, IMyTerminalBlock>();
        public void AddIndex(IMyTerminalBlock block, int i, bool oR = false)
        {
            bool oRI = iSB.Count == 0, oRO = oSB.Count == 0, oRC = cSB.Count == 0, oRT = tSB.Count == 0; if (block is IMyCargoContainer)
            {
                string tmpA = block.CustomData.ToLower(); RemoveTags(ref tmpA); if (!cIDS.Contains(i)) cIDS.Add(i); if (oR ||
!tmpA.Contains("{"))
                {
                    if (!spOre || tmpA.Contains(oreStorageKeyword.ToLower())) if (!oSB.Contains(i)) oSB.Add(i); if (!spCpt || tmpA.Contains(componentStorageKeyword.ToLower())) if (!cSB.Contains(i)) cSB.Add(i); if (!spIgt || tmpA.Contains(ingotStorageKeyword.ToLower())) if (!iSB.Contains(i)) iSB.Add(i); if (!spTl || tmpA.Contains(toolStorageKeyword.ToLower()))
                        if (!tSB.Contains(i)) tSB.Add(i);
                }
                else iBks[i] = block;
            }
            else if (block is IMyRefinery && !block.BlockDefinition.ToString().ToLower().Contains("shield")) { if (block.BlockDefinition.ToString().ToLower().Contains(srSbTyp.ToLower())) stBK.Add(i); else orPB.Add(i); }
            else if (block is IMyGasGenerator) icPB.Add(i);
            else if (IsGun(block)) amPBK.Add(i);
            else if (
block is IMyReactor) rcBK.Add(i);
        }
        public bool CheckSpecifics()
        {
            for (int i = spIX; i < aBL.Count; i++)
            {
                spIX++; if (aBL[i] is IMyCargoContainer)
                {
                    string tmpA = aBL[i].CustomData.ToLower(); RemoveTags(ref tmpA); if (tmpA.Contains(toolStorageKeyword.ToLower())) spTl = true; if (tmpA.Contains(ingotStorageKeyword.ToLower())) spIgt = true; if (tmpA.Contains(oreStorageKeyword.ToLower(
))) spOre = true; if (tmpA.Contains(componentStorageKeyword.ToLower())) spCpt = true;
                }
            }
            if (spIX >= aBL.Count) { spIX = 0; fdSPs = true; return true; }
            return false;
        }
        public void RemoveTags(ref string cData, string prefix, string suffix)
        {
            if (cData.Contains(prefix) && cData.Contains(suffix))
            {
                int indexA = cData.IndexOf(prefix), indexB = cData.IndexOf(suffix); if (indexA < indexB)
                {
                    string tempA = "", tempB = ""; if (indexA > 0) tempA = cData.Substring(0, indexA); if (indexB + 1 < cData.Length) tempB = cData.Substring(indexB + 1, cData.Length - indexB - 1); cData = tempA + tempB;
                }
            }
        }
        public void RemoveTags(ref string cData)
        {
            RemoveTags(ref cData, "[", "]");
            RemoveTags(ref cData, "{", "}");
        }
        public List<MyInventoryItem> cIvImLt = new List<MyInventoryItem>(), ctOtIv = new List<MyInventoryItem>();

        public List<MyProductionItem> countOutputQueue = new List<MyProductionItem>(); public void CountItemsFromColl()
        {
            try
            {
                if (DateTime.Now >= hpDLy && !dSub)
                {
                    Echo("Script substage: " + ctMde);
                    dSub = true;
                }
                switch (ctMde)
                {
                    case 0:
                        CountItems();
                        break;
                    case 1:
                        CountBlueprints();
                        break;
                }
            }
            catch
            {
                OutP("Error Caught Counting");
            }
            if (ctMde > 1 || rsTb)
            {
                ctMde = 0; sM++;
            }
        }



        public void CountBlueprints()
        {
            try
            {
                if (bkLT.Count == 0) Fill<IMyAssembler>(ref bkLT, "", manualAssemblyKeyword.ToLower()); while (bkLT.Count > 0)
                {
                    try
                    {
                        if (countOutputQueue.Count == 0) ((IMyAssembler)bkLT[0]).GetQueue(countOutputQueue); while (countOutputQueue.Count > 0)
                        {
                            try
                            {
                                AddTempBlueprintCount(countOutputQueue[0].BlueprintId.SubtypeName, (double)
countOutputQueue[0].Amount, ((IMyAssembler)bkLT[0]).Mode != asMd);
                            }
                            catch { }
                            countOutputQueue.RemoveAt(0); if (!AvailableActions()) break;
                        }
                        if (countOutputQueue.Count == 0) { bkLT.RemoveAt(0); }
                        if (!AvailableActions()) break;
                    }
                    catch { }
                }
            }
            catch { OutP("Error Caught Counting Blueprints"); if (bkLT.Count > 0) bkLT.RemoveAt(0); }
            if (bkLT.Count == 0) { SetBlueprintCounts(); ctMde++; }
        }
        public void CountItems()
        {
            try
            {
                for (int i = cCI; i < aBL.Count; i++)
                {
                    try
                    {
                        if (cIvImLt.Count == 0 && ctOtIv.Count == 0 && aBL[i].InventoryCount > 1) aBL[i].GetInventory(1).GetItems(ctOtIv);
                        if (cIvImLt.Count == 0) aBL[i].GetInventory(0).GetItems(cIvImLt); while (ctOtIv.Count > 0) { cIvImLt.Add(ctOtIv[0]); ctOtIv.RemoveAt(0); if (!AvailableActions()) break; }
                        if (ctOtIv.Count == 0) while (
cIvImLt.Count > 0) { try { if (!Discount(aBL[i], cIvImLt[0])) AddTempItemCount(cIvImLt[0], (double)cIvImLt[0].Amount); } catch { } cIvImLt.RemoveAt(0); if (!AvailableActions()) break; }
                        if (cIvImLt.Count == 0 && ctOtIv.Count == 0) cCI++; if (!AvailableActions()) break;
                    }
                    catch { cCI++; }
                }
            }
            catch { OutP("Error Caught Counting Items"); }
            if (cCI >= aBL.Count)
            {
                if (iCIX >= 0) SetTempItemCounts(iCIX);
                iCIX++; if (iCIX > 3) { iCIX = -1; cCI = 0; ctMde++; }
            }
        }



        public void SetConveyorUsage(bool tmpB = false)
        {
            List<IMyTerminalBlock> bKS = new List<IMyTerminalBlock>(); string tmpA = autoConveyorKeyword.ToLower(); Fill<IMyReactor>(ref bKS, "", tmpA); for (int i = 0; i < bKS.Count; i++) ((IMyReactor)bKS[i]).UseConveyorSystem = tmpB; Fill<IMyGasGenerator>(ref bKS, "", tmpA); for (int i = 0;
i < bKS.Count; i++) ((IMyGasGenerator)bKS[i]).UseConveyorSystem = tmpB; Fill<IMyRefinery>(ref bKS, "", tmpA); for (int i = 0; i < bKS.Count; i++) ((IMyRefinery)bKS[i]).UseConveyorSystem = tmpB; Fill<IMySmallGatlingGun>(ref bKS, "", tmpA); for (int i = 0; i < bKS.Count; i++) if (((IMySmallGatlingGun)bKS[i]).UseConveyorSystem != tmpB) bKS[i].ApplyAction("UseConveyor");
            Fill<IMyLargeGatlingTurret>(ref bKS, "", tmpA); for (int i = 0; i < bKS.Count; i++) if (((IMyLargeGatlingTurret)bKS[i]).UseConveyorSystem != tmpB) bKS[i].ApplyAction("UseConveyor"); Fill<IMyLargeMissileTurret>(ref bKS, "", tmpA); for (int i = 0; i < bKS.Count; i++) if (((IMyLargeMissileTurret)bKS[i]).UseConveyorSystem != tmpB) bKS[i].ApplyAction("UseConveyor");
            Fill<IMySmallMissileLauncherReload>(ref bKS, "", tmpA); for (int i = 0; i < bKS.Count; i++) if (((IMySmallMissileLauncherReload)bKS[i]).UseConveyorSystem != tmpB) bKS[i].ApplyAction("UseConveyor"); Fill<IMySmallMissileLauncher>(ref bKS, "", tmpA); for (int i = 0; i < bKS.Count; i++) if (((IMySmallMissileLauncher)bKS[i]).UseConveyorSystem != tmpB) bKS[i].ApplyAction(
                                     "UseConveyor"); Fill<IMyAssembler>(ref bKS, "", tmpA); for (int i = 0; i < bKS.Count; i++) ((IMyAssembler)bKS[i]).UseConveyorSystem = true;
        }
        public void Fill<T>(ref List<IMyTerminalBlock> list, string inclusion = "", string exclusion = "", string defInc = "", string defExc = "", string defExcB = "") where T : class
        {
            string inc = inclusion.ToLower(), exc = exclusion.ToLower();
            gSys.GetBlocksOfType<T>(list, (b => b.IsFunctional && (defInc == "" || b.BlockDefinition.ToString().ToLower().Contains(defInc)) && !b.CustomData.ToLower().Contains(exclusionKeyword.ToLower()) && (!sameGridOnly || b.CustomData.ToLower().Contains(crossGridKeyword.ToLower()) || b.CubeGrid == Grid) && (defExc == "" || !b.BlockDefinition.ToString().ToLower().Contains(defExc)) && (
             exc == "" || !(b.CustomData.ToLower().Contains(exc))) && (defExcB == "" || !b.BlockDefinition.ToString().ToLower().Contains(defExcB)) && (inc == "" || b.CustomData.ToLower().Contains(inc) || b.CustomName.ToLower().Contains(inc))));
        }
        public double Priority(MyInventoryItem item)
        {
            if ((double)item.Amount < 0.01) return -1; string iST = item.Type.SubtypeId; if (iST == "Scrap") iST = "Iron";
            if (iDL.ContainsKey(iST)) return iDL[iST].Priority(); return 0;
        }
        public double Priority(MyDefinitionId item)
        {
            string bST = item.SubtypeName; if (bST == "StoneOreToIngotBasic") return 0.0; for (int i = 0; i < tDL.Count; i++) { if (tDL.Values[i].bST == bST) return tDL.Values[i].Priority(); }
            for (int i = 0; i < cDL.Count; i++)
            {
                if (cDL.Values[i].bST == bST) return cDL.Values[i].Priority();
            }
            return 101.0;
        }
        public void FillListG<T>(ref List<IMyTerminalBlock> bKS, string keywordA, string keywordB = "", string exclude = "") where T : class
        {
            gSys.GetBlocksOfType<T>(bKS, (p => p.CustomName.ToLower().Contains(keywordA.ToLower()) && p.IsFunctional && (keywordB == "" || p.CustomName.ToLower().Contains(keywordB.ToLower())) && !p.CustomData.ToLower().Contains(
exclusionKeyword.ToLower()) && p.CubeGrid.Equals(Grid) && (exclude == "" || !p.CustomName.ToLower().Contains(exclude.ToLower()))));
        }
        public void OrderRefineries()
        {
            try
            {
                if (bkLT.Count == 0) Fill<IMyRefinery>(ref bkLT, "", "", "", "shield"); bool taMT = false; while (bkLT.Count > 0 && !taMT)
                {
                    List<MyInventoryItem> items = new List<MyInventoryItem>(); bkLT[0].GetInventory(0)
.GetItems(items); if (items.Count > 1 && bkLT[0].IsFunctional) { double leadPriority = Priority(items[0]); for (int i = 1; i < items.Count; i++) { if (Priority(items[i]) > leadPriority) { taMT = true; IMyInventory newInv = bkLT[0].GetInventory(0); newInv.TransferItemFrom(newInv, 0, (items.Count - 0), false, items[0].Amount); break; } } if (!taMT) bkLT.RemoveAt(0); } else bkLT.RemoveAt(0); if (
!AvailableActions()) break;
                }
            }
            catch { }
            if (bkLT.Count == 0 || rsTb) { sM++; bkLT.Clear(); }
        }


        public void WorkIdle(bool gasGenerators, bool reactors = false, bool weapons = false, bool sifters = false)
        {
            if (bkLT.Count == 0)
            {
                if (gasGenerators) Fill<IMyGasGenerator>(ref bkLT);
                else if (reactors) Fill<IMyReactor>(ref bkLT);
                else if (weapons) GunBlocks(ref bkLT);
                else if (!sifters)
                    Fill<IMyRefinery>(ref bkLT, "", "", "", "shield", srSbTyp.ToLower());
                else Fill<IMyRefinery>(ref bkLT, "", "", srSbTyp.ToLower(), "shield");
            }
            if (bkLT.Count > 1 && wkIdx < bkLT.Count)
            {
                try
                {
                    IMyInventory origInv = bkLT[wkIdx].GetInventory(0);
                    List<MyInventoryItem> inventoryItemList = new List<MyInventoryItem>();
                    origInv.GetItems(inventoryItemList);

                    if (inventoryItemList.Count > 1 || (inventoryItemList.Count > 0 && (double)inventoryItemList[0].Amount > 1.0))
                    {
                        for (int i = idIdx; i < bkLT.Count; i++)
                        {
                            idIdx++; if (i != wkIdx)
                            {
                                IMyInventory newInv = bkLT[i].GetInventory(0); int origCount = inventoryItemList.Count; if (CrVol(newInv) == 0.0 || idleList.Contains(i))
                                {
                                    if (!idleList.Contains(i)) idleList.Add(i); if (inventoryItemList.Count > 1)
                                    {
                                        int index = inventoryItemList.Count - 1; if (bkLT[i].BlockDefinition.ToString().ToLower().Contains("furnace")) { for (int x = 0; x < inventoryItemList.Count; x++) { string iST = inventoryItemList[x].Type.SubtypeId; if (iST == "Iron" || iST == "Nickel" || iST == "Cobalt") { index = x; break; } } }
                                        VRage.MyFixedPoint aMT = (VRage.MyFixedPoint)((double)inventoryItemList[index].Amount / (double)
bkLT.Count); if (weapons) aMT = (VRage.MyFixedPoint)((int)aMT); newInv.TransferItemFrom(origInv, inventoryItemList[index], aMT);
                                    }
                                    else { VRage.MyFixedPoint aMT = (VRage.MyFixedPoint)((double)inventoryItemList[0].Amount / (double)bkLT.Count); if (weapons) aMT = (VRage.MyFixedPoint)((int)aMT); newInv.TransferItemFrom(origInv, inventoryItemList[0], aMT); }
                                }
                            }
                            if (
!AvailableActions()) break;
                        }
                    }
                    else { idIdx = 0; wkIdx++; }
                    if (idIdx >= bkLT.Count) { idIdx = 0; wkIdx++; }
                }
                catch { }
            }
            else bkLT.Clear(); if (wkIdx >= bkLT.Count || rsTb) { wkIdx = 0; bkLT.Clear(); idleList.Clear(); sM++; }
        }
        public void WorkIdleAssemblers()
        {
            try
            {
                if (bkLT.Count == 0) Fill<IMyAssembler>(ref bkLT, "", manualAssemblyKeyword.ToLower()); if (bkLT.Count > 1)
                {
                    if (!((IMyAssembler)
bkLT[wkIdx]).IsQueueEmpty)
                    {
                        List<MyProductionItem> pQ = new List<MyProductionItem>(); ((IMyAssembler)bkLT[wkIdx]).GetQueue(pQ); if (pQ.Count > 1 || (pQ.Count > 0 && (double)pQ[0].Amount >= 2.0))
                        {
                            for (int i = 0; i < bkLT.Count; i++)
                            {
                                if (i != wkIdx)
                                {
                                    if ((!bkLT[i].BlockDefinition.ToString().ToLower().Contains("survival") || (survivalKitAssembly && ((IMyAssembler)bkLT[wkIdx]).Mode ==
asMd)) && ((((IMyAssembler)bkLT[i]).IsQueueEmpty || (((IMyAssembler)bkLT[i]).Mode == ((IMyAssembler)bkLT[wkIdx]).Mode && idleList.Contains(i)))))
                                    {
                                        if (!idleList.Contains(i)) idleList.Add(i); ((IMyAssembler)bkLT[i]).Mode = ((IMyAssembler)bkLT[wkIdx]).Mode; if (pQ.Count > 1 && ((IMyAssembler)bkLT[i]).CanUseBlueprint(pQ[(pQ.Count - 1)].BlueprintId))
                                        {
                                            VRage.MyFixedPoint aMT = (
VRage.MyFixedPoint)((int)((double)pQ[(pQ.Count - 1)].Amount / (double)bkLT.Count)); if ((double)aMT < 1.0) aMT = (VRage.MyFixedPoint)1; if (MoveQueueItem((IMyAssembler)bkLT[wkIdx], (IMyAssembler)bkLT[i], pQ.Count - 1, aMT)) break;
                                        }
                                        else if (((IMyAssembler)bkLT[i]).CanUseBlueprint(pQ[0].BlueprintId))
                                        {
                                            VRage.MyFixedPoint aMT = (VRage.MyFixedPoint)((int)((double)pQ[0].Amount / (
double)bkLT.Count)); if ((double)aMT < 1.0) aMT = (VRage.MyFixedPoint)1; if (MoveQueueItem((IMyAssembler)bkLT[wkIdx], (IMyAssembler)bkLT[i], 0, aMT)) break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    wkIdx++;
                }
                else bkLT.Clear();
            }
            catch { OutP("Error working Idle Assemblers"); }
            if (wkIdx >= bkLT.Count || rsTb) { wkIdx = 0; bkLT.Clear(); idleList.Clear(); sM++; }
        }
        public bool MoveQueueItem(IMyAssembler a, IMyAssembler b,
int queueIndex, VRage.MyFixedPoint aMT)
        {
            List<MyProductionItem> pQb = new List<MyProductionItem>(); a.GetQueue(pQb); MyProductionItem item = pQb[queueIndex]; if (b.CanUseBlueprint(item.BlueprintId))
            {
                b.GetQueue(pQb); int tempA = pQb.Count; double taMT = 0; if (pQb.Count > 0) taMT = (double)pQb[pQb.Count - 1].Amount; b.AddQueueItem(item.BlueprintId, aMT); b.GetQueue(pQb); if (
pQb.Count == tempA + 1 || (double)pQb[pQb.Count - 1].Amount - (double)aMT == taMT) { RemQueueItem(a, item, queueIndex, aMT); return true; }
            }
            return false;
        }
        public void RemQueueItem(IMyAssembler a, MyProductionItem item, int queueIndex, VRage.MyFixedPoint aMT)
        {
            a.RemoveQueueItem(queueIndex, item.Amount); if ((double)aMT < (double)item.Amount) a.AddQueueItem(item.BlueprintId, (
VRage.MyFixedPoint)((double)item.Amount - (double)aMT));
        }
        public int BlueprintRequests()
        {
            int count = 0; List<string> taMT = new List<string>(); List<IMyTerminalBlock> bKS = new List<IMyTerminalBlock>(); Fill<IMyAssembler>(ref bKS, "", manualAssemblyKeyword.ToLower()); try
            {
                for (int i = 0; i < bKS.Count; i++)
                {
                    if (!((IMyAssembler)bKS[i]).IsQueueEmpty)
                    {
                        List<MyProductionItem> pQ = new List<MyProductionItem>(); ((IMyAssembler)bKS[i]).GetQueue(pQ); for (int x = 0; x < pQ.Count; x++) if (!taMT.Contains(pQ[x].BlueprintId.ToString())) { count++; taMT.Add(pQ[x].BlueprintId.ToString()); }
                    }
                }
            }
            catch { OutP("Error Caught Counting Blueprints Requested"); }
            return count;
        }
        public int UniqueBlueprintsInList(List<MyProductionItem> pQ)
        {
            int count = 0; List<string> taMT = new List<string>(); for (int x = 0; x < pQ.Count; x++) if (!taMT.Contains(pQ[x].BlueprintId.ToString())) { count++; taMT.Add(pQ[x].BlueprintId.ToString()); }
            return count;
        }
        public void SetMatchToDefinition(string itemId, double desAmt)
        {
            string iTY = ItemType(itemId); double taMT = desAmt; if (taMT < -1.0) taMT = -1.0; if (ToolType(iTY))
                SetMatchByIdAndList(itemId, ref tDL, taMT);
            else if (iTY == igtTyp) SetMatchByIdAndList(itemId, ref iDL, taMT); else if (iTY == orTyp) SetMatchByIdAndList(itemId, ref oDL, taMT); else if (iTY == cptTyp) SetMatchByIdAndList(itemId, ref cDL, taMT);
        }
        public ItemDefinition curDef; public bool NextBlueprint(ref string bST, ref int dAMT, ref bool finishedCycle,
bool disassemble = false)
        {
            double aMT = 0.0; if (nBIX == 0) hRQS = false; for (int i = nBIX; i < (tDL.Count + cDL.Count); i++)
            {
                if (i < tDL.Count) curDef = tDL.Values[i]; else curDef = cDL.Values[(i - tDL.Count)]; nBIX++; if (!disassemble) aMT = curDef.QueueMore(assemblerProductionRange); else if ((curDef.aMT - curDef.qdaMT) > curDef.dAMT) aMT = curDef.DisassembleMore(assemblerProductionRange); if (
aMT > 0.0) { dAMT = (int)aMT; bST = curDef.bST; if (!disassemble) hRQS = true; return true; }
                if (!AvailableActions()) break;
            }
            if (nBIX >= (tDL.Count + cDL.Count)) { nBIX = 0; finishedCycle = true; }
            return false;
        }
        public void SetMatchByIdAndList(string id, ref SortedList<string, ItemDefinition> list, double desAmt)
        {
            string iST = ItemSubtype(id); if (list.ContainsKey(iST))
                list[iST].dAMT = desAmt;
        }
        public void AddTempItemCount(MyInventoryItem item, double count)
        {
            string iTY = item.Type.TypeId, iST = item.Type.SubtypeId; if (ToolType(iTY) && tDL.ContainsKey(iST)) tDL[iST].taMT += count;
            else if (iTY == igtTyp && iDL.ContainsKey(iST)) iDL[iST].taMT += count;
            else if (iTY == orTyp && oDL.ContainsKey(iST)) oDL[iST].taMT += count;
            else if (iTY == cptTyp &&
cDL.ContainsKey(iST)) cDL[iST].taMT += count;
        }
        public void AddTempBlueprintCount(string bST, double count, bool disassemble)
        {
            bool found = false; for (int i = 0; i < cDL.Count; i++) if (cDL.Values[i].bST == bST) { if (!disassemble) cDL.Values[i].tqaMT += count; else cDL.Values[i].tqdaMT += count; found = true; break; }
            if (!found) for (int i = 0; i < tDL.Count; i++) if (tDL.Values[i].bST == bST)
                    {
                        if (!disassemble) tDL.Values[i].tqaMT += count; else tDL.Values[i].tqdaMT += count; break;
                    }
        }
        public void SetBlueprintCounts()
        {
            for (int i = 0; i < tDL.Count; i++) { tDL.Values[i].qAMT = 0.0 + tDL.Values[i].tqaMT; tDL.Values[i].qdaMT = 0.0 + tDL.Values[i].tqdaMT; tDL.Values[i].tqaMT = 0; tDL.Values[i].tqdaMT = 0; }
            for (int i = 0; i < cDL.Count; i++)
            {
                cDL.Values[i].qAMT = cDL.Values[i].tqaMT;
                cDL.Values[i].qdaMT = cDL.Values[i].tqdaMT; cDL.Values[i].tqaMT = 0; cDL.Values[i].tqdaMT = 0;
            }
        }
        public void SetTempItemCounts(int iC)
        {
            switch (iC)
            {
                case 0: for (int i = 0; i < tDL.Count; i++) { tDL.Values[i].aMT = 0.0 + tDL.Values[i].taMT; tDL.Values[i].taMT = 0; } break;
                case 1: for (int i = 0; i < iDL.Count; i++) { iDL.Values[i].aMT = 0.0 + iDL.Values[i].taMT; iDL.Values[i].taMT = 0; } break;
                case 2: for (int i = 0; i < oDL.Count; i++) { oDL.Values[i].aMT = 0.0 + oDL.Values[i].taMT; oDL.Values[i].taMT = 0; } break;
                case 3: for (int i = 0; i < cDL.Count; i++) { cDL.Values[i].aMT = 0.0 + cDL.Values[i].taMT; cDL.Values[i].taMT = 0; } break;
            }
        }
        public double RequestItemInfo(MyInventoryItem item, bool itemRatio, bool aMT = false)
        {
            string iST = item.Type.SubtypeId, iTY = item.Type.TypeId; if (
iTY == cptTyp && cDL.ContainsKey(iST)) { if (itemRatio) return cDL[iST].vRt; else if (aMT) return cDL[iST].aMT; }
            if (ToolType(iTY) && tDL.ContainsKey(iST)) { if (itemRatio) return tDL[iST].vRt; else if (aMT) return tDL[iST].aMT; }
            if (iTY == igtTyp && iDL.ContainsKey(iST)) { if (itemRatio) return iDL[iST].vRt; else if (aMT) return iDL[iST].aMT; }
            if (iTY == orTyp && oDL.ContainsKey(iST))
            {
                if (
itemRatio) return oDL[iST].vRt;
                else if (aMT) return oDL[iST].aMT;
            }
            if (itemRatio) return 15.0; return 0.0;
        }
        public List<string> RequestItemInfo(string iTY, string iST)
        {
            List<string> oNs = new List<string>();
            if (iTY == cptTyp && cDL.ContainsKey(iST)) oNs.Add(cDL[iST].oN);
            if (ToolType(iTY) && tDL.ContainsKey(iST)) oNs.Add(tDL[iST].oN);
            if (iTY == igtTyp && iDL.ContainsKey(iST))
                oNs.Add(iDL[iST].oN); if (iTY == orTyp && oDL.ContainsKey(iST)) oNs.Add(oDL[iST].oN);
            return oNs;

        }
        public void AddItemDef(string itemName, string iST, string iTY, string bST, double dAMT, double vRt = 0.37)
        {
            ItemDefinition def = new ItemDefinition(); def.iST = iST; def.iTY = iTY; def.bST = bST; def.dAMT = dAMT; def.oN = itemName; def.vRt = vRt; AddItemDefinition(def);
        }


        public void AddItemDefinition(ItemDefinition def, bool update = false)
        {
            bool found = false; if (def.iTY == cptTyp) { if (update) { if (cDL.ContainsKey(def.iST)) { cDL[def.iST].oN = def.oN; cDL[def.iST].fuel = def.fuel; cDL[def.iST].vRt = def.vRt; found = true; } } if (!found) cDL[def.iST] = def; }
            else if (def.iTY == igtTyp)
            {
                if (update)
                {
                    if (iDL.ContainsKey(def.iST))
                    {
                        iDL[def.iST].oN = def.oN;
                        iDL[def.iST].fuel = def.fuel; iDL[def.iST].vRt = def.vRt; found = true;
                    }
                }
                if (!found) iDL[def.iST] = def;
            }
            else if (ToolType(def.iTY)) { if (update) { if (tDL.ContainsKey(def.iST)) { tDL[def.iST].oN = def.oN; tDL[def.iST].fuel = def.fuel; tDL[def.iST].vRt = def.vRt; found = true; } } if (!found) tDL[def.iST] = def; }
            else if (def.iTY == orTyp)
            {
                if (update)
                {
                    if (oDL.ContainsKey(def.iST))
                    {
                        oDL[def.iST].oN = def.oN; oDL[def.iST].fuel = def.fuel; oDL[def.iST].vRt = def.vRt; found = true;
                    }
                }
                if (!found) oDL[def.iST] = def;
            }
            else { if (update) { if (cmDL.ContainsKey(def.iST)) { cmDL[def.iST].oN = def.oN; cmDL[def.iST].fuel = def.fuel; cmDL[def.iST].vRt = def.vRt; found = true; } } if (!found) cmDL[def.iST] = def; }
        }
        public void FillDict()
        {
            tDL.Clear(); iDL.Clear(); oDL.Clear(); cDL.Clear();
            AddItemDef("Bulletproof Glass","BulletproofGlass", cptTyp, "BulletproofGlass", 0, 8);
            AddItemDef("Canvas", "Canvas", cptTyp, "Canvas", 0, 8);
            AddItemDef("Computer", "Computer", cptTyp, "ComputerComponent", 0, 1);
            AddItemDef("Construction Comp", "Construction", cptTyp, "ConstructionComponent", 0, 2);
            AddItemDef("Detector Comp", "Detector", cptTyp,"DetectorComponent", 0, 6);
            AddItemDef("Display", "Display", cptTyp, "Display", 0, 6); AddItemDef("Explosives", "Explosives", cptTyp, "ExplosivesComponent", 0, 2);
            AddItemDef("Girder", "Girder", cptTyp, "GirderComponent", 0, 2);
            AddItemDef("Gravity Gen. Comp", "GravityGenerator", cptTyp, "GravityGeneratorComponent", 0, 200);
            AddItemDef("Interior Plate", "InteriorPlate", cptTyp, "InteriorPlate", 0, 5);
            AddItemDef("Large Steel Tube", "LargeTube", cptTyp, "LargeTube", 0, 38);
            AddItemDef("Medical Comp", "Medical", cptTyp, "MedicalComponent", 0, 160);
            AddItemDef("Metal Grid", "MetalGrid", cptTyp, "MetalGrid", 0, 15);
            AddItemDef("Motor", "Motor", cptTyp, "MotorComponent", 0, 8);
            AddItemDef("Power Cell", "PowerCell", cptTyp, "PowerCell", 0, 45);
            AddItemDef("Radio Comm. Comp", "RadioCommunication", cptTyp, "RadioCommunicationComponent", 0, 70);
            AddItemDef("Reactor Comp", "Reactor", cptTyp, "ReactorComponent", 0, 8);
            AddItemDef("Small Steel Tube", "SmallTube", cptTyp, "SmallTube", 0, 2);
            AddItemDef("Solar Cell", "SolarCell", cptTyp, "SolarCell", 0, 20);
            AddItemDef("Steel Plate", "SteelPlate", cptTyp, "SteelPlate", 0, 3);
            AddItemDef("Superconductor", "Superconductor", cptTyp, "Superconductor", 0, 8);
            AddItemDef("Thruster Comp", "Thrust", cptTyp, "ThrustComponent", 0, 10);
            AddItemDef("Automatic Rifle", "AutomaticRifleItem", tlTyp, "AutomaticRifle", 0, 14);
            AddItemDef("Precise Rifle", "PreciseAutomaticRifleItem", tlTyp, "PreciseAutomaticRifle", 0, 14);
            AddItemDef("Rapid Fire Rifle", "RapidFireAutomaticRifleItem", tlTyp, "RapidFireAutomaticRifle", 0, 14);
            AddItemDef("Ultimate Rifle", "UltimateAutomaticRifleItem", tlTyp, "UltimateAutomaticRifle", 0, 14);
            AddItemDef("Welder 1", "WelderItem", tlTyp, "Welder", 0, 8);
            AddItemDef("Welder 2", "Welder2Item", tlTyp, "Welder2", 0, 8);
            AddItemDef("Welder 3", "Welder3Item", tlTyp, "Welder3", 0, 8);
            AddItemDef("Welder 4", "Welder4Item", tlTyp, "Welder4", 0, 8);
            AddItemDef("Grinder 1", "AngleGrinderItem", tlTyp, "AngleGrinder", 0, 20);
            AddItemDef("Grinder 2", "AngleGrinder2Item", tlTyp, "AngleGrinder2", 0, 20);
            AddItemDef("Grinder 3", "AngleGrinder3Item", tlTyp, "AngleGrinder3", 0, 20);
            AddItemDef("Grinder 4", "AngleGrinder4Item", tlTyp, "AngleGrinder4", 0, 20);
            AddItemDef("Drill 1", "HandDrillItem", tlTyp, "HandDrill", 0, 25);
            AddItemDef("Drill 2", "HandDrill2Item", tlTyp, "HandDrill2", 0, 25);
            AddItemDef("Drill 3", "HandDrill3Item", tlTyp, "HandDrill3", 0, 25);
            AddItemDef("Drill 4", "HandDrill4Item", tlTyp, "HandDrill4", 0, 25);
             AddItemDef("Oxygen Bottle", "OxygenBottle", oxBtTyp, "OxygenBottle", 0, 120);
            AddItemDef("Hydrogen Bottle", "HydrogenBottle", hdBtTyp, "HydrogenBottle", 0, 120);
            AddItemDef("NATO 5.56x45mm", "NATO_5p56x45mm", amTyp, "NATO_5p56x45mmMagazine", 0, 0.2);
            AddItemDef("NATO 25x184mm", "NATO_25x184mm",amTyp, "NATO_25x184mmMagazine", 0, 16);
            AddItemDef("Missile 200mm", "Missile200mm", amTyp, "Missile200mm", 0, 60);
            AddItemDef("Cobalt Ore", "Cobalt", orTyp, "None", 0); AddItemDef("Gold Ore", "Gold", orTyp, "None", 0);
            AddItemDef("Ice", "Ice", orTyp, "None", 0); AddItemDef("Iron Ore", "Iron", orTyp, "None", 0);
            AddItemDef("Magnesium Ore", "Magnesium", orTyp,"None", 0);
            AddItemDef("Nickel Ore", "Nickel", orTyp, "None", 0);
            AddItemDef("Platinum Ore", "Platinum", orTyp, "None", 0);
            AddItemDef("Scrap Ore", "Scrap", orTyp, "None", 0);
            AddItemDef("Silicon Ore", "Silicon", orTyp, "None", 0);
            AddItemDef("Silver Ore", "Silver", orTyp, "None", 0);
            AddItemDef("Stone", "Stone", orTyp, "None", 0);
            AddItemDef("Uranium Ore", "Uranium", orTyp, "None", 0);
            AddItemDef("Gravel", "Stone", igtTyp, "None", 0, 0.37);
            AddItemDef("Magnesium Powder", "Magnesium", igtTyp, "None", 0, 0.575);

            AddItemDef("Cobalt Ingot", "Cobalt", igtTyp, "None", 0, 0.112);
            AddItemDef("Gold Ingot", "Gold", igtTyp, "None", 0, 0.052);
            AddItemDef("Iron Ingot", "Iron", igtTyp, "None", 0, 0.127);
            AddItemDef("Nickel Ingot", "Nickel", igtTyp, "None", 0, 0.112);
            AddItemDef("Platinum Ingot", "Platinum", igtTyp, "None", 0, 0.047);
            AddItemDef("Silicon Wafer", "Silicon", igtTyp, "None", 0, 0.429);
            AddItemDef("Silver Ingot", "Silver", igtTyp, "None", 0, 0.095);
            AddItemDef("Uranium Ingot", "Uranium", igtTyp, "None", 0, 0.052);
        }
        public void EmptyUnusedAssemblers()
        {
            if (DateTime.Now >= eAsTm)
            {
                try
                {
                    if (bkLT.Count == 0 || !(bkLT[0] is IMyAssembler)) { Fill<IMyAssembler>(ref bkLT, "", manualAssemblyKeyword.ToLower()); assCount = bkLT.Count; } while (bkLT.Count > 0)
                    {
                        IMyAssembler block = (IMyAssembler)bkLT[0];
                        bool tmpB = EyAss(bkLT[0]);
                        List<MyProductionItem> queueList = new List<MyProductionItem>();
                        block.GetQueue(queueList);
                        bool readyForNext = true,
                             tmpC = false;
                        if (block.IsQueueEmpty || block.Mode == dsMd || tmpB)
                        {
                            IMyInventory origInv = bkLT[0].GetInventory(0); List<MyInventoryItem> inventoryItemList = new List<MyInventoryItem>(); origInv.GetItems(inventoryItemList); int origCount = inventoryItemList.Count, checkedItems = 0, tmpA = 0;
                            for (int i = 0; i < inventoryItemList.Count; i++)
                            {
                                checkedItems++;
                                if (FindItemHome(bkLT[0], 0, inventoryItemList[i], ref tmpC)) break;
                            }

                            if (block.IsQueueEmpty || tmpB || block.Mode == asMd)
                            {
                                origInv = bkLT[0].GetInventory(1); origInv.GetItems(inventoryItemList); tmpA += inventoryItemList.Count; for (int i = 0; i < inventoryItemList.Count; i++)
                                {
                                    checkedItems++;
                                    if (FindItemHome(bkLT[0], 1, inventoryItemList[i], ref tmpC)) break;
                                }
                            }
                            if (checkedItems < origCount + tmpA) readyForNext = false;
                        }
                        if (readyForNext) bkLT.RemoveAt(0);
                        if (!AvailableActions()) break;
                    }
                    if (bkLT.Count == 0 || rsTb)
                    {
                        bkLT.Clear();
                        sM++;
                        eAsTm = DateTime.Now.AddSeconds(emptyAssemblerDelay);
                    }
                }
                catch { OutP("Error Emptying Assemblers"); }
            }
            else sM++;
        }


        public void MoveBadRequests()
        {
            try
            {
                if (bkLT.Count == 0)
                    Fill<IMyAssembler>(ref bkLT, "", manualAssemblyKeyword.ToLower()); if (bkLT.Count > 0)
                {
                    IMyAssembler block = ((IMyAssembler)bkLT[0]); List<MyProductionItem> quLst = new List<MyProductionItem>(); block.GetQueue(quLst); if (quLst.Count == 0 && block.Mode == dsMd) block.Mode = asMd;
                    else
                    {
                        if (block.Mode == asMd && block.CurrentProgress < 0.1F)
                        {
                            if (quLst.Count > 1)
                            {
                                double leadPriority = -1;
                                int index = 0; for (int x = 0; x < quLst.Count; x++) { try { double tmpA = Priority(quLst[x].BlueprintId); if (leadPriority == -1 || tmpA > leadPriority) { index = x; leadPriority = tmpA; } } catch { } }
                                if (index != 0) block.MoveQueueItemRequest(0, quLst.Count - 1);
                            }
                        }
                    }
                    bkLT.RemoveAt(0);
                }
            }
            catch { OutP("Error Checking Requests"); if (bkLT.Count > 0) bkLT.RemoveAt(0); }
            if (bkLT.Count == 0 || rsTb)
            {
                bkLT.Clear()
; sM++;
            }
        }
        /// <summary>
        /// Empty Assemblers
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool EyAss(IMyTerminalBlock block)
        {
            try { float tmpA = ((IMyAssembler)block).CurrentProgress; return ((((IMyAssembler)block).Mode == asMd && tmpA < 0.05F) || (((IMyAssembler)block).Mode == dsMd && tmpA < 0.01)) && (CrVol(block.GetInventory(0)) / MxVol(block.GetInventory(0)) >= 0.95 || CrVol(block.GetInventory(1)) / MxVol(block.GetInventory(1)) >= 0.95); }
            catch
            {
                Echo(
"Error determining whether to empty assembler");
            }
            return true;
        }


        public List<MyInventoryItem> invItemList = new List<MyInventoryItem>();

        public void ScourInventories(bool distribute)
        {
            try
            {
                if (DateTime.Now >= scourTime && ((!distribute && aBL.Count > 0) || (distribute && cIDS.Count > 0)))
                {
                    if (!distribute) tBlk = aBL[scrIx];
                    else tBlk = aBL[cIDS[scrIx]];
                    try
                    {
                        IMyInventory origInv = tBlk.GetInventory(0);
                        if (invItemList.Count == 0)
                        {
                            if (curInv == 1 && tBlk.InventoryCount > 1)
                            {
                                origInv = tBlk.GetInventory(1); origInv.GetItems(invItemList);
                            }
                            else origInv.GetItems(invItemList);
                        }
                        while (invItemList.Count > 0)
                        {
                            string iTY = invItemList[0].Type.TypeId, iST = invItemList[0].Type.SubtypeId;
                            try
                            {
                                RecordItem(invItemList[0], tBlk, origInv);
                            }
                            catch { }
                            if (!distribute && ((curInv == 1 && !(tBlk is IMyAssembler)) || NeedsHome(invItemList[0], tBlk, curInv)))
                            {
                                bool tmpA = false;
                                FindItemHome(tBlk, curInv, invItemList[0], ref tmpA);
                                if (!tmpA) invItemList.RemoveAt(0);
                            }
                            else if (distribute && Distributable(invItemList[0], tBlk)) { if (DistributeItems(origInv, invItemList[0])) invItemList.RemoveAt(0); }
                            else invItemList.RemoveAt(0);
                            if (!AvailableActions()) break;
                        }
                    }
                    catch
                    {
                        curInv = 0; scrIx++;
                        invItemList.Clear();
                        OutP("Error Caught Scouring Inventory, Skipping This One");
                    }
                    if (invItemList.Count == 0)
                    {
                        if (curInv == 0 && tBlk.InventoryCount > 1 && !distribute) curInv = 1;
                        else { scrIx++; curInv = 0; }
                    }
                    if ((!distribute && scrIx >= aBL.Count) || (distribute && scrIx >= cIDS.Count) || rsTb)
                    {
                        if (distribute) flCns = false; scrIx = 0;
                        curInv = 0; invItemList.Clear();
                        if (distribute)
                        {
                            scourTime = DateTime.Now.AddSeconds(sortAndDistributeDelay);
                            recordItems = !recordItems;
                        }
                        sM++;
                        bkLT.Clear();
                    }
                }
                else sM++;
            }
            catch { OutP("Error Scouring Inventories"); }
        }
        public bool CanType(string iTY) { return (iTY == oxBtTyp || iTY == hdBtTyp); }
        public bool LoadoutHome(string customData, string iTY, string iST)
        {
            if (
!customData.Contains("{")) return false; bool inGT = false, cmPT = false, oRE = false, tOL = false; List<string> oNs = RequestItemInfo(iTY, iST); List<string> tags = new List<string>(); GetTags(ref tags, customData, "{", "}"); for (int i = 0; i < tags.Count; i++)
            {
                string tag = tags[i]; StockTags(ref tag, ref inGT, ref cmPT, ref oRE, ref tOL); int index = tag.IndexOf(":") + 1; if (
tag.Contains(":")) tag = tag.Substring(index, tag.Length - index); if (((iTY == igtTyp && inGT) || (iTY == cptTyp && cmPT) || (iTY == orTyp && oRE) || (ToolType(iTY) && tOL)) && (tag == "*" || Same(oNs, tag))) return true;
            }
            return false;
        }
        public bool Same(List<string> lst, string tag)
        {
            foreach (string s in lst) if (s.ToLower().Replace(" ", "").StartsWith(tag)) return true; return false;
        }
        public bool Discount(IMyTerminalBlock block, MyInventoryItem item) { if (!countLoadoutItems && block.CustomData.Contains("{") && LoadoutHome(block.CustomData, item.Type.TypeId, item.Type.SubtypeId)) return true; return false; }
        public bool NeedsHome(MyInventoryItem item, IMyTerminalBlock block, int curInv)
        {
            try
            {
                string iTY = item.Type.TypeId,
                iST = item.Type.SubtypeId;
                string customData = block.CustomData.ToLower();
                bool ldOt = customData.Contains("{");
                if (CanType(iTY) && flCns && !(block is IMyGasTank) && !(block is IMyGasGenerator)) return true;
                if (ldOt && LoadoutHome(customData, iTY, iST)) return false;
                RemoveTags(ref customData); if (block is IMyReactor) return false;
                if (iST == "Ice" && block is IMyGasGenerator) return false;
                if (IsGun(block)) return false;
                if (iTY == orTyp && (block is IMyRefinery) && !block.BlockDefinition.ToString().ToLower().Contains(srSbTyp.ToLower())) return false;
                if (iTY == igtTyp && iST == "Stone" && block.BlockDefinition.ToString().ToLower().Contains(srSbTyp.ToLower())) return false;
                if ((block is IMyAssembler) && block.BlockDefinition.ToString().ToLower()
                                            .Contains("survival") && (iTY == orTyp)) return false;
                if ((block is IMyAssembler) && (iTY == igtTyp && ((IMyAssembler)block).Mode == asMd)) return false;
                if ((block is IMyAssembler) && ((IMyAssembler)block).Mode == dsMd && (ToolType(iTY) || iTY == cptTyp)) return false;
                if (block is IMyShipWelder)
                {
                    if (!block.IsWorking) return true;
                    else
                    {
                        if (block.BlockDefinition.ToString().ToLower()
.Contains("nanite")) { if (iTY == orTyp || iTY == tlTyp || iTY == igtTyp) return true; else return false; }
                        else return false;
                    }
                }
                if ((iTY == igtTyp && customData.Contains(ingotStorageKeyword.ToLower())) || (iTY == orTyp && customData.Contains(oreStorageKeyword.ToLower())) || (iTY == cptTyp && customData.Contains(componentStorageKeyword.ToLower())) || (ToolType(iTY) && customData.Contains(
toolStorageKeyword.ToLower()))) return false; if (!(block is IMyCargoContainer)) return true; if (iTY == igtTyp) return (cIDS.Count != iSB.Count); if (iTY == orTyp) return (cIDS.Count != oSB.Count); if (iTY == cptTyp) return (cIDS.Count != cSB.Count); if (ToolType(iTY)) return (cIDS.Count != tSB.Count);
            }
            catch
            {
                OutP("Error Checking If Item Needs Home: " + item.Type.ToString());
            }
            return false;
        }
        public bool IsGun(IMyTerminalBlock block) { if ((block is IMySmallGatlingGun) || (block is IMyLargeGatlingTurret) || (block is IMyLargeMissileTurret) || (block is IMySmallMissileLauncherReload) || (block is IMySmallMissileLauncher)) return true; return false; }
        public bool ToolType(string iTY)
        {
            string iTYB = iTY.ToLower(); if (iTYB == tlTyp.ToLower() || iTYB ==
hdBtTyp.ToLower() || iTYB == oxBtTyp.ToLower() || iTYB == amTyp.ToLower()) return true; return false;
        }


        public bool Distributable(MyInventoryItem item, IMyTerminalBlock block)
        {
            try
            {
                string iTY = item.Type.TypeId, iST = item.Type.SubtypeId;
                if (!(block is IMyCargoContainer)) return false;
                if (!block.CustomData.ToLower().Contains("distribute") && !distributeLoadoutItems &&
                    LoadoutHome(block.CustomData.ToLower(), iST, iTY)) return false;
                if (iTY == orTyp && iST == "Stone" && !refineStone) return false;
                if ((iTY == amTyp && amPBK.Count > 0) ||
                    (iST == "Ice" && icPB.Count > 0) || 
                    (iTY == orTyp && orPB.Count > 0) ||
                    (Fuel(iTY + "/" + iST) && rcBK.Count > 0)) return true;
                if (iTY == igtTyp && iST == "Stone" && stBK.Count > 0) return true;
            }
            catch
            {
                OutP("Error Checking If Item Is Distributable: " + item.Type.ToString());
            }
            return false;
        }
        public bool FindItemHome(IMyTerminalBlock block, int inv, MyInventoryItem item, ref bool tmpB)
        {
            try
            {
                IMyInventory origInv = block.GetInventory(inv); string iTY = item.Type.TypeId, iST = item.Type.SubtypeId; bool tmpA = CanType(iTY); if (CanType(iTY) && flCns && !(block is IMyGasTank) && !(
block is IMyGasGenerator)) FillCanister(origInv, item);
                else return SortAway(origInv, item, iST, ref tmpB);
            }
            catch { OutP("Error Finding Item Home"); }
            return false;
        }


        public void FillCanister(IMyInventory origInv, MyInventoryItem item)
        {
            string iTY = item.Type.TypeId; List<IMyTerminalBlock> bKS = new List<IMyTerminalBlock>();
            double tmpA = 0; if (iTY == oxBtTyp)
            {
                gSys.GetBlocksOfType<IMyGasTank>(bKs, (b => b.IsFunctional && b.CubeGrid == Grid && !b.BlockDefinition.ToString().ToLower().Contains("hydrogen")));
                if (bKS.Count == 0) Fill<IMyGasTank>(ref bKS, "", "", "", "hydrogen");
            }
            else
            {
                gSys.GetBlocksOfType<IMyGasTank>(bKs, (b => b.IsFunctional && b.CubeGrid == Grid && b.BlockDefinition.ToString().ToLower().Contains("hydrogen")));
                if (bKS.Count == 0) Fill<IMyGasTank>(ref bKS, "", "", "hydrogen", "");
            }
            if (bKS.Count == 0)
            {
                gSys.GetBlocksOfType<IMyGasGenerator>(bKs, (b => b.IsFunctional && b.CubeGrid == Grid));
                if (bKS.Count == 0) Fill<IMyGasGenerator>(ref bKS); }
            for (int i = 0; i < bKS.Count; i++)
            {
                if ((bKS[i] is IMyGasGenerator) || ((IMyGasTank)bKS[i]).FilledRatio > 0.0)
                {
                    if (TPot(bKS[i].GetInventory(0), origInv, item,
1.0, ref tmpA)) { OutP("Tried To Fill: " + ItemSubtype(item.ToString()) + " In: " + bKS[i].CustomName); if (bKS[i] is IMyGasTank) ((IMyGasTank)bKS[i]).RefillBottles(); break; }
                }
            }
        }

        public bool SortAway(IMyInventory origInv, MyInventoryItem item, string iST, ref bool tmpA)
        {
            string iTY = item.Type.TypeId;
            if (iTY == igtTyp) return SortAway(origInv, item, iST, iSB, ref tmpA);
            else if (iTY == orTyp) return SortAway(origInv, item, iST, oSB, ref tmpA);
            else if (ToolType(iTY)) return SortAway(origInv, item, iST, tSB, ref tmpA);
            else if (iTY == cptTyp) return SortAway(origInv, item, iST, cSB, ref tmpA);
            return false;
        }

        public bool SortAway(IMyInventory origInv, MyInventoryItem item, string iST, List<int> taMT, ref bool tmpA)
        {
            double movedAmount = 0.0; double wholeAmount = (double)item.Amount; for (int i = stIx; i < taMT.Count; i++)
            {
                stIx++; IMyInventory inv = aBL[taMT[i]].GetInventory(0);
                if (!inv.IsFull && TPot(inv, origInv, item, 0.0, ref movedAmount))
                {
                    string itemName = item.Type.ToString(); if (!outIdList.Contains(itemName))
                    {
                        OutP("Tried To Sort: " + ShortNum(movedAmount, true) + " " + iST + " Into: " + aBL[taMT[i]].CustomName + " From: " + ((IMyTerminalBlock)origInv.Owner).CustomName);
                        outIdList.Add(itemName);
                    }
                    if (movedAmount >= wholeAmount)
                    { stIx = 0; return true; }
                }
                if (!AvailableActions())
                {
                    tmpA = true; break;
                }
            }
            if (stIx >= taMT.Count)
            {
                stIx = 0; tmpA = false;
            }
            return false;
        }



        public bool ResearchInventory()
        {
            try
            {
                if (itRH)
                {
                    itRH = false; aBL.Clear(); icPB.Clear(); orPB.Clear();
                    stBK.Clear(); amPBK.Clear(); rcBK.Clear(); tSB.Clear(); oSB.Clear(); iSB.Clear(); cSB.Clear(); cIDS.Clear(); aBL = BlocksWithInventory(); Fill<IMyCargoContainer>(ref bKs); for (int i = 0; i < bKs.Count; i += 0) { if (!sBks.ContainsKey(bKs[i].CustomName)) { sBks[bKs[i].CustomName] = bKs[i]; bKs.RemoveAt(i); } else i++; }
                    for (int i = 0; i < sBks.Values.Count; i++) aBL.Add(sBks.Values[i]);
                    sBks.Clear(); for (int i = 0; i < bKs.Count; i++) aBL.Add(bKs[i]); bKs.Clear();
                }
                if (ivBkCt != aBL.Count)
                    SetConveyorUsage(!conveyorControl);
                if (GetIndices())
                {
                    itRH = true;
                    ivBkCt = aBL.Count;
                    return true; }
            }
            catch { }
            return false;
        }
        public double CntInInv(string itemId, IMyInventory origInv)
        {
            int tmpA = 0;
            return CntInInv(itemId, origInv, ref tmpA);
        }
        public double CntInInv(string itemId, IMyInventory origInv, ref int index)
        {
            double count = 0;
            List<MyInventoryItem> inventoryItemList = new List<MyInventoryItem>();
            origInv.GetItems(inventoryItemList, (t => t.Type.ToString() == itemId));
            for (int i = 0; i < inventoryItemList.Count; i++) count += (double)inventoryItemList[i].Amount; for (int i = 0; i < origInv.ItemCount; i++)
            {
                MyInventoryItem itm = (MyInventoryItem)origInv.GetItemAt(i); if (itm != null && itm.Type.ToString() == itemId) { index = i; break; }
            }
            return count;
        }
        public void GunBlocks(ref List<IMyTerminalBlock> list) { list.Clear(); for (int i = 0; i < amPBK.Count; i++) list.Add(aBL[amPBK[i]]); }


        public bool Fuel(string itemId)
        {
            if (itemId == (igtTyp + "/Uranium")) return true;
            else foreach (string s in fIdLT) if (s == itemId)
                        return true; return false;
        }

        //string itemId = 
        public bool DistributeItems(IMyInventory origInv, MyInventoryItem item)
        {
            string iST = item.Type.SubtypeId, itemId = item.Type.ToString(); bool taMT = false, tmpA = false, tmpC = Fuel(itemId); try
            {
                string iTY = item.Type.TypeId; if (bkLT.Count == 0)
                {
                    if (iST == "Ice") Fill<IMyGasGenerator>(ref bkLT);
                    else if (iTY == orTyp) Fill<IMyRefinery>(ref bkLT, "", "","", "shield", srSbTyp.ToLower());
                    else if (tmpC) Fill<IMyReactor>(ref bkLT);
                    else if (iTY == amTyp) GunBlocks(ref bkLT);
                    else if
                        (iTY == igtTyp && iST == "Stone") Fill<IMyRefinery>(ref bkLT, "", "", srSbTyp.ToLower());
                    dCt = bkLT.Count;
                }
                if (bkLT.Count > 0)
                {
                    double tmpB = RequestItemInfo(item, false, true);
                    double tmpD = tmpB / (double)dCt;
                    double itemAmt = (double)item.Amount,totalOre = 0, movedAmount = 0;
                    if (tmpC) tmpD = fuelQuota;
                    else if (iTY == amTyp) tmpD = (double)ammoQuota; else if (iST == "Ice") tmpD = iceQuota;
                    while (bkLT.Count > 0)
                    {
                        try
                        {
                            IMyInventory inv = bkLT[0].GetInventory(0); if (bkLT[0].BlockDefinition.ToString().ToLower().Contains("furnace")) tFnCt++; int index = -1; double amt = CntInInv(itemId, inv, ref index); if (iTY == orTyp && iST != "Ice")
                            {
                                if (iST != "Iron" && iST != "Nickel" && iST != "Cobalt") tmpD = tmpB / ((double)dCt - fnCt); totalOre = (MxVol(inv) * 1000.0) / 0.37; totalOre = totalOre / ((double)oDL.Count - 1.0); if (tmpD > totalOre) tmpD = totalOre;
                            }
                            if (amt < tmpD)
                            {
                                amt = tmpD - amt; if (amt > itemAmt || (amt < 1.0 && ToolType(iTY))) amt = itemAmt; if (iTY != igtTyp && iTY != orTyp) { amt = (double)((int)amt); if ((double)amt < 1.0) amt = 1.0; }
                                if (amt > 0.0 && TPot(inv, origInv, item, amt, ref movedAmount))
                                {
                                    taMT = true; itemAmt -= movedAmount; movedAmount = 0;
                                }
                            }
                            else if (index != -1 && amt > tmpD * 1.15 && !LoadoutHome(bkLT[0].CustomData.ToLower(), iTY, iST)) { MyInventoryItem itm = (MyInventoryItem)inv.GetItemAt(index);
                                if (itm != null) SortAway(inv, itm, iST, ref tmpC); }
                        }
                        catch
                        {
                            OutP("Error caught distributing to " +
bkLT[0].CustomName);
                        }
                        bkLT.RemoveAt(0); if (!AvailableActions()) break;
                    }
                }
            }
            catch { OutP("Error Caught Distributing Items"); }
            if (taMT && !otDtLst.Contains(itemId) && bkLT.Count == 0) { otDtLst.Add(itemId); OutP("Tried To Distribute: " + iST + " From: " + ((IMyTerminalBlock)origInv.Owner).CustomName); }
            if (bkLT.Count == 0) { tmpA = true; fnCt = tFnCt; tFnCt = 0; }
            return tmpA;
        }
        public bool TPot(IMyInventory newInv, IMyInventory origInv, MyInventoryItem inventoryItem, double dAmt, ref double movedAmount)
        {
            bool tAble = true, wholeMove = dAmt == 0.0 || (double)inventoryItem.Amount <= dAmt; double tAmt = 1.0, mAmt = dAmt; double wholeAmount = (double)inventoryItem.Amount; if (mAmt > 0.0 && mAmt > wholeAmount) mAmt = wholeAmount; try
            {
                if ((double)
inventoryItem.Amount < mAmt) mAmt = (double)inventoryItem.Amount; if (dAmt > 0.0) { tAble = CanAddItem(inventoryItem, mAmt, newInv, origInv, ref tAmt); if (!tAble && tAmt > 0.0 && tAmt < mAmt) mAmt = tAmt; }
                else if (dAmt == 0.0)
                {
                    tAble = CanAddItem(inventoryItem, wholeAmount, newInv, origInv, ref tAmt); if (!tAble && tAmt > 0.0)
                    {
                        tAble = CanAddItem(inventoryItem, tAmt, newInv, origInv,
ref tAmt); wholeMove = false;
                    }
                }
                if (tAmt == 0.0) return false; if (tAble)
                {
                    if (dAmt > 0.0 && newInv.TransferItemFrom(origInv, inventoryItem, (VRage.MyFixedPoint)mAmt)) { movedAmount += mAmt; return true; }
                    if (wholeMove && newInv.TransferItemFrom(origInv, inventoryItem, inventoryItem.Amount)) { movedAmount += wholeAmount; return true; }
                    int index = 0; for (int i = 0; i < origInv.ItemCount; i++)
                    {
                        MyInventoryItem itm = (MyInventoryItem)origInv.GetItemAt(i); if (itm != null && itm.Type.ToString() == inventoryItem.Type.ToString()) { index = i; break; }
                    }
                    VRage.MyFixedPoint tmpB = (VRage.MyFixedPoint)1; int cnt = newInv.ItemCount; if (wholeMove && (origInv.TransferItemTo(newInv, index) || origInv.TransferItemTo(newInv, index, 0) || origInv.TransferItemTo(newInv, index, cnt - 1)
|| origInv.TransferItemTo(newInv, index, 0, true) || origInv.TransferItemTo(newInv, index, cnt - 1, true) || origInv.TransferItemTo(newInv, index, 0, false) || origInv.TransferItemTo(newInv, index, cnt - 1, false) || origInv.TransferItemTo(newInv, index, 0, true, inventoryItem.Amount) || origInv.TransferItemTo(newInv, index, cnt - 1, true, inventoryItem.Amount)
|| origInv.TransferItemTo(newInv, index, 0, false, inventoryItem.Amount) || origInv.TransferItemTo(newInv, index, cnt - 1, false, inventoryItem.Amount))) { movedAmount += wholeAmount; return true; }
                    if ((wholeMove || mAmt <= 1.0) && (origInv.TransferItemTo(newInv, index, 0, true, tmpB) || origInv.TransferItemTo(newInv, index, cnt - 1, true, tmpB) || origInv.TransferItemTo(
newInv, index, 0, false, tmpB) || origInv.TransferItemTo(newInv, index, cnt - 1, false, tmpB))) { movedAmount += 1.0; return true; }
                }
            }
            catch { OutP("Error Transferring Item: " + inventoryItem.Type.ToString()); }
            return false;
        }
        public bool CanAddItem(MyInventoryItem item, double aMT, IMyInventory newInv, IMyInventory origInv, ref double usableAmount)
        {
            try
            {
                if (
!newInv.IsConnectedTo(origInv)) { usableAmount = 0.0; OutP(((IMyTerminalBlock)newInv.Owner).CustomName + " is not connected to " + ((IMyTerminalBlock)origInv.Owner).CustomName); return false; }
                if (!origInv.CanTransferItemTo(newInv, item.Type))
                {
                    usableAmount = 0.0; OutP(item.Type.SubtypeId + " cannot be transferred from " + ((IMyTerminalBlock)origInv.Owner).CustomName +
" to " + ((IMyTerminalBlock)newInv.Owner).CustomName); return false;
                }
                double curr = CrVol(newInv) * 1000.0, maxx = MxVol(newInv) * 1000.0, voll = RequestItemInfo(item, true); if ((aMT * voll) <= (maxx - curr)) return true; else { string iTY = item.Type.TypeId; if (iTY == igtTyp || iTY == orTyp) usableAmount = (((maxx - curr) / voll)); else usableAmount = (double)((int)(((maxx - curr) / voll))); }
            }
            catch { OutP("Error Checking Transferability Of: " + item.Type.ToString()); }
            return false;
        }
        public void ResetRuntimes()
        {
            Runtime.UpdateFrequency = UpdateFrequency.None; Runtime.UpdateFrequency &= ~UpdateFrequency.Update1; Runtime.UpdateFrequency &= ~UpdateFrequency.Update10; Runtime.UpdateFrequency &= ~UpdateFrequency.Update100; if (updateFrequency == 100)
                Runtime.UpdateFrequency = UpdateFrequency.Update100;
            else if (updateFrequency == 1) Runtime.UpdateFrequency = UpdateFrequency.Update1; else Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }
        public string Convert(ItemDefinition def)
        {
            string tmpA = "Custom|" + def.oN + "|" + def.iTY + "|" + def.iST + "|" + def.bST + "|" + def.vRt.ToString("N6"); if (def.fuel) tmpA += "|Fuel"; tmpA += ";";
            return tmpA;
        }
        public ItemDefinition Convert(string text)
        {
            string arg = text.Substring((text.IndexOf("|") + 1), text.Length - (text.IndexOf("|") + 1)); ItemDefinition def = new ItemDefinition(); def.oN = arg.Substring(0, arg.IndexOf("|")); arg = arg.Substring((arg.IndexOf("|") + 1), arg.Length - (arg.IndexOf("|") + 1)); def.iTY = arg.Substring(0, arg.IndexOf("|"));
            arg = arg.Substring((arg.IndexOf("|") + 1), arg.Length - (arg.IndexOf("|") + 1)); def.iST = arg.Substring(0, arg.IndexOf("|")); arg = arg.Substring((arg.IndexOf("|") + 1), arg.Length - (arg.IndexOf("|") + 1)); def.bST = arg.Substring(0, arg.IndexOf("|")); arg = arg.Substring((arg.IndexOf("|") + 1), arg.Length - (arg.IndexOf("|") + 1)); string taMT = arg; if (taMT.Contains("|"))
                taMT = taMT.Replace("|", ""); if (taMT.ToLower().Contains("fuel")) { if (!fIdLT.Contains(def.ItemId())) fIdLT.Add(def.ItemId()); OutP("Fuel: " + def.oN); taMT = taMT.ToLower().Replace("fuel", ""); def.fuel = true; }
            def.vRt = double.Parse(taMT); def.mod = true; return def;
        }
        public void LoadData()
        {
            OutP("Loading data"); if (Me.CustomData != "" && Storage != Me.CustomData)
                Storage = Me.CustomData; if (Storage != "")
            {
                try
                {
                    if (Me.CustomData == "") Me.CustomData = Storage; string tmpB = Storage, tmpA = ""; tmpB = tmpB.Replace("\r\n", String.Empty); tmpB = tmpB.Replace("\n", String.Empty); tmpB = tmpB.Replace("\r", String.Empty); tmpB = tmpB.Replace("\t", String.Empty); string[] settingArray = tmpB.Split(';'); int stCT = 0; for (int i = 0; i < settingArray.Length; i++
        ) { tmpA = settingArray[i]; if (!string.IsNullOrEmpty(tmpA) && tmpA != "" && tmpA.Contains("|")) { if (tmpA.StartsWith("Custom|")) { stCT--; if (!tmpA.Contains(igtTyp) && !tmpA.Contains(orTyp)) stCT--; } stCT++; ProcessSetting(settingArray[i]); } }
                    if (stCT < 105 || rSv) SaveData(); else OutP("Settings: " + stCT.ToString()); rSv = false;
                }
                catch { OutP("Error loading data"); }
            }
            else SaveData();
            ResetRuntimes();
        }
        public void SaveData()
        {
            try
            {
                OutP("Saving data"); if (Me.CustomData == Storage || Me.CustomData == "")
                {
                    string lne = nLne; if (doubleLineSettings) lne += nLne; string sDA = "---COMPONENT QUOTAS---;" + nLne; for (int i = 0; i < cDL.Count; i++)
                    {
                        if (cDL.Values[i].mod) { if (sDA != "") sDA += lne; sDA += Convert(cDL.Values[i]); }
                        if (sDA != "") sDA += lne; sDA += cDL.Values[i].ItemId() + "|" +
cDL.Values[i].dAMT.ToString("N0") + ";";
                    }
                    sDA += nLne + nLne + "---TOOL, CANISTER, AND AMMO QUOTAS---;" + nLne; for (int i = 0; i < tDL.Count; i++) { if (tDL.Values[i].mod) { if (sDA != "") sDA += lne; sDA += Convert(tDL.Values[i]); } if (sDA != "") sDA += lne; sDA += tDL.Values[i].ItemId() + "|" + tDL.Values[i].dAMT.ToString("N0") + ";"; }
                    sDA += nLne + nLne + "---INGOT QUOTAS---;" + nLne; for (int i = 0;
i < iDL.Count; i++) { if (iDL.Values[i].mod) { if (sDA != "") sDA += lne; sDA += Convert(iDL.Values[i]); } if (sDA != "") sDA += lne; sDA += iDL.Values[i].ItemId() + "|" + iDL.Values[i].dAMT.ToString("N0") + ";"; }
                    AddSet(ref sDA, "stoneOreToIngotBasic|" + stoneOreToIngotBasic); sDA += nLne + nLne + "---ORE QUOTAS---;" + nLne; for (int i = 0; i < oDL.Count; i++)
                    {
                        if (oDL.Values[i].mod)
                        {
                            if (sDA != "")
                                sDA += lne; sDA += Convert(oDL.Values[i]);
                        }
                        if (sDA != "") sDA += lne; sDA += oDL.Values[i].ItemId() + "|" + oDL.Values[i].dAMT.ToString("N0") + ";";
                    }
                    sDA += nLne + nLne + "---TOGGLES---;" + nLne; AddSet(ref sDA, "arrangeBlueprints|" + arrangeBlueprints); AddSet(ref sDA, "arrangeRefineries|" + arrangeRefineries); AddSet(ref sDA, "countItemsAndBlueprints|" + countItemsAndBlueprints); AddSet(
ref sDA, "distributeItems|" + distributeItems); AddSet(ref sDA, "doLoadouts|" + doLoadouts); AddSet(ref sDA, "emptyAssemblers|" + emptyAssemblers); AddSet(ref sDA, "queueBlueprints|" + queueBlueprints); AddSet(ref sDA, "queueDisassembly|" + queueDisassembly); AddSet(ref sDA, "mergeBlueprints|" + mergeBlueprints); AddSet(ref sDA, "removeExcessBlueprintsAssembly|" +
removeExcessBlueprintsAssembly); AddSet(ref sDA, "removeExcessBlueprintsDisassembly|" + removeExcessBlueprintsDisassembly); AddSet(ref sDA, "sortItems|" + sortItems); AddSet(ref sDA, "spreadAmmo|" + spreadAmmo); AddSet(ref sDA, "spreadBlueprints|" + spreadBlueprints); AddSet(ref sDA, "spreadGasGenerators|" + spreadGasGenerators); AddSet(ref sDA, "spreadReactors|" +
spreadReactors); AddSet(ref sDA, "spreadRefineries|" + spreadRefineries); sDA += nLne + nLne + "---PANEL KEYWORDS---;" + nLne; AddSet(ref sDA, "cargoPanelName|" + cargoPanelName); AddSet(ref sDA, "componentPanelName|" + componentPanelName); AddSet(ref sDA, "ingotPanelName|" + ingotPanelName); AddSet(ref sDA, "modPanelName|" + modPanelName); AddSet(ref sDA, "outputPanelName|" +
outputPanelName); AddSet(ref sDA, "orePanelName|" + orePanelName); AddSet(ref sDA, "queuePanelName|" + queuePanelName); AddSet(ref sDA, "toolPanelName|" + toolPanelName); sDA += nLne + nLne + "---STORAGE KEYWORDS---;" + nLne; AddSet(ref sDA, "componentStorageKeyword|" + componentStorageKeyword); AddSet(ref sDA, "ingotStorageKeyword|" + ingotStorageKeyword); AddSet(ref sDA,
"oreStorageKeyword|" + oreStorageKeyword); AddSet(ref sDA, "toolStorageKeyword|" + toolStorageKeyword); sDA += nLne + nLne + "---CONTROL KEYWORDS---;" + nLne; AddSet(ref sDA, "autoConveyorKeyword|" + autoConveyorKeyword); AddSet(ref sDA, "crossGridKeyword|" + crossGridKeyword); AddSet(ref sDA, "exclusionKeyword|" + exclusionKeyword); AddSet(ref sDA, "manualAssemblyKeyword|" +
                manualAssemblyKeyword); AddSet(ref sDA, "noShowKeyword|" + noShowKeyword); sDA += nLne + nLne + "---SETTINGS---;" + nLne; AddSet(ref sDA, "actionLimiterMultiplier|" + actionLimiterMultiplier.ToString("N5")); AddSet(ref sDA, "ammoQuota|" + ammoQuota); AddSet(ref sDA, "assemblerProductionRange|" + assemblerProductionRange.ToString("N5")); AddSet(ref sDA, "conveyorControl|" +
                                      conveyorControl); AddSet(ref sDA, "countLoadoutItems|" + countLoadoutItems); AddSet(ref sDA, "displayQuotas|" + displayQuotas); AddSet(ref sDA, "distributeLoadoutItems|" + distributeLoadoutItems); AddSet(ref sDA, "doubleLineSettings|" + doubleLineSettings); AddSet(ref sDA, "emptyAssemblerDelay|" + emptyAssemblerDelay.ToString("N1")); AddSet(ref sDA, "fillCanCycles|" +
                                                      fillCanCycles.ToString()); AddSet(ref sDA, "fuelQuota|" + fuelQuota.ToString("N5")); AddSet(ref sDA, "graphLength|" + graphLength); AddSet(ref sDA, "graphLengthWide|" + graphLengthWide); AddSet(ref sDA, "iceQuota|" + iceQuota); AddSet(ref sDA, "outputDelay|" + outputDelay.ToString("N1")); AddSet(ref sDA, "outputLimit|" + outputLimit); AddSet(ref sDA, "overrideDelay|" +
                                                                         overrideDelay.ToString("N1")); AddSet(ref sDA, "quotasBelow|" + quotasBelow); AddSet(ref sDA, "refineStone|" + refineStone); AddSet(ref sDA, "runTimeLimiter|" + runTimeLimiter.ToString("N1")); AddSet(ref sDA, "sameGridOnly|" + sameGridOnly); AddSet(ref sDA, "scanDelay|" + scanDelay.ToString("N1")); AddSet(ref sDA, "sortAndDistributeDelay|" +
                                                                                         sortAndDistributeDelay.ToString("N1")); AddSet(ref sDA, "splitOutput|" + splitOutput); AddSet(ref sDA, "survivalKitAssembly|" + survivalKitAssembly); AddSet(ref sDA, "thoughtLine|" + thoughtLine); AddSet(ref sDA, "updateFrequency|" + updateFrequency); AddSet(ref sDA, "scriptName|" + scriptName); AddSet(ref sDA, "version|" + version.ToString("N2")); Me.CustomData = sDA;
                    Storage = sDA; OutP("Saved data");
                }
                else { Storage = Me.CustomData; OutP("User Settings Moved From Custom Data To Storage" + nLne + "Use Load Or Recompile To Load Settings Into Script"); }
            }
            catch { OutP("Error Saving Data"); }
        }
        public void ProcessSetting(string sTX)
        {
            try
            {
                int setIndex = sTX.IndexOf("|"), lH = sTX.Length; setIndex++; bool sBL = sTX.ToLower().Contains("true");
                double sDB = -123.321; string sST = sTX.Substring((sTX.IndexOf("|") + 1), sTX.Length - (sTX.IndexOf("|") + 1)); try { sDB = double.Parse(sTX.Substring(setIndex, lH - setIndex)); } catch { }
                try
                {
                    if (sTX.StartsWith("MyObjectBuilder") && sDB != -123.321) { string itemId = sTX.Substring(0, sTX.IndexOf("|")); SetMatchToDefinition(itemId, sDB); }
                    else if (sTX.StartsWith("Custom"))
                        AddItemDefinition(Convert(sTX), true);
                    else
                    {
                        if (sTX.StartsWith("outputPanelName|")) outputPanelName = sST;
                        else if (sTX.StartsWith("orePanelName|")) orePanelName = sST;
                        else if (sTX.StartsWith("ingotPanelName|")) ingotPanelName = sST;
                        else if (sTX.StartsWith("componentPanelName|")) componentPanelName = sST;
                        else if (sTX.StartsWith("toolPanelName|")) toolPanelName = sST;
                        else if (sTX.StartsWith("cargoPanelName|")) cargoPanelName = sST;
                        else if (sTX.StartsWith("queuePanelName|")) queuePanelName = sST;
                        else if (sTX.StartsWith("modPanelName|")) modPanelName = sST;
                        else if (sTX.StartsWith("exclusionKeyword|")) exclusionKeyword = sST;
                        else if (sTX.StartsWith("crossGridKeyword|")) crossGridKeyword = sST;
                        else if (sTX.StartsWith(
"autoConveyorKeyword|")) autoConveyorKeyword = sST;
                        else if (sTX.StartsWith("thoughtLine|")) thoughtLine = sST;
                        else if (sTX.StartsWith("oreStorageKeyword|")) oreStorageKeyword = sST;
                        else if (sTX.StartsWith("ingotStorageKeyword|")) ingotStorageKeyword = sST;
                        else if (sTX.StartsWith("componentStorageKeyword|")) componentStorageKeyword = sST;
                        else if (sTX.StartsWith(
"toolStorageKeyword|")) toolStorageKeyword = sST;
                        else if (sTX.StartsWith("manualAssemblyKeyword|")) manualAssemblyKeyword = sST;
                        else if (sTX.StartsWith("noShowKeyword|")) noShowKeyword = sST;
                        else if (sTX.StartsWith("outputLimit|")) outputLimit = (int)sDB;
                        else if (sTX.StartsWith("graphLength|")) graphLength = (int)sDB;
                        else if (sTX.StartsWith("graphLengthWide|"))
                            graphLengthWide = (int)sDB;
                        else if (sTX.StartsWith("updateFrequency|")) updateFrequency = (int)sDB;
                        else if (sTX.StartsWith("fillCanCycles|")) fillCanCycles = (int)sDB;
                        else if (sTX.StartsWith("ammoQuota|")) ammoQuota = (int)sDB;
                        else if (sTX.StartsWith("stoneOreToIngotBasic|")) stoneOreToIngotBasic = (int)sDB;
                        else if (sTX.StartsWith("outputDelay|")) outputDelay = sDB;
                        else if (
sTX.StartsWith("scanDelay|")) scanDelay = sDB;
                        else if (sTX.StartsWith("version|")) { if (sDB != version) rSv = true; }
                        else if (sTX.StartsWith("scriptName|")) { if (sST != scriptName) rSv = true; }
                        else if (sTX.StartsWith("emptyAssemblerDelay|")) emptyAssemblerDelay = sDB;
                        else if (sTX.StartsWith("fuelQuota|")) fuelQuota = sDB;
                        else if (sTX.StartsWith("iceQuota|")) iceQuota = sDB;
                        else if (
sTX.StartsWith("overrideDelay|")) overrideDelay = sDB;
                        else if (sTX.StartsWith("assemblerProductionRange|")) assemblerProductionRange = sDB;
                        else if (sTX.StartsWith("sortAndDistributeDelay|")) sortAndDistributeDelay = sDB;
                        else if (sTX.StartsWith("actionLimiterMultiplier|")) actionLimiterMultiplier = sDB;
                        else if (sTX.StartsWith("runTimeLimiter|")) runTimeLimiter = sDB;
                        else if (sTX.StartsWith("conveyorControl|")) conveyorControl = sBL;
                        else if (sTX.StartsWith("distributeLoadoutItems|")) distributeLoadoutItems = sBL;
                        else if (sTX.StartsWith("countLoadoutItems|")) countLoadoutItems = sBL;
                        else if (sTX.StartsWith("sameGridOnly|")) sameGridOnly = sBL;
                        else if (sTX.StartsWith("splitOutput|")) splitOutput = sBL;
                        else if (sTX.StartsWith(
"countItemsAndBlueprints|")) countItemsAndBlueprints = sBL;
                        else if (sTX.StartsWith("refineStone|")) refineStone = sBL;
                        else if (sTX.StartsWith("quotasBelow|")) quotasBelow = sBL;
                        else if (sTX.StartsWith("displayQuotas|")) displayQuotas = sBL;
                        else if (sTX.StartsWith("survivalKitAssembly|")) survivalKitAssembly = sBL;
                        else if (sTX.StartsWith("queueBlueprints|"))
                            queueBlueprints = sBL;
                        else if (sTX.StartsWith("doubleLineSettings|")) doubleLineSettings = sBL;
                        else if (sTX.StartsWith("sortItems|")) sortItems = sBL;
                        else if (sTX.StartsWith("distributeItems|")) distributeItems = sBL;
                        else if (sTX.StartsWith("emptyAssemblers|")) emptyAssemblers = sBL;
                        else if (sTX.StartsWith("arrangeBlueprints|")) arrangeBlueprints = sBL;
                        else if (
sTX.StartsWith("spreadBlueprints|")) spreadBlueprints = sBL;
                        else if (sTX.StartsWith("spreadRefineries|")) spreadRefineries = sBL;
                        else if (sTX.StartsWith("arrangeRefineries|")) arrangeRefineries = sBL;
                        else if (sTX.StartsWith("spreadGasGenerators|")) spreadGasGenerators = sBL;
                        else if (sTX.StartsWith("removeExcessBlueprintsAssembly|")) removeExcessBlueprintsAssembly = sBL;
                        else if (sTX.StartsWith("removeExcessBlueprintsDisassembly|")) removeExcessBlueprintsDisassembly = sBL;
                        else if (sTX.StartsWith("spreadReactors|")) spreadReactors = sBL;
                        else if (sTX.StartsWith("spreadAmmo|")) spreadAmmo = sBL;
                        else if (sTX.StartsWith("queueDisassembly|")) queueDisassembly = sBL;
                        else if (sTX.StartsWith("mergeBlueprints|")) mergeBlueprints = sBL;
                        else if (
sTX.StartsWith("doLoadouts|")) doLoadouts = sBL;
                    }
                    if (sTX.Contains("|")) OutP("Processed Setting: " + sTX);
                }
                catch { OutP("Error Processing Setting: " + sTX); }
            }
            catch { OutP("Error Processing Setting: " + sTX); }
        }
        public void AddSet(ref string sDA, string tmpA) { string lne = nLne; if (doubleLineSettings) lne += nLne; sDA += lne + tmpA + ";"; }
        public void ClearQueue()
        {
            Fill<IMyAssembler>(
ref bkLT, "", manualAssemblyKeyword.ToLower(), "", "survival"); for (int i = 0; i < bkLT.Count; i++) ((IMyAssembler)bkLT[i]).ClearQueue();
        }
        public string ItemType(string id) { try { if (id.Contains("/")) return id.Substring(0, id.IndexOf("/")); } catch { OutP("Error Generating Type From: " + id); } return id; }
        public string ItemSubtype(string id)
        {
            try
            {
                if (id.Contains("/"))
                    return id.Substring((id.IndexOf("/") + 1), id.Length - (id.IndexOf("/") + 1));
            }
            catch { OutP("Error Generating Subtype From: " + id); }
            return id;
        }
        public void QueueItems(string bST, int count, bool disassemble = false)
        {
            try
            {
                if (count > 0)
                {
                    MyDefinitionId id = MyDefinitionId.Parse(bptPfx + bST); List<IMyTerminalBlock> tmpA = new List<IMyTerminalBlock>(); MyAssemblerMode mode = asMd;
                    if (disassemble) mode = dsMd; gSys.GetBlocksOfType<IMyAssembler>(tmpA, (p => ((!p.BlockDefinition.ToString().ToLower().Contains("survival") || (!disassemble && survivalKitAssembly)) && !p.CustomData.ToLower().Contains(exclusionKeyword.ToLower()) && p.IsFunctional && ((IMyAssembler)p).CanUseBlueprint(id) && (((IMyAssembler)p).IsQueueEmpty || ((IMyAssembler)p).Mode == mode) && (
                          !sameGridOnly || (p.CustomData.ToLower().Contains(crossGridKeyword.ToLower()) || p.CubeGrid == Grid))) && !p.CustomData.ToLower().Contains(manualAssemblyKeyword.ToLower()))); if (tmpA.Count > count)
                    {
                        List<IMyTerminalBlock> tmpB = new List<IMyTerminalBlock>(); for (int i = 0; i < tmpA.Count; i++) tmpB.Add(tmpA[i]); if (smQuIx >= tmpB.Count) smQuIx = 0; tmpA.Clear(); for (int i = 0; i < count;
i++) { tmpA.Add(tmpB[smQuIx]); smQuIx++; if (smQuIx >= tmpB.Count) smQuIx = 0; }
                    }
                    double split = (double)count / (double)tmpA.Count; VRage.MyFixedPoint aMT = (VRage.MyFixedPoint)((int)split); if (tmpA.Count == 0) { if (disassemble) curDef.qdaMT -= count - (double)aMT; else curDef.qAMT -= count; } else { if (disassemble) curDef.qdaMT -= count - (double)aMT; else curDef.qAMT -= count; }
                    for (int i = 0;
i < tmpA.Count; i++) { ((IMyAssembler)tmpA[i]).Mode = mode; InsQueue((IMyAssembler)tmpA[i], id, aMT); }
                }
            }
            catch { OutP("Error Queueing Items: " + bST.ToString()); }
        }
        public void InsQueue(IMyAssembler bk, MyDefinitionId id, VRage.MyFixedPoint aMT)
        {
            if (bk.IsQueueEmpty) bk.AddQueueItem(id, aMT);
            else
            {
                List<MyProductionItem> pQ = new List<MyProductionItem>(); bk.GetQueue(pQ);
                int iX = -1; for (int i = 0; i < pQ.Count; i++) { if (pQ[i].BlueprintId.ToString() == id.ToString()) { iX = i; break; } }
                if (iX == -1) bk.AddQueueItem(id, aMT); else bk.InsertQueueItem(iX, id, aMT);
            }
        }
        public int CountInQueue(IMyTerminalBlock block, MyDefinitionId id)
        {
            int count = 0; if (!((IMyAssembler)block).IsQueueEmpty)
            {
                List<MyProductionItem> pQ = new List<MyProductionItem>(); ((
IMyAssembler)block).GetQueue(pQ); for (int i = 0; i < pQ.Count; i++) { if (pQ[i].BlueprintId.ToString() == id.ToString()) count += (int)pQ[i].Amount; }
            }
            return count;
        }
        public void OtToPnl(List<StringBuilder> txtLst, string panelKeyword, bool wide = false, string exclude = "", IMyTerminalBlock block = null)
        {
            try
            {
                StringBuilder bA = new StringBuilder(), bB = new StringBuilder()
, bC = new StringBuilder(); List<IMyTerminalBlock> textBlocks = new List<IMyTerminalBlock>(); int lTh = 0, lNs = txtLst.Count, lThA = 0, lThB = 0, lNsA = 0, lNsB = 0; try
                {
                    for (int i = 0; i < txtLst.Count; i++)
                    {
                        if (bA.Length != 0) bA.AppendLine(); if (txtLst[i].Length > lTh) lTh = txtLst[i].Length; bA.Append(txtLst[i].ToString()); if ((i + 1) <= txtLst.Count / 2)
                        {
                            lNsA++; if (txtLst[i].Length > lThA)
                                lThA = txtLst[i].Length; if (bB.Length != 0) bB.AppendLine(); bB.Append(txtLst[i].ToString());
                        }
                        else { lNsB++; if (txtLst[i].Length > lThB) lThB = txtLst[i].Length; if (bC.Length != 0) bC.AppendLine(); bC.Append(txtLst[i].ToString()); }
                    }
                }
                catch { OutP("Error reading output list"); }
                string txtA = bA.ToString(), txtB = bB.ToString(), txtC = bC.ToString(); if (block == null)
                {
                    Fill<IMyTextPanel>(ref textBlocks, panelKeyword, exclude); for (int i = 0; i < textBlocks.Count; i++)
                    {
                        IMyTerminalBlock tbk = textBlocks[i]; IMyTextPanel tmpB = ((IMyTextPanel)textBlocks[i]); string tmpD = textBlocks[i].BlockDefinition.ToString(); bool spn = textBlocks[i].CustomName.ToLower().Contains("span"); if (spn)
                        {
                            try
                            {
                                Matrix matrix = new Matrix();
                                textBlocks[i].Orientation.GetMatrix(out matrix); IMySlimBlock bk = Grid.GetCubeBlock(textBlocks[i].Position + (Vector3I)matrix.Down); if (bk == null) spn = false; else { tbk = (IMyTerminalBlock)bk.FatBlock; if (tbk == null || !(tbk is IMyTextPanel) || !tbk.IsFunctional) spn = false; }
                            }
                            catch { spn = false; }
                        }
                        bool uk = !(tmpD == WPanelDef || tmpD == SWPanelDef || tmpD == LPanelDef || tmpD ==
XLPanelDef || tmpD == TPanelDef || tmpD == SPanelDef || tmpD == STPanelDef || tmpD.ToLower().Contains(cornerPanelDef)); if ((uk && !wide) || (wide && (tmpD == WPanelDef || tmpD == SWPanelDef)) || (!wide && (tmpD == LPanelDef || tmpD == XLPanelDef || tmpD == TPanelDef || tmpD == SPanelDef || tmpD == STPanelDef || tmpD.ToLower().Contains(cornerPanelDef))))
                        {
                            tmpB.ShowPublicTextOnScreen(); if (!spn)
                                tmpB.WritePublicText(txtA, false);
                            else { tmpB.WritePublicText(txtB, false); ((IMyTextPanel)tbk).WritePublicText(txtC); ((IMyTextPanel)tbk).ShowPublicTextOnScreen(); }
                            if (lTh > 0 && lNs > 0)
                            {
                                if (!spn) SizePanel(textBlocks[i], lTh, lNs); else { SizePanel(textBlocks[i], lThA, lNsA); SizePanel(tbk, lThB, lNsB); ((IMyTextPanel)tbk).Font = "Monospace"; }
                                tmpB.Font = "Monospace";
                            }
                            else if (!uk) { tmpB.FontSize = 1.0F; tmpB.Font = "DEBUG"; }
                        }
                    }
                }
                else
                {
                    IMyTextPanel tmpB = ((IMyTextPanel)block); string taMT = block.BlockDefinition.ToString(); bool uk = !(LargePanel(taMT) || WidePanel(taMT)); if (!wide && (LargePanel(taMT)))
                    {
                        if (lTh > 0 && lNs > 0) { SizePanel(block, lTh, lNs); tmpB.Font = "Monospace"; }
                        else if (!uk)
                        {
                            tmpB.FontSize = 1.0F; tmpB.Font = "DEBUG";
                        }
                        tmpB.ShowPublicTextOnScreen(); tmpB.WritePublicText(txtA, false);
                    }
                    else if (wide && WidePanel(taMT)) { if (lTh > 0 && lNs > 0) { SizePanel(block, lTh, lNs); tmpB.Font = "Monospace"; } else if (!uk) { tmpB.FontSize = 1.0F; tmpB.Font = "DEBUG"; } tmpB.ShowPublicTextOnScreen(); tmpB.WritePublicText(txtA, false); }
                    else if (uk)
                    {
                        tmpB.ShowPublicTextOnScreen(); tmpB.WritePublicText(txtA,
false);
                    }
                }
            }
            catch { OutP("Error caught painting panel-line #: " + txtLst.Count.ToString()); }
        }
        public bool LargePanel(string tmpA) { return (tmpA == LPanelDef || tmpA == XLPanelDef || tmpA == TPanelDef || tmpA == SPanelDef || tmpA == STPanelDef || tmpA.ToLower().Contains(cornerPanelDef)); }
        public bool WidePanel(string tmpA)
        {
            return (tmpA == WPanelDef || tmpA == SWPanelDef);
        }
        public void OutQus()
        {
            List<StringBuilder> txtLst = new List<StringBuilder>(); StringBuilder emptyText = new StringBuilder(); emptyText.Append("Nothing Queued"); string taMT = ""; for (int i = 0; i < cDL.Count; i++)
            {
                taMT = cDL.Values[i].oN; if (cDL.Values[i].qAMT > 0) taMT += ", " + "Assemble: " + cDL.Values[i].qAMT.ToString("N0"); if (cDL.Values[i].qdaMT > 0) taMT += ", " + "Disassemble: " +
cDL.Values[i].qdaMT.ToString("N0"); if (taMT != cDL.Values[i].oN) { txtLst.Add(new StringBuilder()); txtLst[txtLst.Count - 1].Append(taMT); }
            }
            for (int i = 0; i < tDL.Count; i++)
            {
                taMT = tDL.Values[i].oN; if (tDL.Values[i].qAMT > 0) taMT += ", " + "Assemble: " + tDL.Values[i].qAMT.ToString("N0"); if (tDL.Values[i].qdaMT > 0) taMT += ", " + "Disassemble: " + tDL.Values[i].qdaMT.ToString("N0");
                if (taMT != tDL.Values[i].oN) { txtLst.Add(new StringBuilder()); txtLst[txtLst.Count - 1].Append(taMT); }
            }
            if (txtLst.Count > 0) { OtToPnl(txtLst, queuePanelName); OtToPnl(txtLst, queuePanelName, true); } else { OtToPnl(new List<StringBuilder> { emptyText }, queuePanelName); OtToPnl(new List<StringBuilder> { emptyText }, queuePanelName, true); }
        }
        public void OutCargo()
        {
            List<StringBuilder> txtLst = new List<StringBuilder>(); List<StringBuilder> wTxtLt = new List<StringBuilder>(); for (int i = 1; i <= 5; i++) { txtLst.Add(new StringBuilder()); wTxtLt.Add(new StringBuilder()); }
            double totCap = 0, totVol = 0; for (int i = 0; i < cIDS.Count; i++)
            {
                try
                {
                    tBlk = aBL[cIDS[i]]; tInv = tBlk.GetInventory(0); totVol += CrVol(tInv); totCap += MxVol(tInv); if (!CData(tBlk)
.Contains(noShowKeyword.ToLower()))
                    {
                        txtLst.Add(new StringBuilder()); wTxtLt.Add(new StringBuilder()); txtLst[txtLst.Count - 1].Append(tBlk.CustomName.PadRight(21)); wTxtLt[wTxtLt.Count - 1].Append(tBlk.CustomName.PadRight(21)); txtLst[txtLst.Count - 1].Append(PBar(((CrVol(tInv) / MxVol(tInv)) * 100.0), graphLength)); wTxtLt[wTxtLt.Count - 1].Append(PBar(((CrVol(tInv)
  / MxVol(tInv)) * 100.0), graphLengthWide));
                    }
                }
                catch { OutP("Error With Cargo Output Possible Missing Block"); }
            }
            txtLst[0].Append("Total Cargo Capacity "); txtLst[0].Append(PBar(((totVol / totCap) * 100.0), graphLength)); wTxtLt[0].Append("Total Cargo Capacity "); wTxtLt[0].Append(PBar(((totVol / totCap) * 100.0), graphLengthWide)); StPc(oSB, ref txtLst, ref wTxtLt, 1,
"Ore", graphLength, graphLengthWide); StPc(iSB, ref txtLst, ref wTxtLt, 2, "Ingot", graphLength, graphLengthWide); StPc(cSB, ref txtLst, ref wTxtLt, 3, "Component", graphLength, graphLengthWide); StPc(tSB, ref txtLst, ref wTxtLt, 4, "Tool", graphLength, graphLengthWide); OtToPnl(txtLst, cargoPanelName, false); OtToPnl(wTxtLt, cargoPanelName, true);
        }
        public void StPc(List<int> list, ref List<StringBuilder> tmpC, ref List<StringBuilder> tmpF, int tmpH, string tmpD, int tmpE, int tmpG)
        {
            double tmpA = 0, tmpB = 0; for (int i = 0; i < list.Count; i++) { tmpA += CrVol(aBL[list[i]].GetInventory(0)); tmpB += MxVol(aBL[list[i]].GetInventory(0)); }
            tmpC[tmpH].Append((tmpD + " Capacity").PadRight(21) + PBar((tmpA / tmpB) * 100.0, tmpE));
            tmpF[tmpH].Append((tmpD + " Capacity").PadRight(21) + PBar((tmpA / tmpB) * 100.0, tmpG));
        }
        public double CrVol(IMyInventory inv) { return (double)inv.CurrentVolume; }
        public double MxVol(IMyInventory inv) { return (double)inv.MaxVolume; }
        public void GetTags(ref List<string> tags, string source, string prefix = "[", string suffix = "]")
        {
            string taMT = source.ToLower().Replace(
" ", ""); if (!taMT.Contains(prefix)) return; int tmpB = taMT.IndexOf(prefix) + prefix.Length; taMT = taMT.Substring(tmpB, taMT.Length - tmpB); if (taMT.Contains(suffix)) taMT = taMT.Substring(0, taMT.IndexOf(suffix)); tags.Clear(); while (taMT != "")
            {
                string tag = ""; if (!taMT.Contains("|")) { tag = taMT; if (tag != "") tags.Add(tag); break; }
                else
                {
                    tmpB = taMT.IndexOf("|");
                    tag = taMT.Substring(0, tmpB); if (tag != "") tags.Add(tag); tmpB++; taMT = taMT.Substring(tmpB, taMT.Length - tmpB);
                }
            }
        }
        public double GetLength(IMyTerminalBlock block) { try { return (GetLength(block.CustomName, "-")); } catch { OutP("Error parsing length"); } return 0.0; }
        public double GetLength(string name, string prefix)
        {
            int taMT = name.IndexOf(prefix) + prefix.Length;
            name = name.Substring(taMT, (name.Length - taMT)); if (name.Contains(" ")) taMT = name.IndexOf(" "); else if (name.Contains("[")) taMT = name.IndexOf("["); else taMT = name.Length; return double.Parse(name.Substring(0, taMT));
        }
        public void OtCnts(SortedList<string, ItemDefinition> defList, string pnlNm, bool mod = false, bool tagged = false, bool wNum = false)
        {
            try
            {
                List<StringBuilder> txtLst = new List<StringBuilder>(), wTxtLt = new List<StringBuilder>(); double count = 0, desAmt = 0; int tmpC = graphLength, tmpD = graphLengthWide; if (tagged && tdBL[0].CustomName.Contains("-")) { try { tmpC = (int)GetLength(tdBL[0]); tmpD = tmpC; } catch { } }
                string infoText = "", desText = "", desTextWide = ""; List<string> tags = new List<string>(); for (int i = 0;
i < defList.Count; i++)
                {
                    if (defList.Values[i].iTY == orTyp && defList.Values[i].iST == "Scrap") { }
                    else
                    {
                        bool taMT = false; if (tagged) { GetTags(ref tags, tdBL[0].CustomName); for (int x = 0; x < tags.Count; x++) { if (defList.Values[i].oN.ToLower().Replace(" ", "").StartsWith(tags[x].ToLower())) { taMT = true; break; } } if (tags.Count == 0) taMT = true; }
                        if (!tagged || taMT)
                        {
                            count = defList.Values[i].aMT; desAmt = defList.Values[i].dAMT; StringBuilder text = new StringBuilder(); StringBuilder wideText = new StringBuilder(); if (!mod)
                            {
                                infoText = defList.Values[i].oN.PadRight(nameWidth) + ShortNum(count, wNum).PadLeft(5); if (!displayQuotas) { desText = ""; desTextWide = ""; }
                                else
                                {
                                    if (defList.Values[i].dAMT > 0)
                                    {
                                        desText = "/" + ShortNum(desAmt, wNum)
.PadRight(5) + " "; desTextWide = "" + desText; desText += PBar(((defList.Values[i].aMT / defList.Values[i].dAMT) * 100.0), tmpC); desTextWide += PBar(((defList.Values[i].aMT / defList.Values[i].dAMT) * 100.0), tmpD);
                                    }
                                    else { desText = "No Quota".PadLeft((12 + tmpC)); desTextWide = "No Quota".PadLeft((12 + tmpD)); }
                                }
                            }
                            else
                            {
                                infoText = (i.ToString() + ":" + defList.Values[i].oN).PadRight(
nameWidth); if (defList.Values[i].bST == "") infoText += " Item"; else infoText += " Blueprint"; count.ToString().PadLeft(5);
                            }
                            if (displayQuotas && quotasBelow)
                            {
                                text.Append(infoText); wideText.Append(infoText); txtLst.Add(text); wTxtLt.Add(wideText); text = new StringBuilder(); wideText = new StringBuilder(); desText = ("∟" + desText.Replace("/", "").Trim()).PadLeft((14 + tmpC));
                                desTextWide = ("∟" + desTextWide.Replace("/", "").Trim()).PadLeft((14 + tmpD)); text.Append(desText); wideText.Append(desTextWide); txtLst.Add(text); wTxtLt.Add(wideText);
                            }
                            else { text.Append(infoText + desText); wideText.Append(infoText + desTextWide); txtLst.Add(text); wTxtLt.Add(wideText); }
                        }
                    }
                }
                if (txtLst.Count > 0 && wTxtLt.Count > 0)
                {
                    if (!tagged)
                    {
                        OtToPnl(txtLst, pnlNm, false,
"["); OtToPnl(wTxtLt, pnlNm, true, "[");
                    }
                    else { OtToPnl(txtLst, pnlNm, false, "", tdBL[0]); OtToPnl(wTxtLt, pnlNm, true, "", tdBL[0]); tdBL.RemoveAt(0); }
                    if (!mod && !tagged)
                    {
                        string tmpB = ingotPanelName, iTY = defList.Values[0].iTY; if (iTY == orTyp) tmpB = orePanelName; else if (iTY == cptTyp) tmpB = componentPanelName; else if (ToolType(iTY)) tmpB = toolPanelName;
                        FillListG<IMyTextPanel>(ref tdBL, tmpB, "["); if (tdBL.Count > 0) OtCnts(defList, pnlNm, false, true, wNum);
                    }
                    else if (tagged && tdBL.Count > 0) OtCnts(defList, pnlNm, false, true, wNum);
                }
                else if (mod) { StringBuilder b = new StringBuilder(); b.Append("No Mod Items"); OtToPnl(new List<StringBuilder> { b }, pnlNm, false); OtToPnl(new List<StringBuilder> { b }, pnlNm, true); }
            }
            catch { OutP("Error caught creating output counts"); }
        }
        public string ShortNum(double tmpA, bool wNum = false)
        {
            string tmpB = "", suff = "", modF = "N1"; double tmpC = tmpA; if (tmpA > 2147483647.0) { tmpC = 2.1; modF = "N1"; suff = "B"; }
            else if (tmpA > 999999999.0) { tmpC = tmpA / 1000000000.0; modF = "N2"; suff = "B"; }
            else if (tmpA > 99999999.0) { tmpC = tmpA / 1000000.0; modF = "N0"; suff = "M"; }
            else if (
tmpA > 9999999.0) { tmpC = tmpA / 1000000.0; modF = "N1"; suff = "M"; }
            else if (tmpA > 999999.0) { tmpC = tmpA / 1000000.0; modF = "N2"; suff = "M"; } else if (tmpA > 99999.0) { tmpC = tmpA / 1000.0; modF = "N0"; suff = "K"; } else if (tmpA >= 1000.0) modF = "N0"; else if (tmpA >= 100.0) modF = "N1"; else if (tmpA >= 10.0) modF = "N2"; else modF = "N3"; if (wNum) modF = "N0"; tmpB = tmpC.ToString(modF) + suff; return tmpB.Replace(",",
                        "");
        }
        public string PBar(double perc, int totalLength)
        {
            if (totalLength >= 3)
            {
                double percentage = perc; if (percentage > 100.0) percentage = 100.0; string text = "", percString = perc.ToString("N0") + "%"; if (perc >= 999.5) percString = "999%"; else if (perc < 10.0) percString = perc.ToString("N1") + "%"; int length = totalLength - 2; if (percentage >= 0)
                {
                    double fullBars = ((double)length)
* percentage / 100.0; int fBars = (int)fullBars; int emptyBars = length - fBars; int halfBars = 0; if (fullBars >= (((double)fBars) + 0.5) && emptyBars > 0) { emptyBars -= 1; halfBars = 1; }
                    for (int i = 0; i < (int)fullBars; i++) text += "█"; for (int i = 0; i < halfBars; i++) text += "▌"; for (int i = 0; i < emptyBars; i++) text += " ";
                }
                else for (int i = 0; i < length; i++) text += "'"; return "[" + text + "]" +
percString.PadLeft(5);
            }
            return "";
        }
        public void RemBp(string bST, bool disassemble, double amount)
        {
            double tmpA = amount; try
            {
                string itemId = bptPfx + bST; Fill<IMyAssembler>(ref bkLT, "", manualAssemblyKeyword.ToLower()); for (int i = 0; i < bkLT.Count; i++)
                {
                    if (!((IMyAssembler)bkLT[i]).IsQueueEmpty && ((!disassemble && ((IMyAssembler)bkLT[i]).Mode == asMd) || (disassemble && ((
IMyAssembler)bkLT[i]).Mode == dsMd)))
                    {
                        List<MyProductionItem> pQ = new List<MyProductionItem>(); ((IMyAssembler)bkLT[i]).GetQueue(pQ); if (pQ.Count > ((tDL.Count + cDL.Count) * 2.5)) { ((IMyAssembler)bkLT[i]).ClearQueue(); }
                        else
                        {
                            for (int x = 0; x < pQ.Count; x++) if (pQ[x].BlueprintId.ToString() == itemId)
                                {
                                    double tmpB = tmpA; if ((double)pQ[x].Amount < tmpB) tmpB = (double)pQ[x].Amount;
                                    RemQueueItem((IMyAssembler)bkLT[i], pQ[x], x, (VRage.MyFixedPoint)tmpB); tmpA -= tmpB; break;
                                }
                        }
                    }
                    if (tmpA <= 0) break;
                }
            }
            catch { OutP("Error Caught Removing Blueprint From Queues"); }
            bkLT.Clear();
        }
        public List<IMyTerminalBlock> BlocksWithInventory()
        {
            List<IMyTerminalBlock> bKS = new List<IMyTerminalBlock>(); List<IMyTerminalBlock> outBlocks = new List<IMyTerminalBlock>();
            List<Vector3I> heldPos = new List<Vector3I>(); try
            {
                gSys.GetBlocks(bKS); while (bKS.Count > 0)
                {
                    string tmpA = CData(bKS[0]); if (!(bKS[0] is IMyLargeInteriorTurret) && !(bKS[0] is IMyCargoContainer) && !tmpA.Contains(exclusionKeyword.ToLower()) && bKS[0].IsFunctional && bKS[0].InventoryCount > 0 && !heldPos.Contains(bKS[0].Position) && (!sameGridOnly || (tmpA.Contains(
crossGridKeyword.ToLower()) || bKS[0].CubeGrid == Grid))) { outBlocks.Add(bKS[0]); heldPos.Add(bKS[0].Position); }
                    bKS.RemoveAt(0);
                }
            }
            catch { OutP("Error Getting Blocks With Inventory"); }
            return outBlocks;
        }
        public void StatusSwitch(ref string taMT)
        {
            if (taMT != "") taMT += nLne; string tmpA = nLne + "Stage: " + sM.ToString() + ": "; switch (sM)
            {
                case 0:
                    if (ctMde == 0)
                    {
                        taMT += "Counting Items"; taMT += tmpA + cCI.ToString() + "/" + aBL.Count.ToString(); if (ctOtIv.Count > 0) taMT += ", " + ctOtIv.Count.ToString(); if (cIvImLt.Count > 0) taMT += ", " + cIvImLt.Count.ToString();
                    }
                    else { taMT += "Counting Blueprints"; taMT += tmpA + bkLT.Count.ToString() + "/" + assCount.ToString() + " Assemblers"; }
                    break;
                case 1:
                    if (quMD == 0)
                    {
                        taMT += "Queueing Items"; taMT += tmpA +
nBIX.ToString() + "/" + (tDL.Count + cDL.Count).ToString() + " Blueprints";
                    }
                    else { taMT += "Checking Assembly Queues"; taMT += tmpA + quIX.ToString() + "/" + (tDL.Count + cDL.Count).ToString() + " Blueprints"; }
                    break;
                case 2: taMT += "Sorting Items"; taMT += tmpA + scrIx.ToString() + "/" + aBL.Count + " Blocks"; break;
                case 3:
                    taMT += "Distributing Items"; taMT += tmpA + scrIx.ToString() + "/" +
cIDS.Count + " Containers"; taMT += ", " + invItemList.Count.ToString(); break;
                case 4: taMT += "Emptying Unused Assemblers"; taMT += tmpA + bkLT.Count.ToString() + "/" + assCount + " Assemblers"; break;
                case 5: taMT += "Arranging Queues"; taMT += tmpA + bkLT.Count + "/" + assCount + " Assemblers"; break;
                case 6:
                    taMT += "Working Idle Assemblers"; taMT += tmpA + wkIdx.ToString() + "/" + bkLT.Count +
" Assemblers"; break;
                case 7: taMT += "Working Idle Refineries"; taMT += tmpA + wkIdx.ToString() + "/" + bkLT.Count + " Refineries"; break;
                case 8: taMT += "Ordering Refineries"; taMT += tmpA + bkLT.Count.ToString() + " Refineries"; break;
                case 9: taMT += "Working Idle Gas Generators"; taMT += tmpA + wkIdx.ToString() + "/" + bkLT.Count + " Gas Generators"; break;
                case 10: taMT += "Working Idle Reactors"; taMT += tmpA + wkIdx.ToString() + "/" + bkLT.Count + " Reactors"; break;
                case 11: taMT += "Filling Empty Weapons"; taMT += tmpA + wkIdx.ToString() + "/" + bkLT.Count + " Weapons"; break;
                case 12:
                    if (quMD == 0) { taMT += "Queueing Disassembly"; taMT += tmpA + nBIX.ToString() + "/" + (tDL.Count + cDL.Count).ToString() + " Blueprints"; }
                    else
                    {
                        taMT += "Checking Disassembly Queues"; taMT += tmpA + quIX.ToString() + "/" + (tDL.Count + cDL.Count).ToString() + " Blueprints";
                    }
                    break;
                case 13: taMT += "Merging Queue Stacks"; taMT += tmpA + quMD.ToString() + "/" + bkLT.Count.ToString() + " Assemblers"; break;
                case 14: taMT += "Stocking Components"; taMT += tmpA + bkLT.Count.ToString() + " Containers"; break;
                default:
                    taMT += "Script Mode: " +
sM.ToString(); break;
            }
        }
        public string Status()
        {
            string taMT = "NDS Inventory Manager", tmpB = thoughtLine, tmpC = ""; try { StatusSwitch(ref taMT); } catch { OutP("Error generating status"); }
            if (taMT != "") taMT += nLne; taMT += "Actions: " + Runtime.CurrentInstructionCount.ToString() + "/" + Runtime.MaxInstructionCount.ToString() + nLne + "Max Actions: " + mxActs.ToString() + ", " +
"In Stage: " + hGsT.ToString(); taMT += nLne + "Runtime: " + Runtime.LastRunTimeMs.ToString("N1") + "/" + runTimeLimiter.ToString("N0"); taMT += ", " + "Range: " + lowRunTime.ToString("N1") + "-" + highRunTime.ToString("N1") + ", " + hGsT; if (aBL.Count > 0)
            {
                if (taMT != "") taMT += nLne; taMT += "Managed Blocks: " + aBL.Count.ToString(); taMT += nLne + "Processors:"; int pC = 0; if (icPB.Count > 0)
                {
                    pC++;
                    taMT += " Ice: " + icPB.Count.ToString();
                }
                if (orPB.Count > 0) { if (pC > 0) taMT += ", "; else taMT += " "; pC++; taMT += "Ore: " + orPB.Count.ToString(); }
                if (stBK.Count > 0) { if (pC > 0) taMT += ", "; else taMT += " "; pC++; taMT += "Sifters: " + stBK.Count.ToString(); }
                if (amPBK.Count > 0)
                {
                    if (pC == 3) taMT += nLne; else if (pC > 0 && pC != 3) taMT += ", "; else taMT += " "; pC++; taMT += "Guns: " + amPBK.Count.ToString(
);
                }
                if (rcBK.Count > 0) { if (pC == 3) taMT += nLne; else if (pC > 0 && pC != 3) taMT += ", "; else taMT += " "; pC++; taMT += "Reactors: " + rcBK.Count.ToString(); }
                if (pC == 0) taMT += " None"; pC = 0; taMT += nLne + "Storages:"; if (tSB.Count > 0) { pC++; taMT += " Tool: " + tSB.Count.ToString(); }
                if (oSB.Count > 0) { if (pC > 0) taMT += ", "; else taMT += " "; pC++; taMT += "Ore: " + oSB.Count.ToString(); }
                if (iSB.Count > 0)
                {
                    if (
pC == 2) taMT += nLne;
                    else if (pC > 0 && pC != 2) taMT += ", "; else taMT += " "; pC++; taMT += "Ingot: " + iSB.Count.ToString();
                }
                if (cSB.Count > 0) { if (pC == 2) taMT += nLne; else if (pC > 0 && pC != 2) taMT += ", "; else taMT += " "; pC++; taMT += "Component: " + cSB.Count.ToString(); }
                if (pC == 0) taMT += " None";
            }
            if (tST == 0) tmpC = @"\"; else if (tST == 1) tmpC = "|"; else if (tST == 2) tmpC = "/"; tST++; if (tST > 2)
            {
                tST = 0;
                tID++; if ((tID + 1) > thoughtLine.Length) tID = 0;
            }
            if (tID == 0) tmpB = tmpC + tmpB; else if (tID == tmpB.Length) tmpB += tmpC; else tmpB = tmpB.Substring(0, tID) + tmpC + tmpB.Substring(tID + 1, (tmpB.Length - tID - 1)); if (taMT != "") taMT += nLne; taMT += tmpB; return taMT;
        }
        public void SizePanel(IMyTerminalBlock block, int textWidth, int textHeight)
        {
            string def = block.BlockDefinition.ToString();
            float taMT = (float)(textWidth * charWidth); float tmpB = (float)(textHeight * charHeight); if (def == WPanelDef || def == SWPanelDef) { float tmpC = WPanelWidth / taMT; float tmpD = WPanelHeight / tmpB; if (tmpC < tmpD) ((IMyTextPanel)block).FontSize = tmpC; else ((IMyTextPanel)block).FontSize = tmpD; }
            else if (def == XLPanelDef || def == LPanelDef || def == TPanelDef || def == SPanelDef || def ==
STPanelDef) { float tmpC = LPanelWidth / taMT; float tmpD = LPanelHeight / tmpB; if (tmpC < tmpD) ((IMyTextPanel)block).FontSize = tmpC; else ((IMyTextPanel)block).FontSize = tmpD; }
            else if (def.ToLower().Contains(cornerPanelDef))
            {
                float tmpC = LPanelWidth / taMT; float tmpD = cHG / tmpB; if (def.ToLower().Contains("flat")) tmpD = cFH / tmpB; if (Grid.GridSizeEnum == MyCubeSize.Small)
                {
                    tmpD = SCornerHeight / tmpB; if (def.ToLower().Contains("flat")) tmpD = SFCornerHeight / tmpB;
                }
                if (tmpC < tmpD) ((IMyTextPanel)block).FontSize = tmpC; else ((IMyTextPanel)block).FontSize = tmpD;
            }
        }
        public void PaintOutput()
        {
            nextOut = DateTime.Now.AddSeconds(outputDelay); string otStr = Status(); List<IMyTerminalBlock> textBlocks = new List<IMyTerminalBlock>(); for (
int i = 0; i < otLT.Count; i++) { if (otStr != "") otStr += nLne; otStr += otLT[i]; }
            Fill<IMyTextPanel>(ref textBlocks, outputPanelName); for (int i = 0; i < textBlocks.Count; i++) { ((IMyTextPanel)textBlocks[i]).ShowPublicTextOnScreen(); ((IMyTextPanel)textBlocks[i]).WritePublicText(otStr, false); }
        }
        public void OutP(string text)
        {
            if (text != "" && (otLT.Count == 0 || (otLT[0] != text)) && (
otLT.Count < 2 || otLT[1] != text) && (otLT.Count < 3 || otLT[2] != text) && (otLT.Count < 4 || otLT[3] != text) && (otLT.Count < 5 || otLT[4] != text) && (otLT.Count < 6 || otLT[5] != text) && (otLT.Count < 7 || otLT[6] != text) && (otLT.Count < 8 || otLT[7] != text))
            {
                if (DateTime.Now >= hpDLy) Echo(text); otLT.Insert(0, text); for (int i = 0; i < 75; i++)
                {
                    if (otLT.Count > outputLimit) otLT.RemoveAt(outputLimit);
                    else break;
                }
            }
        }
        public class BlueprintDefinition { public MyDefinitionId blueprintId; public VRage.MyFixedPoint aMT = (VRage.MyFixedPoint)0.0; }
        public class ItemDefinition
        {
            public string iTY = "", iST = "", bST = "", oN = "Item"; public double aMT = 0, dAMT = 0, qAMT = 0, taMT = 0, tqaMT = 0, vRt = 0.5, qdaMT = 0, tqdaMT = 0, tmpA = 0; public bool mod = false, fuel = false;
            public double Priority() { if (dAMT > 0) return (100.0 - (aMT / dAMT * 100.0)); return 0; }
            public string ItemId() { return iTY + "/" + iST; }
            public double QueueMore(double prodRange = 0.0) { tmpA = 0.0; if (dAMT > 0.0 && ((aMT + qAMT - qdaMT) < dAMT)) tmpA = dAMT - qAMT - aMT + qdaMT; qAMT += tmpA; return tmpA; }
            public double DisassembleMore(double prodRange = 0.0)
            {
                tmpA = 0; if (dAMT > 0.0 && ((aMT + qAMT - qdaMT) > (
dAMT * (1.0 + prodRange)))) tmpA = aMT + qAMT - qdaMT - dAMT; if (dAMT == -1.0 && (aMT + qAMT - qdaMT) > 0.0) tmpA = aMT + qAMT - qdaMT; qdaMT += tmpA; return tmpA;
            }
            public double ExQued(bool dSbl, double prodRange = 0.0)
            {
                double tmpB = dAMT; tmpA = 0; if (tmpB == -1) { if (dSbl) { if (qdaMT > aMT) tmpA = qdaMT - aMT; } else if (qAMT > 0) tmpA = qAMT; }
                else if (tmpB == 0 || ((dSbl && qdaMT == 0) || (!dSbl && qAMT == 0))) tmpA = 0.0;
                else if (dSbl && ((aMT + qAMT - qdaMT) < tmpB || qdaMT > aMT)) tmpA = (tmpB - (aMT - qdaMT)); else if (!dSbl && (aMT + qAMT - qdaMT) > (tmpB * (1.0 + prodRange))) tmpA = aMT + qAMT - qdaMT - tmpB; if (dSbl) qdaMT -= tmpA; else qAMT -= tmpA; return tmpA;
            }
        }
    }
}