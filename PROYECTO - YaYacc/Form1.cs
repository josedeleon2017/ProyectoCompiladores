using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            Parser p = new Parser();

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
            lblLog.Visible = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
