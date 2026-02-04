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
    public partial class EjercicioTiempoTotal : ServiceBase
    {
        private const string EVENT_SOURCE = "EjercicioTiempoTotal";
        private const int DEFAULT_PORT = 5000;

        private bool servidorActivo;
        private Thread hiloServidor;

        private string configPath;
        private string logPath;

        public EjercicioTiempoTotal()
        {
            InitializeComponent();

            CanPauseAndContinue = false;
            AutoLog = false;

            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            configPath = Path.Combine(programData, "EjercicioTiempoTotal", "config.txt");
            logPath = Path.Combine(programData, "EjercicioTiempoTotal", "server.log");
        }

        protected override void OnStart(string[] args)
        {
            SafeWriteEvent("Servicio iniciando...");

            Directory.CreateDirectory(Path.GetDirectoryName(configPath));

            int port = ReadPortOrDefault(DEFAULT_PORT);

            servidorActivo = true;
            hiloServidor = new Thread(() => RunServer(port));
            hiloServidor.Start();
        }

        protected override void OnStop()
        {
            SafeWriteEvent("Servicio deteniendose...");
            servidorActivo = false;

            try
            {
                // Para ayudar a salir del AcceptTcpClient
                hiloServidor?.Join(2000);
            }
            catch { }

            SafeWriteEvent("Servicio detenido.");
        }

        private int ReadPortOrDefault(int defaultPort)
        {
            try
            {
                if (!File.Exists(configPath))
                {
                    SafeWriteEvent("No existe config, usando puerto por defecto " + defaultPort);
                    return defaultPort;
                }

                string text = File.ReadAllText(configPath).Trim();

                int port;
                if (int.TryParse(text, out port) && port >= 1 && port <= 65535)
                {
                    SafeWriteEvent("Puerto leido desde config: " + port);
                    return port;
                }

                SafeWriteEvent("Puerto config invalido, usando por defecto " + defaultPort);
                return defaultPort;
            }
            catch (Exception ex)
            {
                SafeWriteEvent("Error al leer archivo de config: " + ex.Message);
                return defaultPort;
            }
        }

        private void RunServer(int portFromConfig)
        {
            TcpListener listener = null;

            if (!TryStartListener(portFromConfig, out listener))
            {
                SafeWriteEvent("Puerto " + portFromConfig + " ocupado. Probando " + DEFAULT_PORT + "...");

                if (!TryStartListener(DEFAULT_PORT, out listener))
                {
                    SafeWriteEvent("Puerto " + portFromConfig + " y " + DEFAULT_PORT + " ocupados. Cerrando servicio.");
                    return;
                }
            }

            SafeWriteEvent("Servidor escuchando en el puerto: " + ((IPEndPoint)listener.LocalEndpoint).Port);

            try
            {
                while (servidorActivo)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    HandleClient(client);
                }
            }
            catch (Exception ex)
            {
                SafeWriteEvent("Error servidor: " + ex.Message);
            }
            finally
            {
                try { listener.Stop(); } catch { }
            }
        }

        private bool TryStartListener(int port, out TcpListener listener)
        {
            listener = null;

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                return true;
            }
            catch
            {
                try { listener?.Stop(); } catch { }
                return false;
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                IPEndPoint remote = client.Client.RemoteEndPoint as IPEndPoint;
                string ip = remote.Address.ToString();
                int remotePort = remote.Port;

                using (client)
                using (NetworkStream ns = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytes = ns.Read(buffer, 0, buffer.Length);
                    string cmd = Encoding.UTF8.GetString(buffer, 0, bytes).Trim();

                    WriteToFileLog("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-@" + ip + ":" + remotePort + "] " + cmd);

                    if (cmd.Equals("close", StringComparison.OrdinalIgnoreCase))
                    {
                        SafeWriteEvent("Comando no valido: " + cmd);
                        Send(ns, "Comando no valido");
                        return;
                    }

                    if (cmd.Equals("TIME", StringComparison.OrdinalIgnoreCase))
                        Send(ns, DateTime.Now.ToString("HH:mm:ss"));
                    else if (cmd.Equals("DATE", StringComparison.OrdinalIgnoreCase))
                        Send(ns, DateTime.Now.ToString("yyyy-MM-dd"));
                    else if (cmd.Equals("DATETIME", StringComparison.OrdinalIgnoreCase))
                        Send(ns, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    else
                    {
                        SafeWriteEvent("Comando no valido: " + cmd);
                        Send(ns, "Comando no valido");
                    }
                }
            }
            catch (Exception ex)
            {
                SafeWriteEvent("Error cliente: " + ex.Message);
            }
        }

        private void Send(NetworkStream ns, string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            ns.Write(data, 0, data.Length);
        }

        private void SafeWriteEvent(string msg)
        {
            try
            {
                EventLog.WriteEntry(EVENT_SOURCE, msg);
            }
            catch (Exception ex)
            {
                WriteToFileLog("[ERROR] [" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + ex.Message + " | MSG: " + msg);
            }
        }

        private void WriteToFileLog(string line)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, line + Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}
