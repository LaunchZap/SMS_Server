using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Data.Odbc;
using MySql.Data.MySqlClient;

namespace APISMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {                       
            InitializeComponent();
            findPorts();           
        }

        //public SerialPort serialPort;       

        private void findPorts()
	    {
			// get port names
		    //array<Object^>^ objectArray = SerialPort.GetPortNames();
			// add string array to combobox
			//this->comboBox1->Items->AddRange( objectArray );
            //comboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //AutoCompleteStringCollection Ports = new AutoCompleteStringCollection();
            //Ports.Add(serialPort1.PortName);
            //comboBox1.AutoCompleteCustomSource = Ports;
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            
		}

        private void bOpen_Click(object sender, EventArgs e)
        {
            Object selectedItem = comboBox1.SelectedItem;
            if (selectedItem != null)
            {
                //serialPort = new SerialPort(selectedItem.ToString(), 9600, Parity.None, 8, StopBits.One);

                if (!serialPort1.IsOpen && selectedItem != null)
                {
                    //textBox1.AppendText("Conectando\n");

                    try
                    {
                        serialPort1.PortName = selectedItem.ToString();
                        serialPort1.BaudRate = 19200;// 19200;
                        serialPort1.Parity = Parity.None;
                        serialPort1.DataBits = 8;
                        serialPort1.StopBits = StopBits.One;
                        serialPort1.Open();
                        Estado.Text = "Open";
                        comboBox1.Enabled = false;
                        bOpen.Text = "Close";
                        this.serialPort1.DataReceived += new SerialDataReceivedEventHandler(this.sport_DataReceived);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.ToString(), "Error"); }
                }
                else if (serialPort1.IsOpen && selectedItem != null)
                {
                    //textBox1.AppendText("Cerrado\n");
                    serialPort1.Close();
                    Estado.Text = "Close";
                    comboBox1.Enabled = true;
                    bOpen.Text = "Open";
                }
            }
                   
        }

        private void bSend_Click(object sender, EventArgs e)
        {   
            /******** Pendiente el envío de SMS desde la implementacion  ********/
            if (serialPort1.IsOpen) serialPort1.Write(textBox2.Text + "\r");
            //this.SetText("\r\n +CMT: \"3113047400\",,\"14/10/19,10:01:53-20\"\r\nHello world!!! \r\n");            
        }

        private void sport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            String dtn = dt.ToShortTimeString();
            string texto = serialPort1.ReadExisting();
            //textBox1.AppendText("[" + dtn + "] " + "Received: " + texto + "\n");
            //this.textBox1.AppendText(texto.ToString());
            this.SetText(texto);            
        }

        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (text == "")
                    this.textBox1.Clear();
                else
                    this.textBox1.AppendText(text);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Estado.Text = "cambio";
            int indice;
            string Buf = textBox1.Text;
            string Origen="";
            string timeStamp = "";
            string Msg = "";

            char[] value = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
            char[] fecha = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
            char[] sms = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };

            if (Buf.IndexOf("\r", 0) == 0 && Buf.IndexOf("\r", 1) > 0 && Buf.IndexOf("\r", 46) > 44)
            {
                //Estado.Text = "Ok";
                indice = Buf.IndexOf("\"", 0);

                //Extrae el numero celular
                Buf.CopyTo(indice+1, value, 0, 10);                
                for (var index = 0; index < 10; index++ )
                {
                    Origen += value[index];
                }

                indice = Buf.IndexOf("\"", indice+1);

                //Extrae la Fecha y hora
                indice = Buf.IndexOf("\"", indice + 1);

                Buf.CopyTo(indice + 1, fecha, 0, 17);
                for (var index = 0; index < 17; index++)
                {                    
                    if (fecha[index].Equals('/'))
                        timeStamp += "-";
                    else if (fecha[index].Equals(','))
                        timeStamp += " ";
                    else
                        timeStamp += fecha[index];
                }

                indice = Buf.IndexOf("\"", indice + 1);
                
                //Extrae el mensaje
                indice = Buf.IndexOf("\n", indice + 1);

                Buf.CopyTo(indice + 1, sms, 0, 14);
                for (var index = 0; index < 14; index++)
                {
                    Msg += sms[index];
                }

                //MessageBox.Show("Origen: " + Origen + "\nRecivido: " + timeStamp + "\nMensaje: " + Msg, "Contenido SMS");
                this.SetText("");
                //Inserta SMS en la DB
                    Mysql(Origen, timeStamp, Msg);
            }
            
        }

        private void prueba()
        {
            string Buf = "\r\n +CMT: \"3113047400\",,\"14/10/19,10:01:53-20\"\r\nHello world!!! \r\n";
            char[] value = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };

            Estado.Text = Buf.IndexOf("\r", 0) + " " + Buf.IndexOf("\r", 1) + " " + Buf.IndexOf("\r", 46);

            if (Buf.IndexOf("\r", 0) == 0 && Buf.IndexOf("\r", 1) > 0 && Buf.IndexOf("\r", 46) > 44)
            {
                //Estado.Text = "Ok";
                Estado.Text = Buf.IndexOf("\"", Buf.IndexOf("\"", 0) + 1) + " " + " ";
                Buf.CopyTo(Buf.IndexOf("\"", 0)+1, value, 0, 10);
                value.ToString();
            }
        }

       private void Mysql( string Origen, string timeStamp , string Msg)
        {
            try
            {
                MySqlStatus.Text = "Ready";
                //Connection string for Connector/ODBC 3.51
                string MyConString = "Server=localhost;" +
                  "Database=test;" +
                  "Port=3306;" +
                  "uid=designer;" +
                  "password=test2014";
                
                string Insert = "insert into sms(Origen, timeStamp, Msg) value (" + Origen + ",'" + timeStamp + "',\"" + Msg + "\");";
                MySqlConnection MyConnection = new MySqlConnection(MyConString);
                MySqlDataAdapter myData = new MySqlDataAdapter();
                              
                MySqlCommandBuilder cb = new MySqlCommandBuilder(myData);
                MySqlDataReader myRd;                

                //Inserta
                MyConnection.Open();
                MySqlStatus.Text = "Connect";
                myData.SelectCommand = new MySqlCommand(Insert, MyConnection);
                myRd = myData.SelectCommand.ExecuteReader();
                while (myRd.Read()) { }
                MyConnection.Close();
                MySqlStatus.Text = "Close";
                
                //MessageBox.Show("\n !!! success, connected successfully !!!\n", "success");
                
            }
            catch (Exception MyOdbcException) //Catch any ODBC exception ..
            {
                MySqlStatus.Text = "Error";
                MessageBox.Show(MyOdbcException.Message, "ERROR");

            }

        }


    }
}
