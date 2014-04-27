using HRCustomClasses;
using HREngine.API;
using HREngine.API.Actions;
using HREngine.API.Utilities;
using HREngine.Basics;
using HREngine.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
   public class JuHunter : HREngine.Basics.BasicBot
   {
      public JuHunter()
         : base()
      {
      }

      protected override bool IsDefaultHealingEnabled()
      {
         return false;
      }

      protected override ActionBase PlayCardsToField()
      {
         var EnemyState = new PlayerState(HRPlayer.GetEnemyPlayer());
         var LocalState = new PlayerState(HRPlayer.GetLocalPlayer());
         var Sorting = new Sorting();

         // Retreive cards that can be played.
         List<HRCard> playableList =
            GetPlayableCards(HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.HAND));

         // Update list with conditions matching this custom class.
         GetConditionalCards(playableList, EnemyState, LocalState);

         // Sort by cost (ascending)
         Sorting.SortByCost(ref playableList);

         // Finally sort by custom priority
         playableList = SortByPriority(playableList);

         // Taunts
         if (LocalState.TauntMinions.Count == 0)
         {
            foreach (var minion in playableList)
            {
               HREntity Target = null;
               if (minion.GetEntity().HasTaunt() && 
                  CanHandleCard(minion, EnemyState, LocalState, ref Target))
               {
                  return new PlayCardAction(minion, Target);
               }
            }
         }

         // Charges
         foreach (var minion in playableList)
         {
            HREntity Target = null;
            if (minion.GetEntity().HasCharge() &&
               CanHandleCard(minion, EnemyState, LocalState, ref Target))
            {
               return new PlayCardAction(minion, Target);
            }
         }

         // All other available
         foreach (var minion in playableList)
         {
            HREntity Target = null;
            if (CanHandleCard(minion, EnemyState, LocalState, ref Target))
               return new PlayCardAction(minion, Target);
         }

         // Use Hero Power that make sense at last...
         if (LocalState.Player.GetHeroPower().GetCost() <= LocalState.Mana)
         {
            if (LocalState.Player.GetHero().GetClass() == HRClass.HUNTER)
            {
               if (HRBattle.CanUseCard(LocalState.Player.GetHeroPower()))
                  return new PlayCardAction(LocalState.Player.GetHeroPower().GetCard());
            }
         }

         if (LocalState.Player.HasWeapon())
         {
            if (HRBattle.CanUseCard(LocalState.Player.GetHeroCard().GetEntity()))
            {
               return new AttackAction(LocalState.Player.GetWeaponCard().GetEntity(), GetNextAttackToAttack());
            }
         }

         return null;
      }

      private List<HRCard> SortByPriority(List<HRCard> conditionalList)
      {
         var sorting = new Sorting();
         sorting.SortByCost(ref conditionalList);

         var prioritySystem = new Dictionary<string, int>()
         {

         };

         conditionalList.Sort(delegate(HRCard PartX, HRCard PartY)
         {
            int partXPriority = 99;
            int partYPriority = 99;

            if (prioritySystem.ContainsKey(PartX.GetEntity().GetCardId().ToUpper()))
               partXPriority = prioritySystem[PartX.GetEntity().GetCardId().ToUpper()];
            if (prioritySystem.ContainsKey(PartY.GetEntity().GetCardId().ToUpper()))
               partYPriority = prioritySystem[PartY.GetEntity().GetCardId().ToUpper()];

            if (partXPriority < partYPriority || partXPriority == partYPriority)
               return -1;

            return 1;
         });

         return conditionalList;
      }

      private List<HRCard> GetPlayableCards(List<HRCard> list)
      {
         int manaLeft = HRPlayer.GetLocalPlayer().GetNumAvailableResources();
         List<HRCard> result = new List<HRCard>();
         foreach (var item in list)
         {
            if (item.GetEntity().GetCost() <= manaLeft && HRBattle.CanUseCard(item.GetEntity()))
               result.Add(item);
         }
         return result;
      }

      private List<HRCard> GetConditionalCards(
         List<HRCard> playableList, PlayerState EnemyState, PlayerState LocalState)
      {
         List<HRCard> result = new List<HRCard>();
         foreach (var card in playableList)
         {
            HREntity tmp = null;
            if (CanHandleCard(card, EnemyState, LocalState, ref tmp))
               result.Add(card);
         }
         return result;
      }

      private bool CanHandleCard(
         HRCard item, PlayerState EnemyState, PlayerState LocalState, ref HREntity Target)
      {
         string cardID = item.GetEntity().GetCardId().ToUpper();
         switch (cardID)
         {
            case "CS2_122": // "Raid Leader"
               return LocalState.Minions > 0;
            case "EX1_066": // "Acidic Swamp Ooze"
               return EnemyState.Player.HasWeapon() || LocalState.Minions < EnemyState.Minions;
            default:
               break;
         }
         return true;
      }
   }
}
