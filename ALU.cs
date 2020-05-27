namespace MipsEmulator
{
    static class ALU
    {
        public static uint ValueOne { get; set; }
        public static uint ValueTwo { get; set; }
        public static uint Result { get; set; }
        public static string Operation { get; set; }

        public static void ComputeResult()
        {
            if (Operation.Equals("010"))
                Result = ValueOne + ValueTwo;
            else if (Operation.Equals("110"))
                Result = ValueOne - ValueOne;
            else if (Operation.Equals("000"))
                Result = ValueOne & ValueTwo;
            else
                Result = ValueOne | ValueTwo;
        }

    }
}
