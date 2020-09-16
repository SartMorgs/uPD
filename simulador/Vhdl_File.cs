using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uPD
{
    public partial class Vhdl_File : Form
    {
        #region Variables
        string type;
        #endregion Variables

        public Vhdl_File()
        {
            InitializeComponent();
        }

        private void Vhdl_File_Load(object sender, EventArgs e)
        {
            type = Program.vhdl;
            switch (type)
            {
                case "ram":
                    txt_Vhdl.Text = Componentes_VHDL.memoria;
                    break;
                case "rom":
                    txt_Vhdl.Text = Componentes_VHDL.rom;
                    break;
                case "ula":
                    txt_Vhdl.Text = Componentes_VHDL.ula;
                    break;
                case "bco":
                    txt_Vhdl.Text = Componentes_VHDL.banco_registradores;
                    break;
                case "lifo":
                    txt_Vhdl.Text = Componentes_VHDL.lifo;
                    break;
                case "controller":
                    txt_Vhdl.Text = Componentes_VHDL.controle;
                    break;
                default:
                    break;
            }
        }
    }
}
