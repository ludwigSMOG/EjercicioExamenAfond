using System;
using System.ServiceProcess;

namespace EjercicioTiempoTotal
{
    internal static class Program
    {

        static void Main(string[] args)
        {
            //ServFechaHora s = new ServFechaHora();
            //s.InitServer();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Serv2()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}


