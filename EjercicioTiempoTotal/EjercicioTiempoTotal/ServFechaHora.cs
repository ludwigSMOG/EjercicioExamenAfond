using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace EjercicioTiempoTotal
{
    internal class ServFechaHora
    {
        public bool activo = true;
        public int pDef = 5000;

        public Socket escucha;

        private const string fuente = "ServicioFechaHora";
        private const string destino = "Application";

        public void InitServer()
        {
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, pDef);

            escucha = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            escucha.Bind(ie);
            escucha.Listen(10);

            Console.WriteLine("Se inicio el servidor en el puerto: " + pDef);

            while (activo)
            {
                try
                {
                    Socket client = escucha.Accept();
                    Thread t = new Thread(() => AtencionCliente(client));
                    t.Start();
                }
                catch
                {
                    activo = false;
                }

            }
        }

        private void AtencionCliente(Socket cliente)
        {
            using (cliente)
            {
                IPEndPoint ep = (IPEndPoint)cliente.RemoteEndPoint;

                using (NetworkStream ns = new NetworkStream(cliente))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    //sw.AutoFlush = true;

                    string msg = sr.ReadLine();
                    if (msg == null)
                    {
                        return;
                    }
                    string response = comandos(msg);

                    sw.WriteLine(response);
                }
            }
        }

        private string comandos(string msg)
        {
            string cmd = msg.Trim().ToLower();

            if (cmd == "time")
            {

                return DateTime.Now.ToString("HH:mm:ss");
            }

            if (cmd == "date")
            {
                return DateTime.Now.ToString("dd/MM/yyyy");
            }
            if (cmd == "all")
            {

                return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }

            if (cmd.StartsWith("close"))
            {
                string[] parts = cmd.Split(' ');

                if (parts.Length == 1)
                {

                    return "Falto poner la constraseña";
                }

                string passCliente = parts[1];
                string passFichero = Clave();

                if (passFichero == null)
                {

                    return "No se pudo leer la contraseña";
                }
                if (passCliente == passFichero)
                {
                    activo = false;
                    try
                    {
                        escucha.Close();
                    }
                    catch
                    {
                    }

                    return "Se cerro el servidor";
                }

                return "La constraseña fue incorrecta";
            }

            return "El comando fallo";
        }

        private string Clave()
        {
            try
            {
                string ruta = Path.Combine(
                    Environment.GetEnvironmentVariable("PROGRAMDATA"),
                    "contraseña.txt");

                if (!File.Exists(ruta))
                    return null;

                return File.ReadAllText(ruta).Trim();
            }
            catch
            {
                return null;
            }
        }

        public void StopServer()
        {
            activo = false;
            try
            {
                escucha.Close();
            }
            catch
            {
            }

        }

        public void CierreSocket()
        {
            try
            {
                if (escucha != null)
                {
                    escucha.Close();
                }
            }
            catch
            {

            }
        }

        //private int leerPuertoConfig()
        //{
        //    string ruta = Path.Combine(
        //        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        //        "pServ.txt");

        //    try
        //    {
        //        if (!File.Exists(ruta))
        //            throw new FileNotFoundException();

        //        string contenido = File.ReadAllText(ruta).Trim();
        //        if (int.TryParse(contenido, out int puerto))
        //            return puerto;

        //        throw new FormatException();
        //    }
        //    catch (Exception ex)
        //    {
        //        escritura("No se pudieron leer los archivos del pueto configuracion: " + ex.Message,
        //                       EventLogEntryType.Error);
        //        return pDef;
        //    }
        //}

        //private int escucharPuertos(int pConfig, int pDef)
        //{
        //    foreach (int p in new int[] { pConfig, pDef })
        //    {
        //        //int[] nuevo array donde irá del rango de pConfig a pDef
        //        try
        //        {
        //            IPEndPoint ie = new IPEndPoint(IPAddress.Any, p);
        //            using (Socket prueba = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        //            {
        //                prueba.Bind(ie);
        //            }
        //            return p;
        //        }
        //        catch (SocketException)
        //        {
        //            //vacio porque en caso de que de error porque el socket esta ocupado, tiene que probar en otro
        //        }
        //    }
        //    return -1;
        //}


        public void escritura(string mensaje, EventLogEntryType tipo)
        {
            try
            {
                EventLog.WriteEntry(fuente, mensaje, tipo);
            }
            catch
            {
                consola("El error fue: " + mensaje);
            }
        }

        public string RutaLog => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                 "servFechaHora.log");

        public static void consola(string mensaje)
        {
            ServFechaHora sfh = new ServFechaHora();
            string linea = $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {mensaje}";
            try
            {
                File.AppendAllText(sfh.RutaLog, linea + Environment.NewLine);
            }
            catch
            {
                // aqui nada porque si no entrariamos en bucles y queremos que finalice sin hacer nada 
            }
        }
    }
}
