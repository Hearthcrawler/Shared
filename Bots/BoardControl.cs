using HRCustomClasses;
using HREngine.API;
using HREngine.API.Actions;
using HREngine.API.Utilities;
using HREngine.Basics;
using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
   public class BoardControl : HREngine.Basics.BasicBot
   {
      public BoardControl()
      {
         HRLog.Write("BoardControl custom class initialized.");
      }

      protected override HREntity GetNextAttackToAttack()
      {
         var enemyState = new PlayerState(HRPlayer.GetEnemyPlayer());
         if (enemyState.Minions > 0)
         {
            var list = enemyState.TauntMinions;
            if (list.Count == 0)
            {
               list = enemyState.AttackableMinions;
            }

            var sorting = new Sorting();
            sorting.SortByHealth(ref list);

            foreach (var minion in list)
            {
               if (minion.CanBeAttacked())
                  return minion;
            }
         }

         return enemyState.Player.GetHero();
      }
   }
}
