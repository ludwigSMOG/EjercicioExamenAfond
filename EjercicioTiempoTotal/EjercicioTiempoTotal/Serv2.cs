using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace EjercicioTiempoTotal
{ 
    partial class Serv2 : ServiceBase
    {
        private Thread servidorHilo;
        private ServFechaHora servidor;

        public Serv2()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: agregar codigo aqui para iniciar el servicio
            servidor = new ServFechaHora();

            servidorHilo = new Thread(servidor.InitServer);
            servidorHilo.IsBackground = true;
            servidorHilo.Start();
        }

        protected override void OnStop()
        {
            // TODO: agregar codigo aqui para realizar cualquier anulacion necesaria para detener el servicio
            if(servidor != null)
            {
                servidor.activo= false;
                servidor.CierreSocket();
                //mi metodo
            }

        }

    }
}
