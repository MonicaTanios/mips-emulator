using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MipsEmulator
{
    public partial class Form1 : Form
    {
        public static Dictionary<string, object> MipsRegisters = new Dictionary<string, object>();
        public static Hashtable InstructionMemory = new Hashtable();
        public static DataTable PipelineRegistersDataSource;
        public static DataTable DataMemoryDataSource;
        public string[] MachineCodeLines;
        public int ClockCycleValue;
        public int PcCurrentVal;
        public int InitialPcVal;
        public Form1()
        {
            InitializeComponent();
        }

        private void FormDefaultValues()
        {
            //Initialize MIPS Registers DataGridView
            var mipsRegistersDataSource = new DataTable();
            mipsRegistersDataSource.Columns.Add("Register");
            mipsRegistersDataSource.Columns.Add("Value");
            mipsRegistersDataSource.Rows.Add(string.Concat("$", 0), 0);
            for (var i = 1; i < 32; i++) mipsRegistersDataSource.Rows.Add(string.Concat("$", i), i + 100);
            MipsRegistersDataGrid.RowHeadersVisible = false;
            MipsRegistersDataGrid.DataSource = mipsRegistersDataSource;
            MipsRegistersDataGrid.BackgroundColor = System.Drawing.SystemColors.Control;

            //Initialize Pipeline Registers DataGridView
            PipelineRegistersDataSource = new DataTable();
            PipelineRegistersDataSource.Columns.Add("Register");
            PipelineRegistersDataSource.Columns.Add("Value");
            PipelineRegistersDataSource.Rows.Add("IF/ID", 0);
            PipelineRegistersDataSource.Rows.Add("ID/EX", 0);
            PipelineRegistersDataSource.Rows.Add("EX/MEM", 0);
            PipelineRegistersDataSource.Rows.Add("MEM/WB", 0);
            PipelineRegisters.RowHeadersVisible = false;
            PipelineRegisters.DataSource = PipelineRegistersDataSource;
            PipelineRegisters.BackgroundColor = System.Drawing.SystemColors.Control;

            //Initialize Data Memory DataGridView
            DataMemoryDataSource = new DataTable();
            DataMemoryDataSource.Columns.Add("Address");
            DataMemoryDataSource.Columns.Add("Value");
            DataMemory.RowHeadersVisible = false;
            DataMemory.DataSource = DataMemoryDataSource;
            DataMemory.BackgroundColor = System.Drawing.SystemColors.Control;

            MipsRegisters = GetDict(mipsRegistersDataSource);

            userCode.Clear();
            PCVal.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormDefaultValues();
        }

        internal Dictionary<string, object> GetDict(DataTable dt)
        {
            return dt.AsEnumerable()
                .ToDictionary<DataRow, string, object>(row => row.Field<string>(0),
                    row => row.Field<object>(1));
        }
    }
}
