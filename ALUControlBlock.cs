using System;

namespace MipsEmulator
{
    static class ALUControlBlock
    {
        public static string AluOp { get; set; }
        public static string Function { get; set; }
        public static string Operation { get; set; }

        public static void ComputeOperation()
        {
            Operation = "";
            //If lw -> add
            if (Function.Equals("000011"))
            {
                Operation = "010";
            }
            //If R-Type -> add/sub/or/and/..
            else
            {

                //((ALUOp1 & F1) | ALUOp0)
                Operation += ((Convert.ToUInt32(AluOp[1].ToString(), 2) & Convert.ToUInt32(Function[1].ToString(), 2)) | Convert.ToUInt32(AluOp[0].ToString(), 2)).ToString();

                //(!F2 | !ALUOp1)
                Operation += ((1 - Convert.ToUInt32(Function[2].ToString())) | (1 - Convert.ToUInt32(AluOp[1].ToString()))).ToString();

                //((F0 | F3) & ALUOp1)
                Operation += ((Convert.ToUInt32(Function[0].ToString(), 2) | Convert.ToUInt32(Function[3].ToString(), 2)) & Convert.ToUInt32(AluOp[1].ToString(), 2)).ToString();

            }

        }
    }
}