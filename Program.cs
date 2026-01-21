using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PRG281_Project
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Runs the Initial Load Code
            EnumLibrary RunProgram = new EnumLibrary();
            RunProgram.RunLogin();
        }
    }
}
