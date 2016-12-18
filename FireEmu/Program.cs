using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FireEmu
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "";
            string outputFile = "";
            int runTime = 500;
            foreach (string arg in args)
            {
                string[] arr = arg.Trim().Split('=');
                switch (arr[0].Trim())
                {
                    case "-input" :
                        inputFile = arr[1].Trim();
                        break;
                    case "-time":
                        runTime = int.Parse(arr[1].Trim());
                        break;
                    case "-output":
                        outputFile = arr[1].Trim();
                        break;
                }
            }
            //Read from file
            FileStream file = File.OpenRead(inputFile);
            StreamReader reader = new StreamReader(file);
            TestFile tf = TestFile.parse(reader.ReadToEnd());
            reader.Close();
            file.Close();
            //Prepare result file
            FileStream outFile = null;
            StreamWriter writer = null;
            if (!string.IsNullOrEmpty(outputFile))
            {
                outFile = File.OpenWrite(outputFile);
                writer = new StreamWriter(outFile);
            }
            Hougeki hougeki = new Hougeki(tf.valance);
            List<HougekiData> hits = new List<HougekiData>();
            List<HougekiData> criticals = new List<HougekiData>();
            List<HougekiData> misses = new List<HougekiData>();
            for (int i = 0; i < runTime; i++)
            {
                HougekiData data = hougeki.getAttackData(tf.attacker, tf.attacker.slots, tf.attacker.slotLevel, tf.target);
                StringBuilder sb = new StringBuilder();
                sb.Append("Hit status : ");
                switch (data.Critical)
                {
                    case BattleHitStatus.Critical: sb.Append("Critical, "); criticals.Add(data); break;
                    case BattleHitStatus.Miss: sb.Append("Miss, "); misses.Add(data); break;
                    case BattleHitStatus.Normal: sb.Append("Hit, "); hits.Add(data); break;
                }
                sb.Append("Damage : " + data.Damage);
                Console.WriteLine(sb.ToString());
                if (writer != null)
                {
                    writer.WriteLine(sb);
                }
            }
            Console.WriteLine("命中率 : " + (1 - ((double)(misses.Count)) / runTime) + ";");
            Console.WriteLine("暴击率 : " + (((double)(criticals.Count)) / runTime) + ";");
            Console.WriteLine("命中预期 : " + hougeki.calHitProb + ";");
            Console.WriteLine("回避预期 : " + hougeki.calAvoProb + ";");
            Console.WriteLine("计算命中率 : " + hougeki.calHitRate + ";");
            Console.WriteLine("计算暴击率 : " + hougeki.calCriticalRate + ";");
            if (writer != null)
            {
                writer.WriteLine("命中率 : " + (1 - ((double)(misses.Count)) / runTime) + ";");
                writer.WriteLine("暴击率 : " + (((double)(criticals.Count)) / runTime) + ";");
                writer.WriteLine("命中预期 : " + hougeki.calHitProb + ";");
                writer.WriteLine("回避预期 : " + hougeki.calAvoProb + ";");
                writer.WriteLine("计算命中率 : " + hougeki.calHitRate + ";");
                writer.WriteLine("计算暴击率 : " + hougeki.calCriticalRate + ";");
                writer.Close();
                writer.Close();
            }

            Console.WriteLine("Press Enter to exit");
            Console.Read();
        }


     
    }
}
