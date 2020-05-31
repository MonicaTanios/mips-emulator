using System;

namespace MipsEmulator
{
    static class ControlUnit
    {
        public static int RegDst { get; set; }
        public static int AluSrc { get; set; }
        public static int MemToReg { get; set; }
        public static int RegWrite { get; set; }
        public static int MemRead { get; set; }
        public static int MemWrite { get; set; }
        public static int Branch { get; set; }
        public static int AluOp0 { get; set; }
        public static int AluOp1 { get; set; }

        public static void SetAluControls(string opCode)
        {
            var arr = opCode.ToCharArray();
            Array.Reverse(arr);
            opCode = new string(arr);

            var rFormat = char.Parse(((1 - Convert.ToUInt32(opCode[0].ToString())) &
                                          (1 - Convert.ToUInt32(opCode[1].ToString())) &
                                          (1 - Convert.ToUInt32(opCode[2].ToString())) &
                                          (1 - Convert.ToUInt32(opCode[3].ToString())) &
                                          (1 - Convert.ToUInt32(opCode[4].ToString())) &
                                          (1 - Convert.ToUInt32(opCode[5].ToString()))).ToString());

            var lw = char.Parse((Convert.ToUInt32(opCode[0].ToString()) &
                                     Convert.ToUInt32(opCode[1].ToString()) &
                                     (1 - Convert.ToUInt32(opCode[2].ToString())) &
                                     (1 - Convert.ToUInt32(opCode[3].ToString())) &
                                     (1 - Convert.ToUInt32(opCode[4].ToString())) &
                                     Convert.ToUInt32(opCode[5].ToString())).ToString());

            var sw = char.Parse((Convert.ToUInt32(opCode[0].ToString()) &
                                     Convert.ToUInt32(opCode[1].ToString()) &
                                     (1 - Convert.ToUInt32(opCode[2].ToString())) &
                                     Convert.ToUInt32(opCode[3].ToString()) &
                                     (1 - Convert.ToUInt32(opCode[4].ToString())) &
                                     Convert.ToUInt32(opCode[5].ToString())).ToString());

            var beq = char.Parse(((1 - Convert.ToUInt32(opCode[0].ToString())) &
                                      (1 - Convert.ToUInt32(opCode[1].ToString())) &
                                      Convert.ToUInt32(opCode[2].ToString()) &
                                      (1 - Convert.ToUInt32(opCode[3].ToString())) &
                                      (1 - Convert.ToUInt32(opCode[4].ToString())) &
                                      (1 - Convert.ToUInt32(opCode[5].ToString()))).ToString());

            RegDst = int.Parse(rFormat.ToString());
            AluSrc = int.Parse(((char)(lw | sw)).ToString());
            MemToReg = int.Parse(lw.ToString());
            RegWrite = int.Parse(((char)(rFormat | lw)).ToString());
            MemRead = int.Parse(lw.ToString());
            MemWrite = int.Parse(sw.ToString());
            Branch = int.Parse(beq.ToString());
            AluOp1 = int.Parse(rFormat.ToString());
            AluOp0 = int.Parse(beq.ToString());
        }

    }
}