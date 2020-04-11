using System;
using System.Windows.Forms;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using HarmonyLib;

namespace SoundTheAlarm {
    public class SubModule : MBSubModuleBase {

        private List<string> _managedSettlements;
        private Settlement _settlementToTrack;

        // Initialize Sound The Alarm once the save has been loaded
        public override void OnGameLoaded(Game game, object initializerObject) {
            base.OnGameLoaded(game, initializerObject);
            try {
                _managedSettlements = new List<string>();
                CampaignEvents.VillageBeingRaided.AddNonSerializedListener(this, new Action<Village>(this.DisplayVillageRaid));
                CampaignEvents.VillageBecomeNormal.AddNonSerializedListener(this, new Action<Village>(this.FinalizeVillageRaid));
                CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.DisplaySiege));
                CampaignEvents.OnSiegeEventEndedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.FinalizeSiege));
            } catch (Exception ex) {
                MessageBox.Show("An error has occurred whilst initialising Sound The Alarm:\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        // Below is the alternate method using the Harmony library to patch the relative methods
        //protected override void OnSubModuleLoad() {
        //    base.OnSubModuleLoad();
        //    try {
        //        HourlyTickPatch.Initialize();

        //        Harmony harmony = new Harmony("mod.bannerlord.alpine");
        //        harmony.PatchAll();

        //    } catch(Exception ex) {
        //        MessageBox.Show("An error has occurred whilst initialising Sound The Alarm:\n\n" + ex.Message + "\n\n" + ex.StackTrace);
        //    }
        //}

        private bool ShouldAlertForSettlement(Settlement settlement) {
            return settlement.MapFaction.Leader == Hero.MainHero || settlement.OwnerClan.Leader == Hero.MainHero;
        }

        // Action method fired once the VilageBeingRaided event fires
        private void DisplayVillageRaid(Village v) {
            if(Hero.MainHero != null) {
                if(Hero.MainHero.IsAlive) {
                    if(ShouldAlertForSettlement(v.Settlement)) {
                        if(!_managedSettlements.Contains(v.Settlement.Name.ToString())) {
                            _managedSettlements.Add(v.Settlement.Name.ToString());
                            string display =
                                v.Settlement.Name.ToString() +
                                " is under attack by " +
                                v.Settlement.LastAttackerParty.Name.ToString() +
                                " of the " +
                                v.Settlement.LastAttackerParty.LeaderHero.MapFaction.Name.ToString() +
                                "!"
                            ;
                            
                            _settlementToTrack = v.Settlement;
                            InformationManager.ShowInquiry(new InquiryData("Sound The Alarm", display, true, true, "Track", "Close", new Action(Track), null, ""), true);
                        }
                    }
                }
            }
        }

        // Action method fired once the VillageBecomeNormal event fires
        private void FinalizeVillageRaid(Village v) {
            if(_managedSettlements.Contains(v.Settlement.Name.ToString())) {
                InformationManager.DisplayMessage(new InformationMessage(v.Settlement.Name.ToString() + " is no longer being raided"));
                _managedSettlements.Remove(v.Settlement.Name.ToString());
            }
        }

        // Action method fired once the OnSiegeEventStartedEvent event fires
        private void DisplaySiege(SiegeEvent e) {
            if (Hero.MainHero != null) {
                if (Hero.MainHero.IsAlive) {
                    if (ShouldAlertForSettlement(e.BesiegedSettlement)) {
                        if (!_managedSettlements.Contains(e.BesiegedSettlement.Name.ToString())) {
                            _managedSettlements.Add(e.BesiegedSettlement.Name.ToString());
                            string display =
                                e.BesiegedSettlement.Name.ToString() +
                                " is under attack by " +
                                e.BesiegedSettlement.LastAttackerParty.Name.ToString() +
                                " of the " +
                                e.BesiegedSettlement.LastAttackerParty.LeaderHero.MapFaction.Name.ToString() +
                                "!"
                            ;
                            _settlementToTrack = e.BesiegedSettlement;
                            InformationManager.ShowInquiry(new InquiryData("Sound The Alarm", display, true, true, "Track", "Close", new Action(Track), null, ""), true);
                        }
                    }
                }
            }
        }

        // Action method fired once the OnSiegeEventEndedEvent event fires
        private void FinalizeSiege(SiegeEvent e) {
            if (_managedSettlements.Contains(e.BesiegedSettlement.Name.ToString())) {
                _managedSettlements.Remove(e.BesiegedSettlement.Name.ToString());
            }
        }

        // Action method fired once the user clicks 'Track' on the popup
        private void Track() {
            Campaign.Current.VisualTrackerManager.RegisterObject(_settlementToTrack);
        }

    }
}
