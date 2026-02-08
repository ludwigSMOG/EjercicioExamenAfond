using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EjercicioTiempoTotal
{
    public partial class Form1 : Form
    {
        private string ipServ = "127.0.0.1";
        private int pServ = 5000;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void btnTiempo_Click(object sender, EventArgs e)
        {
            envio("time");
        }

        private async void btnFecha_Click(object sender, EventArgs e)
        {
            envio("date");
        }

        private async void btnTodo_Click(object sender, EventArgs e)
        {
            envio("all");
        }

        private async void btnCierre_Click(object sender, EventArgs e)
        {
            string pass = txtContraseña.Text.Trim();
            if (pass.Length == 0)
            {
                envio("close");

            }
            else
            {

                envio("close " + pass);
            }
        }

        private void envio(string comando)
        {
            Thread hilo = new Thread(() =>
            {
                try
                {
                    IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ipServ), pServ);
                    Socket servidor = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

                    servidor.Connect(ie);

                    NetworkStream ns = new NetworkStream(servidor);
                    StreamReader sr = new StreamReader(ns);
                    StreamWriter sw = new StreamWriter(ns);
      
                    sw.WriteLine(comando);
                    string respuesta = sr.ReadLine();

                    Invoke(new Action(() =>
                    {
                        lblResultado.Text = respuesta;
                    }));

                    servidor.Close();
                }
                catch
                {
                    Invoke(new Action(MostrarErrorConexion));
                }
            });

            hilo.Start();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            using (Configuracion f = new Configuracion(ipServ, pServ))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    ipServ = f.IP;
                    pServ = f.puerto;
                    //puedo coger los valores de las variables del otro
                    //formulario porque heredan del mismo padre
                }
            }
        }

        private void MostrarErrorConexion()
        {
            lblResultado.Text = "No se pudo establecer una conexion";
        }

    }
}
