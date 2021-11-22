using Newtonsoft.Json;
using PROYECTO___YaYacc.YaYacc;
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
                        Grammar objGrammar = new Grammar(path);

                        var CurrentDirectory = Directory.GetCurrentDirectory();
                        int posBinDirectory = CurrentDirectory.IndexOf("bin", 0);
                        string RelativeDirectory = CurrentDirectory.Substring(0, posBinDirectory);

                        string jsonPath = $"{RelativeDirectory}\\grammar.json";
                        string jsonGrammar = JsonConvert.SerializeObject(objGrammar);

                        using (FileStream fs = File.Create(jsonPath))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(jsonGrammar);
                            fs.Write(info, 0, info.Length);
                        }

                        int posConsoleDirectory = CurrentDirectory.IndexOf("PROYECTO - YaYacc", 0);
                        string RelativeCosoleDirectory = CurrentDirectory.Substring(0, posConsoleDirectory) + @"CONSOLA - YaYacc\bin\Debug\CONSOLA - YaYacc.exe";

                        string ruta = RelativeCosoleDirectory;
                        System.Diagnostics.Process.Start(ruta);
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
