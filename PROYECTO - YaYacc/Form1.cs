using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTO___YaYacc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Gramática YaYacc | *.y";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                String path = dialog.FileName; 
                var str = File.ReadAllText(path);
                try
                {
                    DialogResult result = MessageBox.Show("Archivo cargado exitosamente!!!", "Resultado archivo");

                    Parser p = new Parser(str);

                    if (p.ValidateExpression())
                    {
                        lblResult.Visible = true;
                        lblResult.ForeColor = Color.Green;
                        lblResult.Text = "GRAMÁTICA VÁLIDA";
                    }
                    else
                    {
                        lblResult.Visible = true;
                        lblResult.ForeColor = Color.Red;
                        lblResult.Text = "GRAMÁTICA INVÁLIDA";
                    }
                    lblRuta.Visible = true;
                    lblLog.Visible = true;

                }
                catch
                {
                    DialogResult result = MessageBox.Show("Ha ocurrido un error, compruebe el formato del archivo", "Error");
                }

            }

            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
