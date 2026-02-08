using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EjercicioTiempoTotal
{
    public partial class Configuracion : Form
    {
        public string IP;
        public int puerto;

        public Configuracion()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }


        public Configuracion(string ip, int port)
        {
            InitializeComponent();
            txtIP.Text = ip;
            txtPuerto.Text = port.ToString();
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            IP = txtIP.Text.Trim();
            puerto = Convert.ToInt32(txtPuerto.Text.Trim());
            DialogResult = DialogResult.OK;
        }
    }
}

