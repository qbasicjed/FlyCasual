﻿using Upgrade;
using Ship;
using System.Collections.Generic;
using System;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class AfterBurners : GenericUpgrade
    {
        public AfterBurners() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "AfterBurners",
                UpgradeType.Modification,
                cost: 8,
                abilityType: typeof(Abilities.SecondEdition.AfterBurnersAbility),
                charges: 2,
                restriction: new BaseSizeRestriction(BaseSize.Small),
                seImageNumber: 70
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a speed 3-5 maneuver you may spend 1 charge to perform a boost action, even while stressed.
    public class AfterBurnersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            //AI doesn't use ability
            if (HostShip.Owner.UsesHotacAiRules) return;

            if (HostShip.AssignedManeuver.Speed >= 3 && HostShip.AssignedManeuver.Speed <= 5 && !HostShip.IsBumped && HostUpgrade.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman("AfterBurners: You may spend 1 charge to perform a boost action");

            HostShip.BeforeFreeActionIsPerformed += RegisterSpendChargeTrigger;
            HostShip.AskPerformFreeAction(new BoostAction() { CanBePerformedWhileStressed = true }, CleanUp);
        }

        private void RegisterSpendChargeTrigger(GenericAction action)
        {
            HostShip.BeforeFreeActionIsPerformed -= RegisterSpendChargeTrigger;
            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate {
                    HostUpgrade.SpendCharge();
                    Triggers.FinishTrigger();
                }
            );
        }

        private void CleanUp()
        {
            HostShip.BeforeFreeActionIsPerformed -= RegisterSpendChargeTrigger;
            Triggers.FinishTrigger();
        }
    }
}