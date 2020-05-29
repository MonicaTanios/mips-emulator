using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MipsEmulator
{
    public partial class MipsEmulator : Form
    {
        public static Dictionary<string, object> MipsRegisters = new Dictionary<string, object>();
        public static Hashtable InstructionMemory = new Hashtable();
        public static DataTable PipelineRegistersDataSource;
        public static DataTable DataMemoryDataSource;
        public string[] MachineCodeLines;
        public int ClockCycleValue;
        public int PcCurrentVal;
        public int InitialPcVal;
        public MipsEmulator()
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

        private void Fetch()
        {
            if (PcCurrentVal >= (InitialPcVal + MachineCodeLines.Length * 4))
            {
                foreach (DataGridViewRow row in PipelineRegisters.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "IF/ID") continue;
                    PipelineRegisters[1, row.Index].Value = "-";
                    return;
                }
            }

            //Get instruction
            var currentInstruction = InstructionMemory[PcCurrentVal];
            //Move to next instruction
            PcCurrentVal += 4;
            //Store Instruction in IF/ID
            foreach (DataGridViewRow row in PipelineRegisters.Rows)
            {
                if (row.Cells["Register"].Value.ToString() != "IF/ID") continue;
                PipelineRegisters[1, row.Index].Value = currentInstruction;
                break;
            }
            PipelineRegistersList.IFID.Enqueue(currentInstruction);
        }

        private void Decode()
        {
            if (PipelineRegistersList.IFID.Count == 0)
            {
                foreach (DataGridViewRow row in PipelineRegisters.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "IF/ID") continue;
                    PipelineRegisters[1, row.Index].Value = "-";
                    return;
                }
            }

            //Take instruction from IF/ID
            var currentInstruction = PipelineRegistersList.IFID.Dequeue();

            //Remove extra spaces, commas, initial PC
            var instructionFormat = currentInstruction.ToString().Substring(6).Replace(" ", "");
            var instructionOpCode = instructionFormat.Substring(0, 6);
            ControlUnit.SetAluControls(instructionOpCode);
            RegisterFile.ReadRegisterOne = Convert.ToUInt32(instructionFormat.Substring(6, 5), 2);
            RegisterFile.ReadRegisterTwo = Convert.ToUInt32(instructionFormat.Substring(11, 5), 2);

            //Read Data for given Register Addresses
            RegisterFile.ReadData1();
            RegisterFile.ReadData2();

            //Store opCode
            PipelineRegistersList.IDEX.Enqueue(instructionOpCode);
            //Store rs, rt
            PipelineRegistersList.IDEX.Enqueue(RegisterFile.ReadDataOne);
            PipelineRegistersList.IDEX.Enqueue(RegisterFile.ReadDataTwo);
            var address = Convert.ToUInt32(instructionFormat.Substring(16, 16), 2);
            //Store Sign Extension
            PipelineRegistersList.IDEX.Enqueue(address);
            //Store rt
            PipelineRegistersList.IDEX.Enqueue(RegisterFile.ReadRegisterTwo);
            //Store rd
            PipelineRegistersList.IDEX.Enqueue(Convert.ToUInt32(instructionFormat.Substring(16, 5), 2));
            var func = instructionFormat.Substring(26, 6);
            var arr = func.ToCharArray();
            Array.Reverse(arr);
            func = new string(arr);
            ALUControlBlock.Function = func;

            //Add to DataGridView
            foreach (DataGridViewRow row in PipelineRegisters.Rows)
            {
                if (row.Cells["Register"].Value.ToString() != "ID/EX") continue;
                PipelineRegisters[1, row.Index].Value = string.Concat(RegisterFile.ReadDataOne.ToString(), ", ",
                    RegisterFile.ReadDataTwo, ", ", address.ToString(), ", ", RegisterFile.ReadRegisterTwo.ToString(), ", ", Convert.ToUInt32(instructionFormat.Substring(16, 5), 2).ToString());
                break;
            }
            ALUControlBlock.AluOp = ControlUnit.AluOp0 + ControlUnit.AluOp1.ToString();

        }

        private void Execute()
        {
            if (PipelineRegistersList.IDEX.Count == 0)
            {
                foreach (DataGridViewRow row in PipelineRegisters.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "ID/EX") continue;
                    PipelineRegisters[1, row.Index].Value = "-";
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

            ALU.ValueOne = (uint)readDataOne;
            ALU.ValueTwo = aluValueTwo;
            ALU.Operation = ALUControlBlock.Operation;
            ALU.ComputeResult();

            Muxs.RegDstMux = new uint[2];
            Muxs.RegDstMux[0] = (uint)rt;
            Muxs.RegDstMux[1] = (uint)rd;

            PipelineRegistersList.EXMEM.Enqueue(opCode);
            PipelineRegistersList.EXMEM.Enqueue(ALU.Result);
            PipelineRegistersList.EXMEM.Enqueue(readDataTwo);
            PipelineRegistersList.EXMEM.Enqueue(Muxs.GetRegDstMuxVal(ControlUnit.RegDst));

            //Add to Pipeline DataGridView
            foreach (DataGridViewRow row in PipelineRegisters.Rows)
            {
                if (row.Cells["Register"].Value.ToString() != "EX/MEM") continue;
                PipelineRegisters[1, row.Index].Value = string.Concat(ALU.Result.ToString(), ", ",
                    readDataTwo.ToString(), ", ", Muxs.GetRegDstMuxVal(ControlUnit.RegDst).ToString());
                break;
            }

        }

        private void Memory()
        {
            if (PipelineRegistersList.EXMEM.Count == 0)
            {
                foreach (DataGridViewRow row in PipelineRegisters.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "EX/MEM") continue;
                    PipelineRegisters[1, row.Index].Value = "-";
                    return;
                }
            }

            var opCode = PipelineRegistersList.EXMEM.Dequeue();
            ControlUnit.SetAluControls(opCode.ToString());
            DataMemoryList.Address = uint.Parse(PipelineRegistersList.EXMEM.Dequeue().ToString());
            DataMemoryList.WriteData = uint.Parse(PipelineRegistersList.EXMEM.Dequeue().ToString());
            object readData = 0;
            var memWrite = ControlUnit.MemWrite;
            var memRead = ControlUnit.MemRead;

            if (memRead == 1)
            {
                readData = DataMemoryList.ComputeReadData();
            }
            else if (memWrite == 1)
            {
                DataMemoryList.ComputeWriteData();
            }

            PipelineRegistersList.MEMWB.Enqueue(opCode);
            PipelineRegistersList.MEMWB.Enqueue(readData);
            PipelineRegistersList.MEMWB.Enqueue(DataMemoryList.Address);
            var variable = PipelineRegistersList.EXMEM.Dequeue();
            PipelineRegistersList.MEMWB.Enqueue(variable);

            //Add to DataGridView
            foreach (DataGridViewRow row in PipelineRegisters.Rows)
            {
                if (row.Cells["Register"].Value.ToString() != "MEM/WB") continue;
                PipelineRegisters[1, row.Index].Value = string.Concat(readData.ToString(), ", ",
                    DataMemoryList.Address.ToString(), ", ", variable.ToString());
                break;
            }

            //Add to DataMemory DataGridView
            if(uint.Parse(readData.ToString()) == 99)
                DataMemoryDataSource.Rows.Add(ALU.Result, readData);

        }

        private void WriteBack()
        {
            if (PipelineRegistersList.MEMWB.Count == 0)
            {
                foreach (DataGridViewRow row in PipelineRegisters.Rows)
                {
                    if (row.Cells["Register"].Value.ToString() != "MEM/WB") continue;
                    PipelineRegisters[1, row.Index].Value = "-";
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
            foreach (DataGridViewRow row in MipsRegistersDataGrid.Rows)
            {
                if (row.Cells["Register"].Value.ToString() != string.Concat("$", RegisterFile.WriteRegister.ToString())) continue;
                MipsRegistersDataGrid[1, row.Index].Value = RegisterFile.WriteData;
                var dt = (DataTable) MipsRegistersDataGrid.DataSource;
                MipsRegisters = GetDict(dt);
                break;
            }
        }

        private void runOneCycle_Click(object sender, EventArgs e)
        {
            ClockCycleValue++;
            WriteBack();
            Memory();
            Execute();
            Decode();
            Fetch();
        }

        private void initialize_Click(object sender, EventArgs e)
        {
            MachineCodeLines = userCode.Text.Split(
                new[] { "\n" },
                StringSplitOptions.None
            );
            PcCurrentVal = int.Parse(PCVal.Text);
            InitialPcVal = PcCurrentVal;
            var pcValue = int.Parse(PCVal.Text);
            foreach (var line in MachineCodeLines)
            {
                InstructionMemory[pcValue] = line;
                pcValue += 4;
            }
            PipelineRegistersList.IFID = new Queue<object>();
            PipelineRegistersList.MEMWB = new Queue<object>();
            PipelineRegistersList.EXMEM = new Queue<object>();
            PipelineRegistersList.IDEX = new Queue<object>();
            FormDefaultValues();
        }
    }
}
