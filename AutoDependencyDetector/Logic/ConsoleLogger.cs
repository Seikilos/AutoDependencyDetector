using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Logic
{
    public class ConsoleLogger
    {

        public void Info( string msg, params object[] args )
        {
            using ( new ConsoleColorScope( frontColor: ConsoleColor.White ) )
            {
                format( msg, args );
            }
        }


        private void format( string msg, params object[] args )
        {
            Console.WriteLine(string.Format( msg, args ) + Environment.NewLine);
        }


        public class ConsoleColorScope : IDisposable
        {
            public ConsoleColorScope(ConsoleColor backColor = ConsoleColor.Black, ConsoleColor frontColor = ConsoleColor.Gray )
            {
                Console.BackgroundColor = backColor;
                Console.ForegroundColor = frontColor;
            }

            public void Dispose()
            {
                Console.ResetColor();
            }
        }
    }
}
