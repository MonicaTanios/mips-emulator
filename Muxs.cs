namespace MipsEmulator
{
    static class Muxs
    {
        public static uint[] RegDstMux;
        public static uint[] AluSrcMux;
        public static uint[] MemToRegMux;

        public static uint GetRegDstMuxVal(int regDst)
        {
            return RegDstMux[regDst];
        }

        public static uint GetAluSrcMuxVal(int aluSrc)
        {
            return AluSrcMux[aluSrc];
        }

        public static uint GetMemToRegMuxVal(int memToReg)
        {
            return MemToRegMux[memToReg];
        }
    }
}