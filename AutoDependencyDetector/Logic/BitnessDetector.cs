using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Logic
{
    public class BitnessDetector
    {
        public enum Bitness
        {
            Native = 0, X86 = 0x014c, Itanium = 0x0200, X64 = 0x8664
        }

        /// <summary>
        /// Taken from https://stackoverflow.com/a/885481/2416394
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Bitness BitnessOf( string filename )
        {
            const int PE_POINTER_OFFSET = 60;
            const int MACHINE_OFFSET = 4;
            byte[] data = new byte[4096];
            using ( Stream s = new FileStream( filename, FileMode.Open, FileAccess.Read ) )
            {
                s.Read( data, 0, 4096 );
            }
            // dos header is 64 bytes, last element, long (4 bytes) is the address of the PE header
            int PE_HEADER_ADDR = BitConverter.ToInt32(data, PE_POINTER_OFFSET);
            int machineUint = BitConverter.ToUInt16(data, PE_HEADER_ADDR + MACHINE_OFFSET);
            return (Bitness) machineUint;
        }
    }
}
