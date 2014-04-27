using HREngine.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRCustomClasses
{
   public class Sorting
   {
      public Sorting()
      {
      }

      public enum SortMode
      {
         Ascending = 0,
         Descending = 1
      };

      public void SortByCost(ref List<HRCard> set, SortMode order = SortMode.Ascending)
      {
         set.Sort(delegate(HRCard X, HRCard Y)
         {
            if (order == SortMode.Ascending)
            {
               if (X.GetEntity().GetCost() < Y.GetEntity().GetCost())
                  return -1; // Y is Greater

               return 1; // X is Greater
            }
            else
            {
               if (X.GetEntity().GetCost() > Y.GetEntity().GetCost())
                  return 1;

               return -1;
            }
         });
      }

      public void SortByHealth(ref List<HREntity> set, SortMode order = SortMode.Ascending)
      {
         set.Sort(delegate(HREntity X, HREntity Y)
         {
            if (order == SortMode.Ascending)
            {
               if (X.GetRemainingHP() < Y.GetRemainingHP())
                  return -1; // Y is Greater

               return 1; // X is Greater
            }
            else
            {
               if (X.GetRemainingHP() > Y.GetRemainingHP())
                  return 1;

               return -1;
            }
         });
      }

      public void SortByAttack(ref List<HREntity> set, SortMode order = SortMode.Ascending)
      {
         set.Sort(delegate(HREntity X, HREntity Y)
         {
            if (order == SortMode.Ascending)
            {
               if (X.GetATK() < Y.GetATK())
                  return -1; // Y is Greater

               return 1; // X is Greater
            }
            else
            {
               if (X.GetATK() > Y.GetATK())
                  return 1;

               return -1;
            }
         });
      }
   }
}
