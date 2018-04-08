using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.Windows.Forms;

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
        

        public String Get_FilePath_OpenSave(FileDialog myDialog, String strExtension = "pzn")
        {
            DialogResult ret;
            int fNo = 0;

            myDialog.RestoreDirectory = true;
            myDialog.InitialDirectory  = Get_PuzzleDirName(strExtension, ref fNo);
            if (strExtension == "pzn") {
                myDialog.Filter = "Sudoku File（*.pzn）|*.pzn";
            } else if (strExtension == "txt")
            {
                myDialog.Filter = "Text File（*.txt）|*.txt";
            } else if (strExtension == "csv")
            {
                myDialog.Filter = "Csv File（*.csv）|*.csv";
            } else
            {
                myDialog.Filter = "All File（*.*）|*.*";
            }
            myDialog.FileName = "";
            if (Convert.ToString(myDialog.Tag) == "Save") {
                if (strExtension == "pzn") {
                    myDialog.FileName = "NumPlace" + String.Format("{0:D3}", fNo + 1);
                }
            }

            //'ダイアログボックスを表示し、［保存］ボタンが選択されたらファイル名を表示
            ret = myDialog.ShowDialog();
            if (ret == DialogResult.OK) {
                return myDialog.FileName;
            }

            return "";

        }


        public String Get_PuzzleDirName(String strExtension, ref int fNo) {

            String myCrossDir = "\\PuzzleData";
            String myDirName, myDrvName, strFileName;
            int currentNo = 0;
            String returnStr = System.IO.Directory.GetCurrentDirectory();
            bool endLoop = false;

            //        Get_PuzzleDirName = IO.Directory.GetCurrentDirectory

            myDirName = System.IO.Directory.GetCurrentDirectory();
            myDrvName = System.IO.Directory.GetDirectoryRoot(myDirName);
            fNo = 0;

            //F_Loop:

            do
            {
                endLoop = true;
                if (System.IO.Directory.Exists(myDirName + myCrossDir) == true)
                {
                    returnStr = myDirName + myCrossDir;
                }
                else
                {
                    if (myDirName != myDrvName)
                    {
                        myDirName = System.IO.Directory.GetParent(myDirName).FullName;
                        endLoop = false;
                        //                    GoTo F_Loop;
                    }
                }

            } while (endLoop == false);

            strFileName = "";
            if (strExtension == "pzn") {
                strFileName = "NumPlace*.pzn";
            }

            foreach (String strPuzzleFile in System.IO.Directory.GetFiles(returnStr, strFileName))
            {
                currentNo = Get_Number_From_String(System.IO.Path.GetFileName(strPuzzleFile));
                if (currentNo > fNo) {
                    fNo = currentNo;
                }
            }

            return returnStr;

        }

        public int Get_Number_From_String(String strNum)
        {
            int i;
            String curNumber = "";
            double d;

            for (i = 0; i < strNum.Length; i++)
            {
                if (double.TryParse(strNum.Substring(i, 1), out d))
                {
                    curNumber += strNum.Substring(i, 1);
                } else if (curNumber.Length > 0)
                {
                    return Convert.ToInt32(curNumber); 
                }
            }

            return 0;

        }


    }
}
