using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace MiPrimerServicio
{
    public partial class MiPrimerServicio : ServiceBase
    {
        public MiPrimerServicio()
        {
            InitializeComponent();
        }

        public void WriteEvent(string mensaje)
        {
            const string nombre = "MiPrimerServicio";
            EventLog.WriteEntry(nombre, mensaje);
        }

        private System.Timers.Timer timer;
        protected override void OnStart(string[] args)
        {
            WriteEvent("Iniciando MiPrimerServicio mediante OnStart");

            timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += TimerTick;
            timer.Start();
        }
        private int t = 0;
        public void TimerTick(object sender, System.Timers.ElapsedEventArgs args)
        {
            WriteEvent($"MiPrimerServicio lleva ejecutándose {t} segundos.");
            t += 10;
        }
        protected override void OnStop()
        {
            WriteEvent("Deteniendo MiPrimerServicio");

            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }

            t = 0;
        }

        protected override void OnPause()
        {
            WriteEvent("MiPrimerServicio en Pausa");
            timer?.Stop();
            //en caso de null, llamo a la funcion de stop para que mi programa pare y no continue

        }
        protected override void OnContinue()
        {
            WriteEvent("Continuando MiPrimerServicio");
            timer?.Start();
            //igual que arriba solo que en este caso es cuando empieza
        }
    }
}
