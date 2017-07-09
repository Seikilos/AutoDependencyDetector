using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoDependencyDetector.Data;
using AutoDependencyDetector.Logic;
using Newtonsoft.Json;

namespace AutoDependencyDetector
{
    class Program
    {
        static void Main( string[] args )
        {
            try
            {

                var options = new Options();
                var res =  CommandLine.Parser.Default.ParseArguments( args,options );

                if ( res == false )
                {
                    Environment.Exit( 1 );
                }

                var p = new ProcessPipeline();
                p.ExecutePipeline( options );

            }
            catch ( Exception e )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( e.ToString() );
                Environment.Exit( 1 );
            }
        }


       
    }
}
