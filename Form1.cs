using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO.Ports;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Diagnostics;
using UDP;


namespace SerialSplitter
{
    public partial class Form1 : Form
    {
        string SW_Version = "3.3\r";        // =======> Version de software para compatibilidad
        string VHKV, VHMA, VHMS, VAKV, VAMA, VAMS, VEKV, VEMA, VEMS, CIKV, CIMA, CIMS, FMKV, FMMA, FMMS, F1KV, F1MA, F1MS, F2KV, F2MA, F2MS, F3KV, F3MA, F3MS, F4KV, F4MA, F4MS, SerialNumber = "";
        string dataIN1 = "", dataIN2 = "", dataIN3 = "", dataOUT1 = "", dataOUT2 = "", dataOUT3 = "", path, textKVP, textKVN, textmAReal, textRmA, LastER, textSFI, textSRE, textSCC, textSIC, textSUC, textUPW, textHU, textVCC, message;
        string Serial1PortName, Serial1BaudRate, Serial1DataBits, Serial1StopBits, Serial1Parity, Serial2PortName, Serial2BaudRate, Serial2DataBits, Serial2StopBits, Serial2Parity, Serial3PortName, Serial3BaudRate, Serial3DataBits, Serial3StopBits, Serial3Parity;

        readonly string[] mA_Table = new string[8] { "50\r", "100\r", "200\r", "300\r", "400\r", "500\r", "600\r", "700\r" };
        readonly string[] ms_Table = new string[30] { "2\r", "5\r", "8\r", "10\r", "20\r", "30\r", "40\r", "50\r", "60\r", "80\r", "100\r", "120\r", "150\r", "200\r", "250\r", "300\r", "400\r", "500\r", "600\r", "800\r", "1000\r", "1200\r", "1500\r", "2000\r", "2500\r", "3000\r", "3500\r", "4000\r", "4500\r", "5000\r" };
        Boolean ACK = false;
        Boolean NACK = false;
        Boolean AutoON = true;
        Boolean SW_Ready = false;
        Boolean DEBUG = false;
        Boolean DisplayInibit = false;
        Boolean Cine = false;
        public static float VCC = 0.0f;
        int Counter, LOW_Limit, HI_Limit, Cine_LOW_Limit, Cine_HI_Limit;

        float mxs;

        string fileToCopy = "C:\\TechXA\\LogIFDUE.txt";
        string destinationDirectory = "G:\\My Drive\\Logs\\";
        StringBuilder sb = new StringBuilder();
        char LF = (char)10;
        char CR = (char)13;

        System.Windows.Forms.Timer t = null;

        Logger logger = new Logger("C:\\TechXA\\LogIFDUE.txt");    // Ruta del archivo de log

        // Create an isntance of IPC with DRAXA
        SimpleUDP u = new SimpleUDP(45000);
        // ...
        // string s = u.Read();
        // u.Write("Welcome App1");

