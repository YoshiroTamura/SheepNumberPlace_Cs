using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SheepNumberPlace_Cs
{
    class SudokuGrid
    {
        private List<int> cProspectNo;
        private int cFixNo;
        private int cAssignedNo;
        private List<int> cExcludeNo;
        private bool cLocked;
        private bool cFixError;
        private Color cForeColor;
        private Color cBackColor;
        private List<int> cMemoNo;

        public SudokuGrid()
        {
            this.cProspectNo = new List<int>();
            this.cExcludeNo = new List<int>();
            this.cMemoNo = new List<int>();
            this.cLocked = false;
            this.cFixError = false;
            this.cFixNo = 0;
            this.cForeColor = Color.Black;
            this.cBackColor = Color.White;
            Reset_ProspectNo();
        }

        public void Reset_ProspectNo()
        {
            int i;
            this.cProspectNo.Clear();
            for (i = 1; i <= 9; i++)
            {
                this.cProspectNo.Add(i);
            }
        }

        public void Copy(SudokuGrid baseSudokuGrid)
        {
            this.cProspectNo.Clear();
            this.cExcludeNo.Clear();
            this.cMemoNo.Clear();
            this.cProspectNo.AddRange(baseSudokuGrid.ProspectNo);
            this.cExcludeNo.AddRange(baseSudokuGrid.ExcludeNo);
            this.cMemoNo.AddRange(baseSudokuGrid.MemoNo);
            this.cLocked = baseSudokuGrid.Locked;
            this.cFixError = baseSudokuGrid.FixError;
            this.cFixNo = baseSudokuGrid.FixNo;
            this.cForeColor = baseSudokuGrid.ForeColor;
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

        public List<int> ExcludeNo
        {
            get
            {
                return cExcludeNo;
            }

            set
            {
                cExcludeNo = value;
            }
        }

        public int FixNo
        {
            get
            {
                return cFixNo;
            }

            set
            {
                cFixNo = value;
            }
        }

        public Color ForeColor
        {
            get
            {
                return cForeColor;
            }

            set
            {
                cForeColor = value;
            }
        }

        public Color BackColor
        {
            get
            {
                return cBackColor;
            }

            set
            {
                cBackColor = value;
            }
        }

        public int AssignedNo
        {
             get
            {
                return cAssignedNo;
            }

            set
            {
                cAssignedNo = value;
            }
        }

        public bool Locked
        {
            get
            {
                return cLocked;
            }

            set
            {
                cLocked = value;
            }
        }

        public bool FixError
        {
            get
            {
                return cFixError;
            }

            set
            {
                cFixError = value;
            }
        }

        public List<int> MemoNo
        {
            get
            {
                return cMemoNo;
            }

            set
            {
                cMemoNo = value;
            }
        }

    }

}
