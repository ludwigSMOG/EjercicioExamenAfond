using System;
using System.ServiceProcess;

namespace EjercicioTiempoTotal
{
    internal static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EjercicioTiempoTotal()
            };
            ServiceBase.Run(ServicesToRun);
        }

        
    }
}


