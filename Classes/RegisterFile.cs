namespace MipsEmulator
{
    static class RegisterFile
    {
        public static uint ReadRegisterOne { get; set; }
        public static uint ReadRegisterTwo { get; set; }
        public static uint WriteRegister { get; set; }
        public static uint WriteData { get; set; }
        public static uint ReadDataOne { get; set; }
        public static uint ReadDataTwo { get; set; }

        public static object ReadData1()
        {
            ReadDataOne = uint.Parse(MipsEmulator.MipsRegisters["$" + ReadRegisterOne].ToString());
            return ReadDataOne;
        }

        public static object ReadData2()
        {
            ReadDataTwo = uint.Parse(MipsEmulator.MipsRegisters["$" + ReadRegisterTwo].ToString());
            return ReadDataTwo;
        }

        public static void PerformRegisterWrite()
        {
            MipsEmulator.MipsRegisters["$" + WriteRegister] = WriteData;
        }
    }
}
