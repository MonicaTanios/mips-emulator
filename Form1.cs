using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MipsEmulator
{
    public partial class MipsEmulator : Form
    {
        public static Dictionary<string, object> MipsRegisters = new Dictionary<string, object>();
        public static Hashtable InstructionMemory = new Hashtable();
        public static Hashtable DataMemory = new Hashtable();
        public static DataTable PipelineRegistersDataSource;
        public static DataTable DataMemoryDataSource;
        public string[] MachineCodeLines;
        public int ClockCycleValue;
        public int PcCurrentVal = -1;
        public int InitialPcVal;
        public MipsEmulator()
        {
            InitializeComponent();
        }

        private void FormDefaultValues()
        {
            // Initialize MIPS Registers DataGridView
            var mipsRegistersDataSource = new DataTable();
            mipsRegistersDataSource.Columns.Add("Register");
            mipsRegistersDataSource.Columns.Add("Value");
            mipsRegistersDataSource.Rows.Add(string.Concat("$", 0), 0);
            for (var i = 1; i < 32; i++) mipsRegistersDataSource.Rows.Add(string.Concat("$", i), i + 100);
            MipsRegistersDataGrid.RowHeadersVisible = false;
            MipsRegistersDataGrid.DataSource = mipsRegistersDataSource;
            MipsRegistersDataGrid.BackgroundColor = SystemColors.Control;

            // Initialize Pipeline Registers DataGridView
            PipelineRegistersDataSource = new DataTable();
            PipelineRegistersDataSource.Columns.Add("Register");
            PipelineRegistersDataSource.Columns.Add("Value");
            PipelineRegistersDataSource.Rows.Add("IF/ID", 0);
            PipelineRegistersDataSource.Rows.Add("ID/EX", 0);
            PipelineRegistersDataSource.Rows.Add("EX/MEM", 0);
            PipelineRegistersDataSource.Rows.Add("MEM/WB", 0);
            PipelineRegistersDataGridView.RowHeadersVisible = false;
            PipelineRegistersDataGridView.DataSource = PipelineRegistersDataSource;
            PipelineRegistersDataGridView.BackgroundColor = SystemColors.Control;

            // Initialize Data Memory DataGridView
            DataMemoryDataSource = new DataTable();
            DataMemoryDataSource.Columns.Add("Address");
            DataMemoryDataSource.Columns.Add("Value");
            DataMemoryDataGridView.RowHeadersVisible = false;
            DataMemoryDataGridView.DataSource = DataMemoryDataSource;
            DataMemoryDataGridView.BackgroundColor = SystemColors.Control;

            MipsRegisters = GetDict(mipsRegistersDataSource);

            // Clear Text Fields
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
                .ToDictionary(row => row.Field<string>(0),
                    row => row.Field<object>(1));
        }

        private void Fetch()
        {
            if (PcCurrentVal >= (InitialPcVal + MachineCodeLines.Length * 4))
            {
                foreach (DataGridViewRow row in PipelineRegistersDataGridView.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "IF/ID") continue;
                    PipelineRegistersDataGridView[1, row.Index].Value = "-";
                    return;
                }
            }

            // Get instruction
            var currentInstruction = InstructionMemory[PcCurrentVal];
            // Move to next instruction
            PcCurrentVal += 4;
            // Store Instruction in IF/ID
            foreach (DataGridViewRow row in PipelineRegistersDataGridView.Rows)
            {
                if (row.Cells["Register"].Value.ToString() != "IF/ID") continue;
                PipelineRegistersDataGridView[1, row.Index].Value = currentInstruction;
                break;
            }
            PipelineRegistersList.IFID.Enqueue(currentInstruction);
        }

        private void Decode()
        {
            if (PipelineRegistersList.IFID.Count == 0)
            {
                foreach (DataGridViewRow row in PipelineRegistersDataGridView.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "IF/ID") continue;
                    PipelineRegistersDataGridView[1, row.Index].Value = "-";
                    return;
                }
            }

            // Take instruction from IF/ID
            var currentInstruction = PipelineRegistersList.IFID.Dequeue();

            // Remove extra spaces, commas, initial PC
            var instructionFormat = currentInstruction.ToString().Substring(6).Replace(" ", "");
            var instructionOpCode = instructionFormat.Substring(0, 6);
            ControlUnit.SetAluControls(instructionOpCode);
            RegisterFile.ReadRegisterOne = Convert.ToUInt32(instructionFormat.Substring(6, 5), 2);
            RegisterFile.ReadRegisterTwo = Convert.ToUInt32(instructionFormat.Substring(11, 5), 2);

            // Read Data for given Register Addresses
            RegisterFile.ReadData1();
            RegisterFile.ReadData2();

            // Enqueue in ID/EX
            // Store opCode
            PipelineRegistersList.IDEX.Enqueue(instructionOpCode);
            // Store rs, rt
            PipelineRegistersList.IDEX.Enqueue(RegisterFile.ReadDataOne);
            PipelineRegistersList.IDEX.Enqueue(RegisterFile.ReadDataTwo);
            var address = Convert.ToUInt32(instructionFormat.Substring(16, 16), 2);
            // Store Sign Extension
            PipelineRegistersList.IDEX.Enqueue(address);
            // Store rt
            PipelineRegistersList.IDEX.Enqueue(RegisterFile.ReadRegisterTwo);
            // Store rd
            PipelineRegistersList.IDEX.Enqueue(Convert.ToUInt32(instructionFormat.Substring(16, 5), 2));
            var func = instructionFormat.Substring(26, 6);
            var arr = func.ToCharArray();
            Array.Reverse(arr);
            func = new string(arr);
            ALUControlBlock.Function = func;

            // Add to DataGridView
            foreach (DataGridViewRow row in PipelineRegistersDataGridView.Rows)
            {
                if (row.Cells["Register"].Value.ToString() != "ID/EX") continue;
                PipelineRegistersDataGridView[1, row.Index].Value = string.Concat(
                    RegisterFile.ReadDataOne.ToString(), "\n",
                    RegisterFile.ReadDataTwo, "\n", 
                    address.ToString(), "\n", 
                    RegisterFile.ReadRegisterTwo.ToString(), "\n", 
                    Convert.ToUInt32(instructionFormat.Substring(16, 5), 2).ToString());
                break;
            }
            ALUControlBlock.AluOp = ControlUnit.AluOp0 + ControlUnit.AluOp1.ToString();

        }

        private void Execute()
        {
            if (PipelineRegistersList.IDEX.Count == 0)
            {
                foreach (DataGridViewRow row in PipelineRegistersDataGridView.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "ID/EX") continue;
                    PipelineRegistersDataGridView[1, row.Index].Value = "-";
                    return;
                }
            }

            var opCode = PipelineRegistersList.IDEX.Dequeue();
            ControlUnit.SetAluControls(opCode.ToString());
            var readDataOne = PipelineRegistersList.IDEX.Dequeue();
            var readDataTwo = PipelineRegistersList.IDEX.Dequeue();
            var signExtension = PipelineRegistersList.IDEX.Dequeue();
            var rt = PipelineRegistersList.IDEX.Dequeue();
            var rd = PipelineRegistersList.IDEX.Dequeue();

            Muxs.AluSrcMux = new uint[2];
            Muxs.AluSrcMux[0] = uint.Parse(readDataTwo.ToString());
            Muxs.AluSrcMux[1] = uint.Parse(signExtension.ToString());

            var aluValueTwo = Muxs.GetAluSrcMuxVal(ControlUnit.AluSrc);

            ALUControlBlock.ComputeOperation();

            // Perform ALU Operation
            ALU.ValueOne = (uint)readDataOne;
            ALU.ValueTwo = aluValueTwo;
            ALU.Operation = ALUControlBlock.Operation;
            ALU.ComputeResult();

            Muxs.RegDstMux = new uint[2];
            Muxs.RegDstMux[0] = (uint)rt;
            Muxs.RegDstMux[1] = (uint)rd;

            // Enqueue in EX/MEM
            PipelineRegistersList.EXMEM.Enqueue(opCode);
            PipelineRegistersList.EXMEM.Enqueue(ALU.Result);
            PipelineRegistersList.EXMEM.Enqueue(readDataTwo);
            PipelineRegistersList.EXMEM.Enqueue(Muxs.GetRegDstMuxVal(ControlUnit.RegDst));

            // Add to Pipeline DataGridView
            foreach (DataGridViewRow row in PipelineRegistersDataGridView.Rows)
            {
                if (row.Cells["Register"].Value.ToString() != "EX/MEM") continue;
                PipelineRegistersDataGridView[1, row.Index].Value = string.Concat(
                    ALU.Result.ToString(), "\n",
                    readDataTwo.ToString(), "\n", 
                    Muxs.GetRegDstMuxVal(ControlUnit.RegDst).ToString());
                break;
            }

        }

        private void Memory()
        {
            if (PipelineRegistersList.EXMEM.Count == 0)
            {
                foreach (DataGridViewRow row in PipelineRegistersDataGridView.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "EX/MEM") continue;
                    PipelineRegistersDataGridView[1, row.Index].Value = "-";
                    return;
                }
            }

            var opCode = PipelineRegistersList.EXMEM.Dequeue();
            ControlUnit.SetAluControls(opCode.ToString());
            DataMemoryList.Address = uint.Parse(PipelineRegistersList.EXMEM.Dequeue().ToString());
            DataMemoryList.WriteData = uint.Parse(PipelineRegistersList.EXMEM.Dequeue().ToString());

            // Perform ReadData / WriteData
            if (Convert.ToBoolean(ControlUnit.MemRead))
                DataMemoryList.ComputeReadData();
            else if (Convert.ToBoolean(ControlUnit.MemWrite))
                DataMemoryList.ComputeWriteData();

            // Enqueue in MEM/WB
            PipelineRegistersList.MEMWB.Enqueue(opCode);
            PipelineRegistersList.MEMWB.Enqueue(DataMemoryList.ReadData);
            PipelineRegistersList.MEMWB.Enqueue(DataMemoryList.Address);
            var variable = PipelineRegistersList.EXMEM.Dequeue();
            PipelineRegistersList.MEMWB.Enqueue(variable);

            // Add to DataGridView
            foreach (DataGridViewRow row in PipelineRegistersDataGridView.Rows)
            {
                if (row.Cells["Register"].Value.ToString() != "MEM/WB") continue;
                PipelineRegistersDataGridView[1, row.Index].Value = string.Concat(
                    DataMemoryList.ReadData.ToString(), "\n",
                    DataMemoryList.Address.ToString(), "\n", 
                    variable.ToString());
                break;
            }

            // Add to DataMemory DataGridView
            if (uint.Parse(DataMemoryList.ReadData.ToString()) != 99 || DataMemoryList.Address <= 31) return;
            DataMemoryDataSource.Rows.Add(ALU.Result, DataMemoryList.ReadData);
            DataMemory[ALU.Result] = DataMemoryList.ReadData;

        }

        private void WriteBack()
        {
            if (PipelineRegistersList.MEMWB.Count == 0)
            {
                foreach (DataGridViewRow row in PipelineRegistersDataGridView.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "MEM/WB") continue;
                    PipelineRegistersDataGridView[1, row.Index].Value = "-";
                    return;
                }
            }

            var opCode = PipelineRegistersList.MEMWB.Dequeue();
            ControlUnit.SetAluControls(opCode.ToString());

            Muxs.MemToRegMux = new uint[2];
            Muxs.MemToRegMux[1] = uint.Parse(PipelineRegistersList.MEMWB.Dequeue().ToString());
            Muxs.MemToRegMux[0] = uint.Parse(PipelineRegistersList.MEMWB.Dequeue().ToString());
            RegisterFile.WriteData = Muxs.GetMemToRegMuxVal(ControlUnit.MemToReg);
            RegisterFile.WriteRegister = uint.Parse(PipelineRegistersList.MEMWB.Dequeue().ToString());

            if (ControlUnit.RegWrite != 1) return;
            {
                RegisterFile.PerformRegisterWrite();
                foreach (DataGridViewRow row in MipsRegistersDataGrid.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != string.Concat("$", RegisterFile.WriteRegister.ToString())) continue;
                    MipsRegistersDataGrid[1, row.Index].Value = RegisterFile.WriteData;
                    var dt = (DataTable)MipsRegistersDataGrid.DataSource;
                    MipsRegisters = GetDict(dt);
                    break;
                }
            }
        }

        private void RunOneCycle_Click(object sender, EventArgs e)
        {
            if (PcCurrentVal == -1)
            {
                MessageBox.Show(@"Please Enter Machine Code & PC Value then click Initialize");
                return;
            }

            ClockCycleValue++;
            WriteBack();
            Memory();
            Execute();
            Decode();
            Fetch();
            PipelineRegistersDataGridView.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            PipelineRegistersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            PipelineRegistersDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void Initialize_Click(object sender, EventArgs e)
        {
            if (userCode.Text == "")
            {
                MessageBox.Show(@"Please Enter Machine Code & PC Value");
                return;
            }

            ClockCycleValue = 0;
            MachineCodeLines = userCode.Text.Split(new[] { "\n" }, StringSplitOptions.None);
            PcCurrentVal = int.Parse(PCVal.Text);
            InitialPcVal = PcCurrentVal;
            foreach (var line in MachineCodeLines)
            {
                InstructionMemory[InitialPcVal] = line;
                InitialPcVal += 4;
            }

            InitialPcVal = PcCurrentVal;
            PipelineRegistersList.IFID = new Queue<object>();
            PipelineRegistersList.MEMWB = new Queue<object>();
            PipelineRegistersList.EXMEM = new Queue<object>();
            PipelineRegistersList.IDEX = new Queue<object>();
            FormDefaultValues();
        }
    }
}
