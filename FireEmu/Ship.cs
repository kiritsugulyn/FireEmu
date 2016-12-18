using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace FireEmu
{
    [DataContract]
    class Mem_ship
    {
        [DataMember]
        public int Houg;

        [DataMember]
        public int Stype;

        [DataMember]
        public int Luck;

        [DataMember]
        public int Level;

        [DataMember]
        public int Taik;

        [DataMember]
        public String Yomi;

        [DataMember]
        public int Kaihi;

        [DataMember]
        public int Nowhp;

        [DataMember]
        public int Soukou;

        [DataMember]
        public int Status;

        [DataMember]
        public List<Mst_slotitem> slots;

        [DataMember]
        public List<int> slotLevel;

        public Mem_ship()
        {
            slots = new List<Mst_slotitem>();
            slotLevel = new List<int>();
        }

        public DamageState Get_DamageState()
        {
            if ((double)Nowhp / Taik > 0.75) return DamageState.Nromal;
            if ((double)Nowhp / Taik > 0.5) return DamageState.Syouha;
            if ((double)Nowhp / Taik > 0.25) return DamageState.Tyuuha;
            return DamageState.Taiha;

        }

        public FatigueState Get_FatigueState()
        {
            if (Status > 49) return FatigueState.Exaltation;
            if (Status > 29) return FatigueState.Normal;
            if (Status > 20) return FatigueState.Light;
            return FatigueState.Distress;
        }

        public static Mem_ship parse(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (Mem_ship)new DataContractJsonSerializer(typeof(Mem_ship)).ReadObject(ms);
            }
        }

    }

    [DataContract]
    class Mst_slotitem
    {
        [DataMember]
        public int Id;

        [DataMember]
        public int Houm;

        [DataMember]
        public int Api_mapbattle_type3;

        [DataMember]
        public int Houg;
    }

    enum DamageState
    {
        Nromal,
        Tyuuha,
        Taiha,
        Syouha
    }

    enum FatigueState
    {
        Exaltation,
        Light,
        Distress,
        Normal
    }

    enum BattleHitStatus
    {
        Normal,
        Miss,
        Critical
    }

    [DataContract]
    class TestFile
    {
        [DataMember]
        public int valance;

        [DataMember]
        public Mem_ship attacker;

        [DataMember]
        public Mem_ship target;

        public static TestFile parse(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (TestFile)new DataContractJsonSerializer(typeof(TestFile)).ReadObject(ms);
            }
        }

    }

}
