namespace MipsEmulator
{
    static class DataMemoryList
    {
        public static uint Address { get; set; }
        public static uint WriteData { get; set; }
        public static int MemWrite { get; set; }
        public static int MemRead { get; set; }
        public static uint ReadData { get; set; }

        public static void ComputeReadData()
        {
            MemWrite = ControlUnit.MemWrite;
            MemRead = ControlUnit.MemRead;
            ReadData = (Address < 32)
                ? uint.Parse(MipsEmulator.MipsRegisters[string.Concat("$", Address.ToString())].ToString())
                : 99;
        }

        public static void ComputeWriteData()
        {
            MipsEmulator.MipsRegisters[string.Concat("$", Address.ToString())] = WriteData;
        }
    }
}