        public Form1()
        {
            InitializeComponent();
            path = AppDomain.CurrentDomain.BaseDirectory;
            // SetImage(path + "About.png");
            // Create an isntance of XmlTextReader and call Read method to read the file  
            XmlTextReader configReader = new XmlTextReader("C:\\TechXA\\Config_DUE_IF.xml");

            LastER = "";

            try
            {
                configReader.Read();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // If the node has value  
            while (configReader.Read())
            {
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "SerialPort1")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    Serial1PortName = getBetween(s1, "name=", 4);
                    Serial1BaudRate = getBetween(s1, "baudrate=", 5);
                    Serial1DataBits = getBetween(s1, "databits=", 1);
                    Serial1StopBits = getBetween(s1, "stopbits=", 3);
                    Serial1Parity = getBetween(s1, "parity=", 4);
                }
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "SerialPort2")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    Serial2PortName = getBetween(s1, "name=", 4);
                    Serial2BaudRate = getBetween(s1, "baudrate=", 5);
                    Serial2DataBits = getBetween(s1, "databits=", 1);
                    Serial2StopBits = getBetween(s1, "stopbits=", 3);
                    Serial2Parity = getBetween(s1, "parity=", 4);
                }
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "SerialPort3")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    Serial3PortName = getBetween(s1, "name=", 4);
                    Serial3BaudRate = getBetween(s1, "baudrate=", 5);
                    Serial3DataBits = getBetween(s1, "databits=", 1);
                    Serial3StopBits = getBetween(s1, "stopbits=", 3);
                    Serial3Parity = getBetween(s1, "parity=", 4);
                }
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "VascularHead")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    VHKV = getBetween(s1, "Kv=", 3);
                    VHMA = getBetween(s1, "mA=", 3);
                    VHMS = getBetween(s1, "ms=", 3);
                }
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "VascularAbdomen")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    VAKV = getBetween(s1, "Kv=", 3);
                    VAMA = getBetween(s1, "mA=", 3);
                    VAMS = getBetween(s1, "ms=", 3);
                }
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "VascularExtremity")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    VEKV = getBetween(s1, "Kv=", 3);
                    VEMA = getBetween(s1, "mA=", 3);
                    VEMS = getBetween(s1, "ms=", 3);
                }
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "Cine")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    CIKV = getBetween(s1, "Kv=", 3);
                    CIMA = getBetween(s1, "mA=", 3);
                    CIMS = getBetween(s1, "ms=", 3);
                }
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "FLUOROMAP")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    FMKV = getBetween(s1, "Kv=", 3);
                    FMMA = getBetween(s1, "mA=", 2);
                    FMMS = getBetween(s1, "ms=", 2);
                }

                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "FLUORO1")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    F1KV = getBetween(s1, "Kv=", 3);
                    F1MA = getBetween(s1, "mA=", 2);
                    F1MS = getBetween(s1, "ms=", 2);
                }

                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "FLUORO2")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    F2KV = getBetween(s1, "Kv=", 3);
                    F2MA = getBetween(s1, "mA=", 2);
                    F2MS = getBetween(s1, "ms=", 2);
                }

                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "FLUORO3")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    F3KV = getBetween(s1, "Kv=", 3);
                    F3MA = getBetween(s1, "mA=", 2);
                    F3MS = getBetween(s1, "ms=", 2);
                }

                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "FLUORO4")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    F4KV = getBetween(s1, "Kv=", 3);
                    F4MA = getBetween(s1, "mA=", 2);
                    F4MS = getBetween(s1, "ms=", 2);
                }

                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "Power")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    VCC = Convert.ToSingle(getBetween(s1, "VCC=", 5));
                    LOW_Limit = Convert.ToInt32(getBetween(s1, "LOW_Limit=", 3));
                    HI_Limit = Convert.ToInt32(getBetween(s1, "HI_Limit=", 3));
                    Cine_LOW_Limit = Convert.ToInt32(getBetween(s1, "Cine_LOW_Limit=", 3));
                    Cine_HI_Limit = Convert.ToInt32(getBetween(s1, "Cine_HI_Limit=", 3));
                }
            }
            CheckPortsNames();
            this.TopMost = true;
        }

        void CheckPortsNames()
        {
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++)
            {
                if (Serial1PortName == ports[i])
                {
                    OpenSerial1();
                    radioButton1.Checked = true;
                    radioButton1.BackColor = Color.Green;
               }
                if (Serial2PortName == ports[i])
                {
                    OpenSerial2();
                    radioButton2.Checked = true;
                    radioButton2.BackColor = Color.Green;
                }
                if (Serial3PortName == ports[i])
                {
                    OpenSerial3();
                    radioButton3.Checked = true;
                    radioButton3.BackColor = Color.Green;
                }
            }
        }

        public static string getBetween(string strSource, string strStart, int largo)
        {
            if (strSource.Contains(strStart))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = Start + largo;
                return strSource.Substring((Start + 1), End - Start);
            }

            return "";
        }

        public async void OpenSerial1()     // Serial Port para la comunicacion con el Software Vieworks
        {
            serialPort1.PortName = Serial1PortName;
            serialPort1.BaudRate = int.Parse(Serial1BaudRate);  // 115200  Valid values are 110, 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, or 115200.
            serialPort1.DataBits = int.Parse(Serial1DataBits);
            serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), Serial1StopBits);
            serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), Serial1Parity);
            serialPort1.Encoding = Encoding.GetEncoding("iso-8859-1");
            // Encoding = Encoding.GetEncoding("Windows-1252");
            serialPort1.Open();
            serialPort1.DtrEnable = false;
            await Task.Delay(50);
            serialPort1.DtrEnable = true;
            await Task.Delay(100);
        }

        public async void OpenSerial2()     // Serial Port para la comunicacion con el Generador
        {
            serialPort2.PortName = Serial2PortName;
            serialPort2.BaudRate = int.Parse(Serial2BaudRate);  // 115200  Valid values are 110, 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, or 115200.
            serialPort2.DataBits = int.Parse(Serial2DataBits);
            serialPort2.StopBits = (StopBits)Enum.Parse(typeof(StopBits), Serial2StopBits);
            serialPort2.Parity = (Parity)Enum.Parse(typeof(Parity), Serial2Parity);
            serialPort2.Encoding = Encoding.GetEncoding("iso-8859-1");
            // Encoding = Encoding.GetEncoding("Windows-1252");
            serialPort2.Open();
            serialPort2.DtrEnable = false;
            await Task.Delay(50);
            serialPort2.DtrEnable = true;
            await Task.Delay(800);
        }

        public async void OpenSerial3()     // Serial Port para la comunicacion con el Generador
        {
            serialPort3.PortName = Serial3PortName;
            serialPort3.BaudRate = int.Parse(Serial3BaudRate);  // 115200  Valid values are 110, 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, or 115200.
            serialPort3.DataBits = int.Parse(Serial3DataBits);
            serialPort3.StopBits = (StopBits)Enum.Parse(typeof(StopBits), Serial3StopBits);
            serialPort3.Parity = (Parity)Enum.Parse(typeof(Parity), Serial3Parity);
            serialPort3.Encoding = Encoding.GetEncoding("iso-8859-1");
            // Encoding = Encoding.GetEncoding("Windows-1252");
            serialPort3.Open();
            serialPort3.DtrEnable = false;
            await Task.Delay(50);
            serialPort3.DtrEnable = true;
            await Task.Delay(800);
            StartTimer();
        }

        private void StartTimer()
        {
            t = new System.Windows.Forms.Timer();
            t.Interval = 3000;
            t.Tick += new EventHandler(t_Tick);
            t.Enabled = true;
        }

        void t_Tick(object sender, EventArgs e)
        {
            Counter += 1;
            if (Counter == 10)
            {
                buttonGRST_Click(sender, e);
            }
            if (Counter > 1)
            {
                DisplayInibit = false;
            }
            dataOUT3 = "HS";
            serialPort3.WriteLine(dataOUT3);
            if (DEBUG) DisplayData(6, dataOUT3);
            if (WaitForACK())
            {
                buttonPW.BackColor = Color.LightGreen;
                ACK = false;
                LastER = "";
            }
            else
            {
                buttonPW.BackColor = Color.Red;
                ACK = false;
                if (LastER != "Hand Shake Error")
                {
                    logger.LogError("Hand Shake Error");
                    LastER = "Hand Shake Error";
                }
            }
            ReadUPD_Data(e);
        }

        Boolean WaitForACK()
        {
            int start_time, elapsed_time;
            start_time = DateTime.Now.Second;
            while (!ACK && !NACK)
            {
                elapsed_time = DateTime.Now.Second - start_time;
                if ((elapsed_time >= 2) || (elapsed_time < 0))
                {
                    NACK = true;
                    // textBoxER.Text = "Timeout en Comunicacion";
                    return false;
                }
            }
            if (ACK) return true; else return false;
        }

        private void DisplayData(int port, string data)
        {
            switch (port)
            {
                case 1:
                    textBoxDO.Text = data;
                    break;
                case 2:
                    textBoxNO.Text = data;
                    break;
                case 3:
                    textBoxGO.Text = data;
                    break;
                case 4:
                    textBoxDI.Text = data;
                    break;
                case 5:
                    textBoxNI.Text = data;
                    break;
                case 6:
                    textBoxGI.Text = data;
                    break;
                default:
                    break;
            }
        }

        private void AnalyzeDataABC(string data)
        {
            int value = Convert.ToInt32(data.Substring(2));
            if (!Cine)
            {
                if (value < (LOW_Limit - 10))
                {
                    dataOUT3 = "K+5";
                    serialPort3.Write(dataOUT3);
                    DisplayInibit = true;
                } else
                {
                    if (value < LOW_Limit)
                    {
                        dataOUT3 = "K+1";
                        serialPort3.Write(dataOUT3);
                        DisplayInibit = true;
                    }
                }
                if (value > (HI_Limit + 10))
                {
                    dataOUT3 = "K-5";
                    serialPort3.Write(dataOUT3);
                    DisplayInibit = true;
                } else
                {
                    if (value > HI_Limit)
                    {
                        dataOUT3 = "K-1";
                        serialPort3.Write(dataOUT3);
                        DisplayInibit = true;
                    }
                }
            } else
            {
                if (value < (Cine_LOW_Limit - 10))
                {
                    dataOUT3 = "K+5";
                    serialPort3.Write(dataOUT3);
                    DisplayInibit = true;
                }
                else
                {
                    if (value < Cine_LOW_Limit)
                    {
                        dataOUT3 = "K+1";
                        serialPort3.Write(dataOUT3);
                        DisplayInibit = true;
                    }
                }
                if (value > (Cine_HI_Limit + 10))
                {
                    dataOUT3 = "K-5";
                    serialPort3.Write(dataOUT3);
                    DisplayInibit = true;
                }
                else
                {
                    if (value > Cine_HI_Limit)
                    {
                        dataOUT3 = "K-1";
                        serialPort3.Write(dataOUT3);
                        DisplayInibit = true;
                    }
                }

            }
            if (DEBUG) DisplayData(6, dataOUT3);
            serialPort1.WriteLine("ACK");
            if (DEBUG) DisplayData(4, "ACK");
        }

        private void buttonNRST_Click(object sender, EventArgs e)
        {
            serialPort2.DtrEnable = false;
            Thread.Sleep(50);
            serialPort2.DtrEnable = true;
            Thread.Sleep(100);
            serialPort2.DtrEnable = false;
        }

        private void buttonGRST_Click(object sender, EventArgs e)
        {
            serialPort3.DtrEnable = false;
            Thread.Sleep(50);
            serialPort3.DtrEnable = true;
            Thread.Sleep(100);
            serialPort3.DtrEnable = false;
            buttonPW.BackColor = Color.LightGray;
            button1.BackColor = Color.LightGray;
            button2.BackColor = Color.LightGray;
            button3.BackColor = Color.LightGray;
            buttonPW.BackColor = Color.LightGray;
            textBoxER.Text = "";
            textBoxKV.Text = "";
            textBoxMA.Text = "";
            textBoxMS.Text = "";
            textBoxKVF.Text = "";
            textBoxMAF.Text = "";
            textBoxCms.Text = "";
            textBoxSms.Text = "";
            Refresh();
        }

        private void LoggearError()
        {
            if (LastER != textBoxER.Text)
            {
                try
                {
                    logger.LogError(textBoxER.Text);
                    logger.LogWarning("VCC:" + textVCC.Substring(0, textVCC.Length - 1) +
                                " ,Sense Ref:" + textSRE.Substring(0, textSRE.Length - 1) +
                                " ,Sense Fil:" + textSFI.Substring(0, textSFI.Length - 1) +
                                " ,Sense CC:" + textSCC.Substring(0, textSCC.Length - 1) +
                                " ,U Cap:" + textSUC.Substring(0, textSUC.Length - 1) +
                                " ,I Com:" + textSIC.Substring(0, textSIC.Length - 1) +
                                " ,U Power:" + textUPW.Substring(0, textUPW.Length - 1) +
                                " ,%HU:" + textHU.Substring(0, textHU.Length - 1));
                    LastER = textBoxER.Text;
                }
                catch (Exception err)
                {
                    // MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            logger.LogInfo("Salida de la Aplicación por el operador");
            // Call Form1_FormClosing
            Form_FormClosing(sender, new FormClosingEventArgs(CloseReason.UserClosing, false));
            Application.Exit();
        }

        private void buttonPW_Click(object sender, EventArgs e)
        {
            if (serialPort3.IsOpen)
            {
                if (buttonPW.BackColor == Color.LightSkyBlue)
                {
                    if (SW_Ready)
                    {
                        dataOUT3 = "PW1";
                        serialPort3.WriteLine(dataOUT3);
                        if (DEBUG) DisplayData(6, dataOUT3);
                        // Omitir la siguiente linea en Debug
#if !DEBUG
                        this.Size = new Size(488,120);
                        this.Left = 100;  // 680;   // Centrado
                        this.Top = 948;
                        this.ControlBox = false;
                        this.Text = "";
#endif                        
                        logger.LogInfo("Turn On by Operator");
                        AutoON = true;
                    }
                    else
                    {
                        MessageBox.Show("Error de Software, Versiones incompatibles de Generador y GUI");
                    }
                }
                else
                {
                    dataOUT3 = "PW0";
                    serialPort3.WriteLine(dataOUT3);
                    if (DEBUG) DisplayData(6, dataOUT3);
                    logger.LogInfo("Turn Off by Operator");
                    AutoON = false;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (SerialNumber.Substring(SerialNumber.Length - 1) == "\r")
                {
                    SerialNumber = SerialNumber.Substring(0, SerialNumber.Length - 1);
                }
                if (File.Exists(destinationDirectory + "DUE_Serial_" + SerialNumber + ".txt"))
                {
                    File.Delete(destinationDirectory + "DUE_Serial_" + SerialNumber + ".txt");
                }
                File.Copy(fileToCopy, destinationDirectory + "DUE_Serial_" + SerialNumber + ".txt");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "GDrive Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", "C:\\TechXA\\LogIFDUE.txt");
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (serialPort1.IsOpen || serialPort2.IsOpen || serialPort3.IsOpen)
            {
                e.Cancel = true; //cancel the fom closing
                Thread CloseDown = new Thread(new ThreadStart(CloseSerialOnExit)); //close port in new thread to avoid hang
                CloseDown.Start(); //close port in new thread to avoid hang
            }
        }

        private void CloseSerialOnExit()
        {
            try
            {
                serialPort1.Close(); //close the serial port
                serialPort2.Close();
                serialPort3.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); //catch any serial port closing error messages
            }
            this.Invoke(new EventHandler(NowClose)); //now close back in the main thread
        }

        private void NowClose(object sender, EventArgs e)
        {
            this.Close(); //now close the form
        }

        private void buttonFM_Click(object sender, EventArgs e)
        {
            if (serialPort3.IsOpen)
            {
                if (buttonFM.Text == "No Fluoro")
                {
                    dataOUT3 = "TF1";
                }
                if (buttonFM.Text == "Fluoro C")
                {
                    dataOUT3 = "TF2";
                }
                if (buttonFM.Text == "Fluoro P") dataOUT3 = "TF0";
                serialPort3.WriteLine(dataOUT3);
                if (DEBUG) DisplayData(6, dataOUT3);
            }

        }

        private void buttonRM_Click(object sender, EventArgs e)
        {
            if (serialPort3.IsOpen)
            {
                if (buttonRM.Text == "RAD0")
                {
                    dataOUT3 = "TE1";
                }
                if (buttonRM.Text == "RAD1")
                {
                    dataOUT3 = "TE2";
                }
                if (buttonRM.Text == "RAD2")
                {
                    dataOUT3 = "TE3";
                }
                if (buttonRM.Text == "CINE") dataOUT3 = "TE0";       // TODO Service Mode in Generator
                // if (buttonSPot1.Text == "Service") dataOUT = "TE0";
                serialPort3.WriteLine(dataOUT3);
                if (DEBUG) DisplayData(6, dataOUT3);
            }
        }

        private void buttonSV_Click(object sender, EventArgs e)
        {
            if (serialPort3.IsOpen)
            {
                dataOUT3 = "MZ" + textBoxMAF.Text;
                serialPort3.WriteLine(dataOUT3);
                Thread.Sleep(300);
                dataOUT3 = "TX" + textBoxSms.Text;
                serialPort3.WriteLine(dataOUT3);
            }
        }

        void ReadUPD_Data(EventArgs e)
        {
            string s = u.Read();
            if (s != "")  //  Para Pruebas  eliminar ==>  && GeneratorReady)
            {
                textBoxUDP.Text = s;
                if (textBoxUDP.Text == "Head")
                {
                    // Head Set
                    dataOUT3 = "KV" + VHKV;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "MA" + VHMA;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "TC" + VHMS;
                    serialPort3.WriteLine(dataOUT3);
                }
                if (textBoxUDP.Text == "Abdomen")
                {
                    // Abdomen Set
                    dataOUT3 = "KV" + VAKV;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "MA" + VAMA;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "TC" + VAMS;
                    serialPort3.WriteLine(dataOUT3);
                }
                if (textBoxUDP.Text == "Extremity")
                {
                    // Extremity Set
                    dataOUT3 = "KV" + VEKV;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "MA" + VEMA;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "TC" + VEMS;
                    serialPort3.WriteLine(dataOUT3);
                }
                if (textBoxUDP.Text == "Cine")
                {
                    // Cine Set
                    dataOUT3 = "KV" + CIKV;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "MA" + CIMA;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "TC" + CIMS;
                    serialPort3.WriteLine(dataOUT3);
                }
                if (textBoxUDP.Text == "FluoroOff")
                {
                    // Fluoro Off
                }
                if (textBoxUDP.Text == "CineOff") 
                {
                    // Cine Off
                }
                if (textBoxUDP.Text == "FLUOROMAP")
                {
                    // Road Map
                    dataOUT3 = "KZ" + FMKV;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "MZ" + FMMA;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "TX" + FMMS;
                    serialPort3.WriteLine(dataOUT3);
                }
                if (textBoxUDP.Text == "FLUORO1")
                {
                    // "F1 Set";
                    dataOUT3 = "KZ" + F1KV;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "MZ" + F1MA;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "TX" + F1MS;
                    serialPort3.WriteLine(dataOUT3);
                }
                if (textBoxUDP.Text == "FLUORO2")
                {
                    // "F2 Set";
                    dataOUT3 = "KZ" + F2KV;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "MZ" + F2MA;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "TX" + F2MS;
                    serialPort3.WriteLine(dataOUT3);
                }
                if (textBoxUDP.Text == "FLUORO3")
                {
                    // "F3 Set";
                    dataOUT3 = "KZ" + F3KV;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "MZ" + F3MA;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "TX" + F3MS;
                    serialPort3.WriteLine(dataOUT3);
                }
                if (textBoxUDP.Text == "FLUORO4")
                {
                    // "F4 Set";
                    dataOUT3 = "KZ" + F4KV;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "MZ" + F4MA;
                    serialPort3.WriteLine(dataOUT3);
                    Thread.Sleep(300);
                    dataOUT3 = "TX" + F4MS;
                    serialPort3.WriteLine(dataOUT3);
                }
            }
        }



        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIN1 = serialPort1.ReadLine();
            this.Invoke(new EventHandler(ShowData1));
        }

        private void ShowData1(object sender, EventArgs e)
        {
            if (DEBUG) DisplayData(1, dataIN1);
            if (dataIN1.Contains("Ax"))
            {
                DisplayData(1, dataIN1);
                AnalyzeDataABC(dataIN1);
            }
            else
            {
                dataOUT2 = dataIN1;
                serialPort2.WriteLine(dataOUT2);
                if (DEBUG) DisplayData(5, dataOUT2);
            }
        }

        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIN2 = serialPort2.ReadLine();
            this.Invoke(new EventHandler(ShowData2));
        }

        private void ShowData2(object sender, EventArgs e)
        {
            if (DEBUG) DisplayData(2, dataIN2);
            serialPort1.WriteLine(dataIN2);
            if (DEBUG) DisplayData(4, dataIN2);
        }


        private void serialPort3_DataReceived(object sender, SerialDataReceivedEventArgs e)    // Data received from Generator
        {
            dataIN3 = serialPort3.ReadLine();
            if (dataIN3.Length > 4) message = dataIN3.Remove(4); else message = "";
            if (dataIN3.Contains("ACK"))
            {
                ACK = true;
                // GUI_Sound(1);
            }
            if (dataIN3.Contains("NACK"))
            {
                NACK = true;
            }
            Counter = 0;
            try
            {
                if (!DisplayInibit) this.Invoke(new EventHandler(ShowData3)); else this.Invoke(new EventHandler(ShowDataReduced));
            }
            catch (Exception err)
            {
                // MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowData3(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            if (DEBUG) DisplayData(3, dataIN3);
            string msg;
            if (dataIN3.Length > 4) msg = dataIN3.Remove(0, 4); else msg = "";
            // ACK = false;
            // NACK = false;
            switch (message)
            {
                case "EZ: ":
                    switch (msg)
                    {
                        case "SBE0\r":
                            break;

                        case "SBE1\r":
                            textBoxER.Text = "Falla de tarjeta de Estator";
                            LoggearError();

                            break;

                        case "HBE0\r":
                            break;

                        case "HBE1\r":
                            textBoxER.Text = "Falla de tarjeta de Calefaccion";
                            LoggearError();
                            break;

                        case "IBM0\r":
                            break;

                        case "IBM1\r":
                            textBoxER.Text = "Falla de tarjeta de Inversor";
                            LoggearError();
                            break;

                        case "FPE0\r":
                            break;

                        case "FPE1\r":
                            textBoxER.Text = "Verificar Relay Preparacion";
                            LoggearError();
                            break;

                        case "TMP0\r":
                            break;

                        case "TMP1\r":
                            textBoxER.Text = "Temperatura de Tubo Exedida";
                            LoggearError();
                            break;

                        default:
                            break;
                    }
                    break;
                case "ER: ":
                    if (msg != "\r")
                    {
                        textBoxER.Text = "";
                    }
                    switch (msg)
                    {
                        case "LHB\r":
                            textBoxER.Text = "Falla de Lampara Testigo Calefaccion";
                            LoggearError();
                            break;

                        case "CAP\r":
                            textBoxER.Text = "Falla de Estator (UCap)";
                            LoggearError();
                            break;

                        case "COM\r":
                            textBoxER.Text = "Falla de Estator (ICom)";
                            LoggearError();
                            break;

                        case "IBE\r":
                            button1.BackColor = Color.Red;
                            textBoxER.Text = "Falla de Inversor";
                            LoggearError();
                            break;

                        case "IBZ\r":
                            button1.BackColor = Color.Red;  // Inverter error
                            textBoxER.Text = "GAT Desconectado";
                            LoggearError();
                            break;

                        case "FIL\r":
                            button2.BackColor = Color.Red;
                            textBoxER.Text = "Falla de Filamento";
                           LoggearError();
                            break;

                        case "FCC\r":
                                textBoxER.Text = "Filamento en Corto Circuito";
                            button2.BackColor = Color.Red;
                            LoggearError();
                            break;

                        case "TMP\r":
                            button3.BackColor = Color.Red;        // Temperatura Tubo
                            textBoxER.Text = "Temperatura de Tubo Exedida";
                            LoggearError();
                            break;

                        case "EEE\r":
                            textBoxER.Text = "Falla de Memoria EEPROM";
                            LoggearError();
                            break;

                        case "SYM\r":
                            textBoxER.Text = "Simulador Activado";
                            break;

                        case "UPW\r":
                            button1.BackColor = Color.Red;
                            textBoxER.Text = "Baja Tension en UPower";
                            LoggearError();
                            break;

                        case "CPM\r":
                            // Error Stator Boar Missing
                            textBoxER.Text = "Falta Placa Estator";
                            button3.BackColor = Color.Red;
                            LoggearError();
                            break;

                        case "FPE1\r":
                            // Fin Prep Board Missing o Relay Pegado
                            textBoxER.Text = "Verificar Relay Preparacion";
                            LoggearError();
                            break;

                        case "ESF0\r":
                            textBoxER.Text = "Falla de Relay Foco Fino";
                            LoggearError();
                            break;

                        case "ESF1\r":
                            textBoxER.Text = "Falla de Relay Foco Grueso";
                            LoggearError();
                            break;


                        default:
                            if (msg != "\r") textBoxER.Text = msg;
                            break;
                    }
                    // buttonCal.BackColor = Color.RosyBrown;
                    break;
                case "ET: ":
                    textBoxET.Text = dataIN3.Remove(0, 4);
                    if (textBoxET.Text == "CINE\r") Cine = true; else Cine = false;
                    break;
                case "SN: ":
                    SerialNumber = dataIN3.Remove(0, 4);
                    break;
                case "SW: ":
                    if (dataIN3.Remove(0, 4) != SW_Version)
                    {
                        MessageBox.Show("Error de Software, Versiones incompatibles de Generador y GUI");
                        SW_Ready = false;
                        textBoxER.Text = "Software error (DUE != GUI)";
                        LoggearError();
                        break;
                    }
                    SW_Ready = true;
                    break;
                case "Kv: ":
                    if (textBoxKV.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxKV.Text = dataIN3.Remove(0, 4);
                    }
                    // kvs = Int32.Parse(textBoxKV.Text);
                    break;
                case "mA: ":
                    if (textBoxMA.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxMA.Text = dataIN3.Remove(0, 4);
                    }
                    // mas = Int32.Parse(textBoxMA.Text);
                    break;
                case "SKv:":
                    textBoxKVF.Text = dataIN3.Remove(0, 4);
                    break;
                case "SmA:":
                    textBoxMAF.Text = dataIN3.Remove(0, 4);
                    break;
                case "ms: ":
                    if (textBoxMS.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxMS.Text = dataIN3.Remove(0, 4);
                    }
                    // mss = Int32.Parse(textBoxMS.Text);
                    break;
                case "mf: ":
                    if (textBoxSms.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxSms.Text = dataIN3.Remove(0, 4);
                    }
                    break;
                case "mc: ":
                    if (textBoxCms.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxCms.Text = dataIN3.Remove(0, 4);
                    }
                    break;
                case "Kv+:":
                    textKVP = dataIN3.Remove(0, 4);
                    break;
                case "Kv-:":
                    textKVN = dataIN3.Remove(0, 4);
                    break;
                case "RmA:":
                    textRmA = dataIN3.Remove(0, 4);
                    try
                    {
                        decimal mARead = Convert.ToDecimal(textRmA);
                        int mASet = Convert.ToInt32(textBoxMA.Text);
                        int mAReal = (int)(mARead * mASet) / 1000;
                        textmAReal = mAReal.ToString();
                    }
                    catch
                    {
                        textmAReal = "???";
                    }
                    break;
                case "SFI:":
                    textSFI = dataIN3.Remove(0, 4);
                    break;
                case "SRE:":
                    textSRE = dataIN3.Remove(0, 4);
                    break;
                case "SCC:":
                    textSCC = dataIN3.Remove(0, 4);
                    break;
                case "SIC:":
                    textSIC = dataIN3.Remove(0, 4);
                    break;
                case "SUC:":
                    textSUC = dataIN3.Remove(0, 4);
                    break;
                case "UPW:":
                    textUPW = dataIN3.Remove(0, 4);
                    break;
                case "TST:":
                    // textBoxTST.Text = dataIN3.Remove(0, 4);
                    break;
                case "FO: ":
                    break;
                case "HO: ":
                    // textBoxHO.Text = dataIN3.Remove(0, 4);
                    break;
                case "CAL:":
                    if (msg == "1\r")
                    {
                        dataOUT3 = "DB0";
                        serialPort3.WriteLine(dataOUT3);   // Send data to Generator to turn off Calibration
                        if (DEBUG) DisplayData(6, dataOUT3);
                    }
                    break;
                case "VCC:":
                    textVCC = dataIN3.Remove(0, 4);
                 /* if (textVCC != "")
                    {
                        try
                        {
                            float Test = float.Parse(textVCC);
                            if (Test < (VCC - 0.3) || Test > (VCC + 0.3)) radioButton3.BackColor = Color.Red;
                            else
                            {
                                if (Test < (VCC - 0.1) || Test > (VCC + 0.1)) radioButton3.BackColor = Color.Yellow;
                                else
                                {
                                    if (Test > (VCC - 0.11) || Test < (VCC + 0.09)) radioButton3.BackColor = Color.LightGreen;
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            //   MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } */
                    break;
                case "HU: ":
                    textHU = dataIN3.Remove(0, 4);
                    break;
                case "FT: ":
                    if (msg == "0\r")
                    {
                        buttonFM.BackColor = Color.LightGray;
                        buttonFM.Text = "No Fluoro";
                    }
                    if (msg == "1\r")
                    {
                        buttonFM.BackColor = Color.LightYellow;
                        buttonFM.Text = "Fluoro C";
                    }
                    if (msg == "2\r")
                    {
                        buttonFM.BackColor = Color.LightGreen;
                        buttonFM.Text = "Fluoro P";
                    }
                    break;
                case "RD: ":
                    if ((buttonPW.BackColor == Color.LightSkyBlue) && AutoON)
                    {
                        buttonPW_Click(sender, e);
                    }

                    if (msg == "0\r")
                    {
                        buttonRM.BackColor = Color.LightYellow;
                        buttonRM.Text = "RAD0";
                    }
                    if (msg == "1\r")
                    {
                        buttonRM.BackColor = Color.LightGreen;
                        buttonRM.Text = "RAD1";
                    }
                    if (msg == "2\r")
                    {
                        buttonRM.BackColor = Color.LightGreen;
                        buttonRM.Text = "RAD2";
                    }
                    if (msg == "3\r")
                    {
                        buttonRM.BackColor = Color.LightGreen;
                        buttonRM.Text = "CINE";
                    }
                    if (msg == "4\r")
                    {
                        buttonRM.BackColor = Color.Red;
                        buttonRM.Text = "Service";
                    }
                    break;
                case "CL: ":
                    break;
                case "POK:":
                 /* if (msg == "0\r")
                    {
                        PROK = 0;
                    }
                    if (msg == "1\r")
                    {
                        PROK = 1;
                    }
                    if (msg == "2\r")
                    {
                        PROK = 2;
                    } */
                    break;
                case "XOK:":
                 /* if (msg == "0\r")
                    {
                        // buttonPrep
                        RXOK = 0;
                    }
                    if (msg == "1\r")
                    {
                        RXOK = 1;
                    } */
                    break;
                case "EEP:":
                    // ConfigSize = Convert.ToInt32(dataIN.Remove(0, 4));
                    // ConfigReady = true;
                    break;

                case "TC1:":
                    // textBoxTC1.Text = dataIN.Remove(0, 4) + "ºC";
                    break;

                case "LOG:":
                    // GUI_Sound(4);
                    logger.LogInfo("VCC:" + textVCC.Substring(0, textVCC.Length - 1) +
                                   " Kv:" + textBoxKV.Text.Substring(0, textBoxKV.Text.Length - 1) +
                                   " mA:" + textBoxMA.Text.Substring(0, textBoxMA.Text.Length - 1) +
                                   " ms:" + textBoxMS.Text.Substring(0, textBoxMS.Text.Length - 1) +
                                   " Kv+:" + textKVP.Substring(0, textKVP.Length - 1) +
                                   " Kv-:" + textKVN.Substring(0, textKVN.Length - 1) +
                                   " mA:" + textmAReal + " %HU:" + textHU);

                    // logger.LogWarning("Este es un mensaje de advertencia.");
                    // logger.LogError("Este es un mensaje de error.");
                    break;

                default:
                    break;
            }

            if ((textBoxET.Text == "OFF\r") || (textBoxET.Text == "ERROR\r") || (textBoxET.Text == "\r"))
            {
                buttonPW.BackColor = Color.LightSkyBlue;
            }

            if ((textBoxET.Text == "IDLE\r")) // || (textBox1.Text == "ERROR\r") || (textBox1.Text == "\r")
            {
                buttonPW.BackColor = Color.LightGreen;
            }
        }

        private void ShowDataReduced(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            if (DEBUG) DisplayData(3, dataIN3);
            string msg;
            if (dataIN3.Length > 4) msg = dataIN3.Remove(0, 4); else msg = "";
            // ACK = false;
            // NACK = false;
            switch (message)
            {
                case "ER: ":
                    if (msg != "\r")
                    {
                        textBoxER.Text = "";
                    }
                    switch (msg)
                    {
                        case "LHB\r":
                            textBoxER.Text = "Falla de Lampara Testigo Calefaccion";
                            LoggearError();
                            break;

                        case "CAP\r":
                            textBoxER.Text = "Falla de Estator (UCap)";
                            LoggearError();
                            break;

                        case "COM\r":
                            textBoxER.Text = "Falla de Estator (ICom)";
                            LoggearError();
                            break;

                        case "IBE\r":
                            button1.BackColor = Color.Red;
                            textBoxER.Text = "Falla de Inversor";
                            LoggearError();
                            break;

                        case "IBZ\r":
                            button1.BackColor = Color.Red;  // Inverter error
                            textBoxER.Text = "GAT Desconectado";
                            LoggearError();
                            break;

                        case "FIL\r":
                            button2.BackColor = Color.Red;
                            textBoxER.Text = "Falla de Filamento";
                            LoggearError();
                            break;

                        case "FCC\r":
                            textBoxER.Text = "Filamento en Corto Circuito";
                            button2.BackColor = Color.Red;
                            LoggearError();
                            break;

                        case "TMP\r":
                            button3.BackColor = Color.Red;        // Temperatura Tubo
                            textBoxER.Text = "Temperatura de Tubo Exedida";
                            LoggearError();
                            break;

                        case "EEE\r":
                            textBoxER.Text = "Falla de Memoria EEPROM";
                            LoggearError();
                            break;

                        case "SYM\r":
                            textBoxER.Text = "Simulador Activado";
                            break;

                        case "UPW\r":
                            button1.BackColor = Color.Red;
                            textBoxER.Text = "Baja Tension en UPower";
                            LoggearError();
                            break;

                        case "CPM\r":
                            // Error Stator Boar Missing
                            textBoxER.Text = "Falta Placa Estator";
                            button3.BackColor = Color.Red;
                            LoggearError();
                            break;

                        case "FPE1\r":
                            // Fin Prep Board Missing o Relay Pegado
                            textBoxER.Text = "Verificar Relay Preparacion";
                            LoggearError();
                            break;

                        case "ESF0\r":
                            textBoxER.Text = "Falla de Relay Foco Fino";
                            LoggearError();
                            break;

                        case "ESF1\r":
                            textBoxER.Text = "Falla de Relay Foco Grueso";
                            LoggearError();
                            break;


                        default:
                            if (msg != "\r") textBoxER.Text = msg;
                            break;
                    }
                    // buttonCal.BackColor = Color.RosyBrown;
                    break;
                case "ET: ":
                    textBoxET.Text = dataIN3.Remove(0, 4);
                    break;
                case "Kv: ":
                    if (textBoxKV.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxKV.Text = dataIN3.Remove(0, 4);
                    }
                    // kvs = Int32.Parse(textBoxKV.Text);
                    break;
                case "SKv:":
                    textBoxKVF.Text = dataIN3.Remove(0, 4);
                    break;
                case "CL: ":
                    break;

                case "LOG:":
                    // GUI_Sound(4);
                    logger.LogInfo("VCC:" + textVCC.Substring(0, textVCC.Length - 1) +
                                   " Kv:" + textBoxKV.Text.Substring(0, textBoxKV.Text.Length - 1) +
                                   " mA:" + textBoxMA.Text.Substring(0, textBoxMA.Text.Length - 1) +
                                   " ms:" + textBoxMS.Text.Substring(0, textBoxMS.Text.Length - 1) +
                                   " Kv+:" + textKVP.Substring(0, textKVP.Length - 1) +
                                   " Kv-:" + textKVN.Substring(0, textKVN.Length - 1) +
                                   " mA:" + textmAReal + " %HU:" + textHU);

                    // logger.LogWarning("Este es un mensaje de advertencia.");
                    // logger.LogError("Este es un mensaje de error.");
                    break;

                default:
                    break;
            }

            if ((textBoxET.Text == "OFF\r") || (textBoxET.Text == "ERROR\r") || (textBoxET.Text == "\r"))
            {
                buttonPW.BackColor = Color.LightSkyBlue;
            }

            if ((textBoxET.Text == "IDLE\r")) // || (textBox1.Text == "ERROR\r") || (textBox1.Text == "\r")
            {
                buttonPW.BackColor = Color.LightGreen;
            }
        }
    }

// Add a public class logger to the project
    public class Logger
    {
        private string logFilePath;

        public Logger(string logFilePath)
        {
            this.logFilePath = logFilePath;
            // Si el archivo no existe, crea uno nuevo; si existe, lo abre en modo append (agregar al final).
            File.AppendAllText(logFilePath, "=== Inicio de Log Interface Pimax Digirad ===" + Environment.NewLine);
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARNING", message);
        }

        public void LogError(string message)
        {
            WriteLog("ERROR", message);
        }

        private void WriteLog(string logLevel, string message)
        {
            string logEntry = $"{DateTime.Now} [{logLevel}] - {message}{Environment.NewLine}";
            File.AppendAllText(logFilePath, logEntry);
        }
    }
}