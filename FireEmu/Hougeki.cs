using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireEmu
{
    class HougekiData
    {
        public BattleHitStatus Critical;
        public int Damage;
    }
    class Hougeki
    {
        protected double valance1;

        protected double valance2;

        protected double valance3;

        protected readonly Random r;

        public double calHitProb;
        public double calAvoProb;
        public double calHitRate;
        public double calCriticalRate;

        public Hougeki()
        {
            this.valance1 = 5.0;
            this.valance2 = 90.0;
            this.valance3 = 1.3;
            r = new Random();
        }
        public Hougeki(int valance)
        {
            this.valance1 = 5.0;
            this.valance2 = valance;
            this.valance3 = 1.3;
            r = new Random();
        }
        public HougekiData getAttackData(Mem_ship attacker, List<Mst_slotitem> attackerSlot, List<int> attackerSlotLevel, Mem_ship atackTarget)
        {
            HougekiData hougeki = new HougekiData();
            int num2;
            int num3;
            num2 = this.getHougAttackValue(attacker, attackerSlot, attackerSlotLevel, atackTarget);
            num3 = this.getHougHitProb(attacker, attackerSlot, attackerSlotLevel);
            calHitProb = num3;

            int battleAvo = getBattleAvo(atackTarget);
            calAvoProb = battleAvo;
            BattleHitStatus battleHitStatus = this.getHitStatus(num3, battleAvo, attacker, atackTarget, this.valance3);
            int num4 = this.setDamageValue(battleHitStatus, num2, atackTarget.Soukou, attacker, atackTarget);
            hougeki.Damage = (num4);
            hougeki.Critical = (battleHitStatus);
            return hougeki;
        }

        protected virtual int getHougAttackValue(Mem_ship atk_ship, List<Mst_slotitem> atk_slot, List<int> slotLevel, Mem_ship def_ship)
        {
            int num = 150;
            List<int> list = slotLevel;
            double num2 = 0.0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            using (var enumerator = Enumerable.Select(atk_slot, (Mst_slotitem obj, int idx) => new
            {
                obj,
                idx
            }).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    Mst_slotitem obj2 = current.obj;
                    num2 += this.getHougSlotPlus_Attack(obj2, list[current.idx]);
                }
            }
            double num6 = this.valance1 + (double)atk_ship.Houg + num2;
            DamageState damageState = atk_ship.Get_DamageState();
            double num7 = 1.0;
            if (damageState == DamageState.Tyuuha)
            {
                num7 = 0.7;
            }
            else if (damageState == DamageState.Taiha)
            {
                num7 = 0.4;
            }
            num6 *= num7;
            num6 += this.getHougItemAtackHosei(atk_ship, atk_slot);
            if (num6 > (double)num)
            {
                num6 = (double)num + Math.Sqrt(num6 - (double)num);
            }
            return (int)num6;
        }

        protected virtual double getHougItemAtackHosei(Mem_ship ship, List<Mst_slotitem> mst_item)
        {
            if (mst_item.Count == 0)
            {
                return 0.0;
            }
            if (ship.Stype != 3 && ship.Stype != 4 && ship.Stype != 21)
            {
                return 0.0;
            }
            ILookup<int, Mst_slotitem> lookup = Enumerable.ToLookup<Mst_slotitem, int>(mst_item, (Mst_slotitem x) => x.Id);
            int num = 0;
            if (lookup.Contains(4))
            {
                num += Enumerable.Count<Mst_slotitem>(lookup[4]);
            }
            if (lookup.Contains(11))
            {
                num += Enumerable.Count<Mst_slotitem>(lookup[11]);
            }
            int num2 = 0;
            if (lookup.Contains(119))
            {
                num2 += Enumerable.Count<Mst_slotitem>(lookup[119]);
            }
            if (lookup.Contains(65))
            {
                num2 += Enumerable.Count<Mst_slotitem>(lookup[65]);
            }
            if (lookup.Contains(139))
            {
                num2 += Enumerable.Count<Mst_slotitem>(lookup[139]);
            }
            return 1.0 * Math.Sqrt((double)num) + 2.0 * Math.Sqrt((double)num2);
        }

        protected virtual int getHougHitProb( Mem_ship atk_ship, List<Mst_slotitem> atk_slot, List<int> slotLevel)
        {
            double num = 0.0;
            List<int> list = slotLevel;
            double num2 = 0.0;
            int num3 = 0;
            using (var enumerator = Enumerable.Select(atk_slot, (Mst_slotitem obj, int idx) => new
            {
                obj,
                idx
            }).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    num3 += current.obj.Houm;
                    num2 += this.getHougSlotPlus_Hit(current.obj, list[current.idx]);
                }
            }
            double num4 = Math.Sqrt((double)atk_ship.Luck * 1.5) + Math.Sqrt((double)atk_ship.Level) * 2.0 + (double)num3;
            double num5 = this.valance2 + num4 + num2;
            FatigueState fatigueState = atk_ship.Get_FatigueState();
            double num6 = 1.0;
            if (fatigueState == FatigueState.Exaltation)
            {
                num6 = 1.2;
            }
            else if (fatigueState == FatigueState.Light)
            {
                num6 = 0.8;
            }
            else if (fatigueState == FatigueState.Distress)
            {
                num6 = 0.5;
            }
            num5 *= num6;
            num5 = this.getHougHitProbUpValue(num5, atk_ship, atk_slot);
            double num7 = num5 * num;
            num5 += num7;
            return (int)num5;
        }

        protected virtual double getHougSlotPlus_Hit(Mst_slotitem mstItem, int slotLevel)
        {
            double result = 0.0;
            if (slotLevel <= 0)
            {
                return result;
            }
            if (mstItem.Api_mapbattle_type3 == 5 || mstItem.Api_mapbattle_type3 == 22)
            {
                return result;
            }
            if ((mstItem.Api_mapbattle_type3 == 12 || mstItem.Api_mapbattle_type3 == 13) && mstItem.Houm > 2)
            {
                result = Math.Sqrt((double)slotLevel) * 1.7;
            }
            else if (mstItem.Api_mapbattle_type3 == 21 || mstItem.Api_mapbattle_type3 == 14 || mstItem.Api_mapbattle_type3 == 40 || mstItem.Api_mapbattle_type3 == 16 || mstItem.Api_mapbattle_type3 == 27 || mstItem.Api_mapbattle_type3 == 28 || mstItem.Api_mapbattle_type3 == 17 || mstItem.Api_mapbattle_type3 == 15)
            {
                result = Math.Sqrt((double)slotLevel);
            }
            return result;
        }

        protected virtual double getHougHitProbUpValue(double hit_prob, Mem_ship atk_ship, List<Mst_slotitem> atk_slot)
        {
            HashSet<int> hashSet = new HashSet<int>();
            hashSet.Add(8);
            hashSet.Add(10);
            hashSet.Add(3);
            hashSet.Add(9);
            hashSet.Add(4);
            hashSet.Add(21);
            HashSet<int> hashSet2 = hashSet;
            if (!hashSet2.Contains(atk_ship.Stype))
            {
                return hit_prob;
            }
            if (atk_ship.Stype == 9 && atk_ship.Taik > 92)
            {
                return hit_prob;
            }
            Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> arg_A4_0 = dictionary;
            int arg_A4_1 = 1;
            List<int> list = new List<int>();
            list.Add(9);
            arg_A4_0.Add(arg_A4_1, list);
            Dictionary<int, List<int>> arg_BE_0 = dictionary;
            int arg_BE_1 = 2;
            list = new List<int>();
            list.Add(117);
            arg_BE_0.Add(arg_BE_1, list);
            Dictionary<int, List<int>> arg_E0_0 = dictionary;
            int arg_E0_1 = 3;
            list = new List<int>();
            list.Add(105);
            list.Add(8);
            arg_E0_0.Add(arg_E0_1, list);
            Dictionary<int, List<int>> arg_11D_0 = dictionary;
            int arg_11D_1 = 4;
            list = new List<int>();
            list.Add(7);
            list.Add(103);
            list.Add(104);
            list.Add(76);
            list.Add(114);
            arg_11D_0.Add(arg_11D_1, list);
            Dictionary<int, List<int>> arg_146_0 = dictionary;
            int arg_146_1 = 5;
            list = new List<int>();
            list.Add(133);
            list.Add(137);
            arg_146_0.Add(arg_146_1, list);
            Dictionary<int, List<int>> arg_168_0 = dictionary;
            int arg_168_1 = 6;
            list = new List<int>();
            list.Add(4);
            list.Add(11);
            arg_168_0.Add(arg_168_1, list);
            Dictionary<int, List<int>> arg_197_0 = dictionary;
            int arg_197_1 = 7;
            list = new List<int>();
            list.Add(119);
            list.Add(65);
            list.Add(139);
            arg_197_0.Add(arg_197_1, list);
            Dictionary<int, List<int>> dictionary2 = dictionary;
            Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
            ILookup<int, Mst_slotitem> lookup = Enumerable.ToLookup<Mst_slotitem, int>(atk_slot, (Mst_slotitem x) => x.Id);
            using (Dictionary<int, List<int>>.Enumerator enumerator = dictionary2.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<int, List<int>> current = enumerator.Current;
                    int num = 0;
                    using (List<int>.Enumerator enumerator2 = current.Value.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            int current2 = enumerator2.Current;
                            if (lookup.Contains(current2))
                            {
                                num += Enumerable.Count<Mst_slotitem>(lookup[current2]);
                            }
                        }
                    }
                    dictionary3.Add(current.Key, num);
                }
            }
            double num2 = 1.0;
            double num3 = hit_prob;
            if (atk_ship.Level >= 100)
            {
                num2 = 0.6;
            }
            int num4 = dictionary3[1];
            int num5 = dictionary3[2];
            int num6 = dictionary3[3];
            int num7 = dictionary3[4];
            int num8 = dictionary3[5];
            int num9 = dictionary3[6];
            int num10 = dictionary3[7];
            if (atk_ship.Stype == 8)
            {
                num3 = num3 - 10.0 * num2 * Math.Sqrt((double)num4) - 5.0 * num2 * Math.Sqrt((double)num6) - 7.0 * num2 * Math.Sqrt((double)num5);
                num3 -= 2.0 * num2 * Math.Sqrt((double)num8);
                if (atk_ship.Yomi.Equals("ビスマルク") || atk_ship.Yomi.Equals("リットリオ・イタリア") || atk_ship.Yomi.Equals("ローマ"))
                {
                    num3 += 3.0 * num2 * Math.Sqrt((double)num8);
                }
                num3 += 4.0 * Math.Sqrt((double)num7);
            }
            else if (atk_ship.Stype == 10)
            {
                num3 = num3 - 7.0 * num2 * Math.Sqrt((double)num4) - 3.0 * num2 * Math.Sqrt((double)num5);
                num3 += 2.0 * num2 * Math.Sqrt((double)num8);
                num3 = num3 + 4.0 * Math.Sqrt((double)num7) + 2.0 * Math.Sqrt((double)num6);
            }
            else if (atk_ship.Stype == 9)
            {
                num3 = num3 - 7.0 * num2 * Math.Sqrt((double)num4) - 3.0 * num2 * Math.Sqrt((double)num5);
                num3 += 2.0 * num2 * Math.Sqrt((double)num8);
                num3 = num3 + 2.0 * Math.Sqrt((double)num7) + 2.0 * Math.Sqrt((double)num6);
            }
            else if (atk_ship.Stype == 3 || atk_ship.Stype == 4 || atk_ship.Stype == 21)
            {
                num3 = num3 + 4.0 * Math.Sqrt((double)num9) + 3.0 * Math.Sqrt((double)num10) - 2.0;
            }
            return num3;
        }

        protected int getBattleAvo(Mem_ship targetShip)
        {
            double num = (double)targetShip.Kaihi + Math.Sqrt((double)(targetShip.Luck * 2));
            double num2 = (double)((int)num);
            if (num2 >= 65.0)
            {
                double num3 = 55.0 + Math.Sqrt(num2 - 65.0) * 2.0;
                num2 = (double)((int)num3);
            }
            else if (num2 >= 40.0)
            {
                double num4 = 40.0 + Math.Sqrt(num2 - 40.0) * 3.0;
                num2 = (double)((int)num4);
            }
            return (int)num2;
        }

        protected virtual BattleHitStatus getHitStatus(int hitProb, int avoProb, Mem_ship attackShip, Mem_ship targetShip, double cliticalKeisu)
        {
            double num = (double)(hitProb - avoProb);
            FatigueState fatigueState = targetShip.Get_FatigueState();
            if (num <= 10.0)
            {
                num = 10.0;
            }
            double num2 = 1.0;
            if (fatigueState == FatigueState.Exaltation)
            {
                num2 = 0.7;
            }
            else if (fatigueState == FatigueState.Normal)
            {
                num2 = 1.0;
            }
            else if (fatigueState == FatigueState.Light)
            {
                num2 = 1.2;
            }
            else if (fatigueState == FatigueState.Distress)
            {
                num2 = 1.4;
            }
            num *= num2;
            if (num >= 96.0)
            {
                num = 96.0;
            }
            double num3 = 0.0;
            double num4 = 1.0;
            int num5 = r.Next(100);
            double num6 = Math.Sqrt(num) * cliticalKeisu;
            calHitRate = num;
            calCriticalRate = num6;
            if ((double)num5 <= num6)
            {
                return BattleHitStatus.Critical;
            }
            if ((double)num5 > num)
            {
                return BattleHitStatus.Miss;
            }
            return BattleHitStatus.Normal;
        }

        protected virtual int setDamageValue(BattleHitStatus hitType, int atkPow, int defPow, Mem_ship attacker, Mem_ship target)
        {
            if (hitType == BattleHitStatus.Miss)
            {
                return 0;
            }
            if (hitType == BattleHitStatus.Critical)
            {
                atkPow = (int)((double)atkPow * 1.5);
            }
            double def = (double)r.Next(defPow);
            double num = (double)atkPow - ((double)defPow * 0.7 + def * 0.6);
            double num2 = 100.0;
            if (num2 < 50.0)
            {
                num = Math.Floor(num * num2 / 50.0);
            }
            int num4 = (int)num;
            if (num4 < 1)
            {
                int num5 = r.Next(target.Nowhp);
                num4 = (int)((double)target.Nowhp * 0.06 + (double)num5 * 0.08);
            }
            return num4;
        }

        protected virtual double getHougSlotPlus_Attack(Mst_slotitem mstItem, int slotLevel)
        {
            double result = 0.0;
            if (slotLevel <= 0)
            {
                return result;
            }
            if (mstItem.Api_mapbattle_type3 == 5 || mstItem.Api_mapbattle_type3 == 22)
            {
                return result;
            }
            double num = 2.0;
            if (mstItem.Houg > 12)
            {
                num = 3.0;
            }
            if (mstItem.Api_mapbattle_type3 == 12 || mstItem.Api_mapbattle_type3 == 13 || mstItem.Api_mapbattle_type3 == 16 || mstItem.Api_mapbattle_type3 == 17 || mstItem.Api_mapbattle_type3 == 27 || mstItem.Api_mapbattle_type3 == 28)
            {
                num = 0.0;
            }
            else if (mstItem.Api_mapbattle_type3 == 14 || mstItem.Api_mapbattle_type3 == 15 || mstItem.Api_mapbattle_type3 == 40)
            {
                num = 1.5;
            }
            return num * Math.Sqrt((double)slotLevel) * 0.5;
        }
    }
}
