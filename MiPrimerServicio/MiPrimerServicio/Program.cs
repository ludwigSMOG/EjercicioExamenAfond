using MiPrimerServicio;
using System.ServiceProcess;

namespace MiPrimerServicio
{
    static class Program
    {
        static void Main()
        {
            ServiceBase.Run(new MiPrimerServicio());
        }
    }
}
