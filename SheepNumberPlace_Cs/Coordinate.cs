using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SheepNumberPlace_Cs
{
    class Coordinate
    {
        private int cX;
        private int cY;
        private int cS;
        private int cNo;
        private int cNoB;
        private int cColorNo;
        private List<int> cProspectNo;

        //public Coordinate()
        //{
        //    this.cX = 0;
        //    this.cY = 0;
        //    this.cS = 0;
        //    this.cNo = 0;
        //    this.cNoB = 0;
        //    this.cColorNo = 0;
        //    this.cProspectNo = new List<int>();
        //}

        //public Coordinate(int fx, int fy)
        //{
        //    this.cX = fx;
        //    this.cY = fy;
        //    this.cS = 0;
        //    this.cNo = 0;
        //    this.cNoB = 0;
        //    this.cColorNo = 0;
        //    this.cProspectNo = new List<int>();
        //}

        public Coordinate(int fx = 0, int fy = 0, int fs = 0, int fn = 0, int fc = 0)
        {
            this.cX = fx;
            this.cY = fy;
            this.cS = fs;
            this.cNo = fn;
            this.cNoB = fn;
            this.cColorNo = fc;
            this.cProspectNo = new List<int>();
        }

        public int X
        {
            get
            {
                return cX;
            }

            set
            {
                cX = value;
            }
        }

        public int Y
        {
            get
            {
                return cY;
            }

            set
            {
                cY = value;
            }
        }

        public int S
        {
            get
            {
                return cS;
            }
            set
            {
                cS = value;
            }
        }

        public int No
        {
            get
            {
                return cNo;
            }
            set
            {
                cNo = value;
            }
        }

        public int NoB
        {
            get
            {
                return cNoB;
            }
            set
            {
                cNoB = value;
            }
        }

        public int ColorNo
        {
            get
            {
                return cColorNo;
            }
            set
            {
                cColorNo = value;
            }
        }

        public List<int> ProspectNo
        {
            get
            {
                return cProspectNo;
            }
            set
            {
                cProspectNo = value;
            }
        }

    
    
    
    }
}
