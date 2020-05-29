using System.Collections.Generic;

namespace MipsEmulator
{
    static class PipelineRegistersList
    {
        public static Queue<object> IFID { get; set; }
        public static Queue<object> IDEX { get; set; }
        public static Queue<object> EXMEM { get; set; }
        public static Queue<object> MEMWB { get; set; }
    }
}
