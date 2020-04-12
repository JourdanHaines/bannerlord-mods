using System;
using System.Windows.Forms;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace SoundTheAlarm {
    public class STAAction {

        private static STAAction _instance;

        private List<string> managedSettlements;
        private Settlement settlementToTrack;

        public void Initialize() {
            managedSettlements = new List<string>();
        }

        // Action method fired once the VilageBeingRaided event fires
        public void DisplayVillageRaid(Village v) {
            if (Hero.MainHero != null) {
                if (Hero.MainHero.IsAlive) {
                    if (ShouldAlertForSettlement(v.Settlement)) {
                        if (!managedSettlements.Contains(v.Settlement.Name.ToString())) {
                            managedSettlements.Add(v.Settlement.Name.ToString());
                            string display =
                                v.Settlement.Name.ToString() +
                                " is under attack by " +
                                v.Settlement.LastAttackerParty.Name.ToString() +
                                " of the " +
                                v.Settlement.LastAttackerParty.LeaderHero.MapFaction.Name.ToString() +
                                "!"
                            ;

                            settlementToTrack = v.Settlement;
                            InformationManager.ShowInquiry(new InquiryData("Sound The Alarm", display, true, true, "Track", "Close", new Action(Track), null, ""), STALibrary.Instance.STAConfiguration.PauseOnFiefPopup);
                            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                                InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + display, new Color(1.0f, 0.0f, 0.0f)));
                        }
                    }
                }
            }
        }

        // Action method fired once the VillageBecomeNormal event fires
        public void FinalizeVillageRaid(Village v) {
            if (managedSettlements.Contains(v.Settlement.Name.ToString())) {
                managedSettlements.Remove(v.Settlement.Name.ToString());
                if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                    InformationManager.DisplayMessage(new InformationMessage("STALibrary: Removed " + v.Settlement.Name.ToString() + " from managed settlements list", new Color(1.0f, 0.0f, 0.0f)));
            }
        }

        // Action method fired once the OnSiegeEventStartedEvent event fires
        public void DisplaySiege(SiegeEvent e) {
            if (Hero.MainHero != null) {
                if (Hero.MainHero.IsAlive) {
                    if (ShouldAlertForSettlement(e.BesiegedSettlement)) {
                        if (!managedSettlements.Contains(e.BesiegedSettlement.Name.ToString())) {
                            managedSettlements.Add(e.BesiegedSettlement.Name.ToString());
                            string display =
                                e.BesiegedSettlement.Name.ToString() +
                                " is under attack by " +
                                e.BesiegedSettlement.LastAttackerParty.Name.ToString() +
                                " of the " +
                                e.BesiegedSettlement.LastAttackerParty.LeaderHero.MapFaction.Name.ToString() +
                                "!"
                            ;
                            settlementToTrack = e.BesiegedSettlement;
                            InformationManager.ShowInquiry(new InquiryData("Sound The Alarm", display, true, true, "Track", "Close", new Action(Track), null, ""), STALibrary.Instance.STAConfiguration.PauseOnFiefPopup);
                            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                                InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + display, new Color(1.0f, 0.0f, 0.0f)));
                        }
                    }
                }
            }
        }

        // Action method fired once the OnSiegeEventEndedEvent event fires
        public void FinalizeSiege(SiegeEvent e) {
            if (managedSettlements.Contains(e.BesiegedSettlement.Name.ToString())) {
                managedSettlements.Remove(e.BesiegedSettlement.Name.ToString());
                if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                    InformationManager.DisplayMessage(new InformationMessage("STALibrary: Removed " + e.BesiegedSettlement.Name.ToString() + " from managed settlements list", new Color(1.0f, 0.0f, 0.0f)));
            }
        }

        // Action method fired once two empires declare war
        public void OnDeclareWar(IFaction faction1, IFaction faction2) {
            string display =
                faction1.Leader.Name.ToString() +
                " of the " +
                faction1.Name.ToString() +
                " has signed a declaration of war against the " +
                faction2.Name.ToString();
            ;
            InformationManager.ShowInquiry(new InquiryData("Sound The Alarm", display, true, false, "Ok", "Close", null, null, ""), STALibrary.Instance.STAConfiguration.PauseOnWarPeacePopup);
            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + display, new Color(1.0f, 0.0f, 0.0f)));
        }

        // Action method fired once two empires declare peace
        public void OnDeclarePeace(IFaction faction1, IFaction faction2) {
            string display =
                faction1.Leader.Name.ToString() +
                " of the " +
                faction1.Name.ToString() +
                " has signed a declaration of peace with the " +
                faction2.Name.ToString();
            ;
            InformationManager.ShowInquiry(new InquiryData("Sound The Alarm", display, true, false, "Ok", "Close", null, null, ""), STALibrary.Instance.STAConfiguration.PauseOnWarPeacePopup);
            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + display, new Color(1.0f, 0.0f, 0.0f)));
        }

        // Action method fired once the user clicks 'Track' on the popup
        public void Track() {
            Campaign.Current.VisualTrackerManager.RegisterObject(settlementToTrack);
            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                InformationManager.DisplayMessage(new InformationMessage("STALibrary: Tracking " + settlementToTrack.Name.ToString(), new Color(1.0f, 0.0f, 0.0f)));
        }

        // Check if the alert should fire (thanks to iPherian for submitting pull request that fixed alert not showing if you are not the king)
        private bool ShouldAlertForSettlement(Settlement settlement) {
            return settlement.MapFaction.Leader == Hero.MainHero || settlement.OwnerClan.Leader == Hero.MainHero;
        }

        // Returns the instance of this class, if no instance exists, creates a new instance
        public static STAAction Instance {
            get {
                if(STAAction._instance == null) {
                    STAAction._instance = new STAAction();
                }
                return STAAction._instance;
            }
        }

    }
}
