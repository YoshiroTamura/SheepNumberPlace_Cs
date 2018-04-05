using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;


namespace SheepNumberPlace_Cs
{
    class CommonModule
    {
        private System.Random r;

        public CommonModule()
        {
//            Console.Write("Constructor");
            this.r = new System.Random();
        }


        public bool Exist_Settings(String ItemName)
        {
            foreach (SettingsProperty myCfg in Properties.Settings.Default.Properties)
            {
                if (myCfg.Name == ItemName)
                {
                    return true;
                }
            }
            return false;
        }

        public int Generate_RandomRange(int rangeMin, int rangeMax)
        {
            List<int> excludeNo = new List<int>();
            return Generate_RandomRange(rangeMin, rangeMax, excludeNo);
        }

        public int Generate_RandomRange(int rangeMin, int rangeMax, List<int> excludeNo) {

            int rnd = 0;

            do {
//                System.Random r = new System.Random();
                rnd = r.Next(rangeMax - rangeMin + 1) + rangeMin;
                if (excludeNo.Count > 0 && excludeNo.Count < (rangeMax - rangeMin + 1) && excludeNo.IndexOf(rnd) >= 0)
                {
//                    Console.Write("------>" + Convert.ToString( rnd) + "\r\n");
                    rnd = 0;
                }
            }
            while (rnd == 0);

            return rnd;

        }

        public int Generate_Random_FromList(List<int> rndList)
        {
            List<int> excludeNo = new List<int>();
            return Generate_Random_FromList(rndList, excludeNo);
        }

        public int Generate_Random_FromList(List<int> rndList, List<int> excludeNo)
        {
            int i, rnd;

            if (excludeNo.Count > 0)
            {
                for (i = 0; i < excludeNo.Count; i++)
                {
//                    do {
                        rndList.Remove(excludeNo[i]);
//                    } while (rndList.IndexOf(excludeNo[i]) >= 0);
                }
            }

            if (rndList.Count == 0)
            {
                return 0;
            }

            rnd = r.Next(rndList.Count);

            return rndList[rnd];

        }

        //'
        //'  n個の中からm個を選ぶ際の組み合わせ数を取得
        //'
        public int Combinatorics(int n, int m) {

            int i, sn = 1, sm = 1;

            for (i = 1; i <= m; i++) {
                sn = sn * (n - i + 1);
                sm = sm * i;
            }

            return Convert.ToInt32(sn / sm);

        }


    }
}
