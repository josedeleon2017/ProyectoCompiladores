using PROYECTO___YaYacc.YaYacc;
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
    public partial class Input : Form
    {
        public Grammar grammar;
        public LALR parser;
        public Input(Grammar gInput)
        {
            InitializeComponent();
            grammar = gInput;

            parser = new LALR(grammar);
            parser.GenerateTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string exp = txtInput.Text;
            string[] t = exp.Split(' ');

            Queue<string> tokens = new Queue<string>();
            for (int i = 0; i < t.Length; i++)
            {
                tokens.Enqueue(t[i]);
            }
            tokens.Enqueue("$");
            bool result = parser.ValidateExpression(tokens);
            if (result)
            {
                lblResult2.Visible = true;
                lblResult2.ForeColor = Color.Green;
                lblResult2.Text = "EXPRESIÓN VÁLIDA";
            }
            else
            {
                lblResult2.Visible = true;
                lblResult2.ForeColor = Color.Red;
                lblResult2.Text = "EXPRESIÓN INVÁLIDA";
            }
            lblRuta.Visible = true;
            lblLog.Visible = true;
            
        }
    }
}
