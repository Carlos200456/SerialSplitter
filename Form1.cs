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
        string VHKV, VHMA, VHMS, VNKV, VNMA, VNMS, VAKV, VAMA, VAMS, VEKV, VEMA, VEMS, VUEKV, VUEMA, VUEMS, VFKV, VFMA, VFMS, CIKV, CIMA, CIMS, CHKV, CHMA, CHMS, FMKV, FMMA, FMMS, F1KV, F1MA, F1MS, F2KV, F2MA, F2MS, F3KV, F3MA, F3MS, F4KV, F4MA, F4MS, SerialNumber = "";
        string dataIN1 = "", dataIN2 = "", dataIN3 = "", dataOUT1 = "", dataOUT2 = "", dataOUT3 = "", path, textKVP, textKVN, textmAReal, textRmA, LastER, textSFI, textSRE, textSCC, textSIC, textSUC, textUPW, textHU, textVCC, message;
        string Serial1PortName, Serial1BaudRate, Serial1DataBits, Serial1StopBits, Serial1Parity, Serial2PortName, Serial2BaudRate, Serial2DataBits, Serial2StopBits, Serial2Parity, Serial3PortName, Serial3BaudRate, Serial3DataBits, Serial3StopBits, Serial3Parity;

        // readonly string[] mA_Table = new string[8] { "50\r", "100\r", "200\r", "300\r", "400\r", "500\r", "600\r", "700\r" };
        // readonly string[] ms_Table = new string[30] { "2\r", "5\r", "8\r", "10\r", "20\r", "30\r", "40\r", "50\r", "60\r", "80\r", "100\r", "120\r", "150\r", "200\r", "250\r", "300\r", "400\r", "500\r", "600\r", "800\r", "1000\r", "1200\r", "1500\r", "2000\r", "2500\r", "3000\r", "3500\r", "4000\r", "4500\r", "5000\r" };
        bool ACK = false;
        bool NACK = false;
        bool AutoON = true;
        bool SW_Ready = false;
        public static bool OpenIrisDW = false;
        public static bool CloseIrisDW = false;
        public static bool IrisUp = false;

#if !DEBUG
        bool DEBUG = false;
#else
        bool DEBUG = true;
#endif
        bool Cine = false;
        bool AEC_ON = true;
        bool RX_On = false;
        bool AEC_Lock = true;
        public static float VCC = 0.0f;
        int Counter, VHKVOF = 15, VNKVOF = 12, VAKVOF = 12, VEKVOF = 6, VUEKVOF = 6, VFKVOF = 6, CIKVOF = 4, CHKVOF = 4, 
            LOW_Limit, HI_Limit, Cine_LOW_Limit, Cine_HI_Limit, Offset_KV_Cine, AnalogData, Old_AnalogData, ValorMedioCine, 
            ValorMedioFluoro, Demora_SendKV, Demora_AEC, Offset_Max_Fluoro, Offset_Max_Cine;
        int dif_aec = 0;
        // float mxs;

        // StringBuilder sb = new StringBuilder();
        // char LF = (char)10;
        // char CR = (char)13;

        System.Windows.Forms.Timer t = null;
        System.Windows.Forms.Timer f = null;

        // Lock for app path
        private static readonly object _lock = new object();
        private static string _appPath;
        public static string AppPath
        {
            get
            {
                lock (_lock)
                {
                    if (string.IsNullOrEmpty(_appPath))
                    {
                        _appPath = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    return _appPath;
                }
            }
        }

        static string logFilePath = AppPath + "LogIFDUE.txt";
        static string destinationDirectory = "G:\\My Drive\\Logs\\";

        Logger logger = new Logger(logFilePath);    // Ruta del archivo de log

        // Create an isntance of IPC with DRAXA
        SimpleUDP u = new SimpleUDP(45000);
        // ...
        // string s = u.Read();
        // u.Write("Welcome App1");

        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Button existingButton1 = buttonIrisOpen;
            System.Windows.Forms.Button existingButton2 = buttonIrisClose;
            // Remove button2 and button3 from the form
            this.Controls.Remove(existingButton1);
            this.Controls.Remove(existingButton2);
            CustomButton ButtonIrisOpen = new CustomButton();
            CustomButton ButtonIrisClose = new CustomButton();
            ButtonIrisOpen.Location = existingButton1.Location;
            ButtonIrisOpen.Size = existingButton1.Size;
            ButtonIrisOpen.Text = existingButton1.Text;
            ButtonIrisOpen.Font = existingButton1.Font;
            ButtonIrisOpen.MouseDown += buttonIrisOpen_MouseDown;
            ButtonIrisOpen.MouseUp += buttonIris_MouseUp;
            // ... Set any other properties you need ...
            this.Controls.Add(ButtonIrisOpen);
            ButtonIrisClose.Location = existingButton2.Location;
            ButtonIrisClose.Size = existingButton2.Size;
            ButtonIrisClose.Text = existingButton2.Text;
            ButtonIrisClose.Font = existingButton2.Font;
            ButtonIrisClose.MouseDown += buttonIrisClose_MouseDown;
            ButtonIrisClose.MouseUp += buttonIris_MouseUp;
            // ... Set any other properties you need ...
            this.Controls.Add(ButtonIrisClose);

            LastER = "";

            try
            {
                using (XmlTextReader configReader = new XmlTextReader(AppPath + "Config_DUE_IF.xml"))
                {
                    // Move to <Configuration>
                    while (configReader.Read())
                    {
                        if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "Configuration")
                            break;
                    }

                    // Now read the configuration elements inside <Configuration>
                    while (configReader.Read())
                    {
                        if (configReader.NodeType == XmlNodeType.Element)
                        {
                            string elementName = configReader.Name;
                            string s1 = configReader.ReadElementContentAsString();

                            switch (elementName)
                            {
                                case "SerialPort1":
                                    Serial1PortName = getBetween(s1, "name=", 4);
                                    Serial1BaudRate = getBetween(s1, "baudrate=", 5);
                                    Serial1DataBits = getBetween(s1, "databits=", 1);
                                    Serial1StopBits = getBetween(s1, "stopbits=", 3);
                                    Serial1Parity = getBetween(s1, "parity=", 4);
                                    break;
                                case "SerialPort2":
                                    Serial2PortName = getBetween(s1, "name=", 4);
                                    Serial2BaudRate = getBetween(s1, "baudrate=", 5);
                                    Serial2DataBits = getBetween(s1, "databits=", 1);
                                    Serial2StopBits = getBetween(s1, "stopbits=", 3);
                                    Serial2Parity = getBetween(s1, "parity=", 4);
                                    break;
                                case "SerialPort3":
                                    Serial3PortName = getBetween(s1, "name=", 4);
                                    Serial3BaudRate = getBetween(s1, "baudrate=", 5);
                                    Serial3DataBits = getBetween(s1, "databits=", 1);
                                    Serial3StopBits = getBetween(s1, "stopbits=", 3);
                                    Serial3Parity = getBetween(s1, "parity=", 4);
                                    break;
                                case "VascularHead":
                                    VHKV = getBetween(s1, "Kv=", 3);
                                    VHKVOF = Convert.ToInt32(getBetween(s1, "KvOffSet=", 2));
                                    VHMA = getBetween(s1, "mA=", 3);
                                    VHMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "VascularNeck":
                                    VNKV = getBetween(s1, "Kv=", 3);
                                    VNKVOF = Convert.ToInt32(getBetween(s1, "KvOffSet=", 2));
                                    VNMA = getBetween(s1, "mA=", 3);
                                    VNMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "Cine":
                                    CIKV = getBetween(s1, "Kv=", 3);
                                    CIKVOF = Convert.ToInt32(getBetween(s1, "KvOffSet=", 2));
                                    CIMA = getBetween(s1, "mA=", 3);
                                    CIMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "VascularArms":
                                    VUEKV = getBetween(s1, "Kv=", 3);
                                    VUEKVOF = Convert.ToInt32(getBetween(s1, "KvOffSet=", 2));
                                    VUEMA = getBetween(s1, "mA=", 3);
                                    VUEMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "VascularAbdomen":
                                    VAKV = getBetween(s1, "Kv=", 3);
                                    VAKVOF = Convert.ToInt32(getBetween(s1, "KvOffSet=", 2));
                                    VAMA = getBetween(s1, "mA=", 3);
                                    VAMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "VascularLegs":
                                    VEKV = getBetween(s1, "Kv=", 3);
                                    VEKVOF = Convert.ToInt32(getBetween(s1, "KvOffSet=", 2));
                                    VEMA = getBetween(s1, "mA=", 3);
                                    VEMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "VascularFoot":
                                    VFKV = getBetween(s1, "Kv=", 3);
                                    VFKVOF = Convert.ToInt32(getBetween(s1, "KvOffSet=", 2));
                                    VFMA = getBetween(s1, "mA=", 3);
                                    VFMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "Heart":
                                    CHKV = getBetween(s1, "Kv=", 3);
                                    CHKVOF = Convert.ToInt32(getBetween(s1, "KvOffSet=", 2));
                                    CHMA = getBetween(s1, "mA=", 3);
                                    CHMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "FLUOROMAP":
                                    FMKV = getBetween(s1, "Kv=", 3);
                                    FMMA = getBetween(s1, "mA=", 2);
                                    FMMS = getBetween(s1, "ms=", 2);
                                    break;
                                case "FLUORO1":
                                    F1KV = getBetween(s1, "Kv=", 3);
                                    F1MA = getBetween(s1, "mA=", 2);
                                    F1MS = getBetween(s1, "ms=", 2);
                                    break;
                                case "FLUORO2":
                                    F2KV = getBetween(s1, "Kv=", 3);
                                    F2MA = getBetween(s1, "mA=", 2);
                                    F2MS = getBetween(s1, "ms=", 2);
                                    break;
                                case "FLUORO3":
                                    F3KV = getBetween(s1, "Kv=", 3);
                                    F3MA = getBetween(s1, "mA=", 2);
                                    F3MS = getBetween(s1, "ms=", 2);
                                    break;
                                case "FLUORO4":
                                    F4KV = getBetween(s1, "Kv=", 3);
                                    F4MA = getBetween(s1, "mA=", 2);
                                    F4MS = getBetween(s1, "ms=", 2);
                                    break;
                                case "Power":
                                    VCC = Convert.ToSingle(getBetween(s1, "VCC=", 5));
                                    LOW_Limit = Convert.ToInt32(getBetween(s1, "LOW_Limit=", 3));
                                    HI_Limit = Convert.ToInt32(getBetween(s1, "HI_Limit=", 3));
                                    Cine_LOW_Limit = Convert.ToInt32(getBetween(s1, "Cine_LOW_Limit=", 3));
                                    Cine_HI_Limit = Convert.ToInt32(getBetween(s1, "Cine_HI_Limit=", 3));
                                    Offset_Max_Fluoro = Convert.ToInt32(getBetween(s1, "Offset_Max_Fluoro=", 3));
                                    Offset_Max_Cine = Convert.ToInt32(getBetween(s1, "Offset_Max_Cine=", 3));
                                    break;
                                default:
                                    break;
                            }
                        }
                        // Stop if we reach the end of <Configuration>
                        if (configReader.NodeType == XmlNodeType.EndElement && configReader.Name == "Configuration")
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            CheckPortsNames();
            this.TopMost = true;
            buttonAEC.BackColor = Color.LightGreen;
            buttonAEC.Text = "AEC ON";
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

        public async void OpenSerial1()     // Serial Port para la comunicacion con el Software Digirad
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

        public async void OpenSerial2()     // Serial Port para la comunicacion con la Nano_IF
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
            f = new System.Windows.Forms.Timer();
            f.Interval = 100;
            f.Tick += new EventHandler(f_Tick);
            f.Enabled = true;
        }

        // Make t_Tick async
        private void t_Tick(object sender, EventArgs e)
        {
            Counter += 1;
            if (Counter == 10)
            {
                buttonGRST_Click(sender, e);
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
        }

        // Make f_Tick async
        private async void f_Tick(object sender, EventArgs e)
        {
            await ReadUPD_Data(e); // Await the async method
            if (Demora_SendKV == 1)
            {
                // Ensure textBoxKVF.Text contains a valid integer value.
                if (int.TryParse(textBoxKVF.Text, out int kv))
                {
                    kv = kv - Offset_KV_Cine;
                    if (kv < 40) kv = 40;
                    dataOUT3 = "KV" + kv.ToString();
                    serialPort3.WriteLine(dataOUT3);
                    if (DEBUG) DisplayData(6, dataOUT3);
                }
            }
            if (Demora_SendKV > 0) Demora_SendKV -= 1;
            if (Demora_AEC > 0) Demora_AEC -= 1;
        }

        bool WaitForACK()
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

        private void AnalyzeDataABC(int value, object sender, EventArgs e)
        {
            ValorMedioCine = ((Cine_HI_Limit - Cine_LOW_Limit) / 2) + Cine_LOW_Limit;
            ValorMedioFluoro = ((HI_Limit - LOW_Limit) / 2) + LOW_Limit;
            if (!Cine)   // AEC Fluoroscopia
            {
                if (value < LOW_Limit)
                {
                    dif_aec = (ValorMedioFluoro - value) / 4;
                    if (dif_aec > Offset_Max_Fluoro) dif_aec = Offset_Max_Fluoro;
                    if (dif_aec < 1) dif_aec = 1;
                    button4_Click(sender, e);
                }
                if (value > HI_Limit)
                {
                    dif_aec = (value - ValorMedioFluoro) / 4;
                    if (dif_aec > Offset_Max_Fluoro) dif_aec = Offset_Max_Fluoro;
                    if (dif_aec < 1) dif_aec = 1;
                    button5_Click(sender, e);
                }
            }
            else    // AEC Cine
            {
                if (value < Cine_LOW_Limit)
                {
                    dif_aec = (ValorMedioCine - value) / 5;
                    if (dif_aec > Offset_Max_Cine) dif_aec = Offset_Max_Cine;
                    if (dif_aec < 1) dif_aec = 1;
                    button4_Click(sender, e);
                }
                if (value > Cine_HI_Limit)
                {
                    dif_aec = (value - ValorMedioCine) / 5;
                    if (dif_aec > Offset_Max_Cine) dif_aec = Offset_Max_Cine;
                    if (dif_aec < 1) dif_aec = 1;
                    button5_Click(sender, e);
                }
            }
            if (DEBUG) DisplayData(6, dataOUT3);
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
            textBoxMScine.Text = "";
            textBoxKVF.Text = "";
            textBoxMAF.Text = "";
            textBoxMSrad.Text = "";
            textBoxSms.Text = "";
            Refresh();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            logger.LogInfo("Salida de la Aplicación por el operador");
            this.Close(); // Esto dispara Form_FormClosing automáticamente
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
                        this.Size = new Size(518, 200);
                        this.Left = 100;  // 680;   // Centrado
                        this.Top = 910;
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
            string SafeTrim(string s)
            {
                return string.IsNullOrEmpty(s) ? "N" : s.TrimEnd('\r', '\n', ' ');
            }

            try
            {
                SerialNumber = SafeTrim(SerialNumber);
                if (File.Exists(destinationDirectory + "Aspor_Serial_" + SerialNumber + ".txt"))
                {
                    File.Delete(destinationDirectory + "Aspor_Serial_" + SerialNumber + ".txt");
                }
                File.Copy(logFilePath, destinationDirectory + "Aspor_Serial_" + SerialNumber + ".txt");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "GDrive Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", logFilePath);
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

        private void buttonAEC_Click(object sender, EventArgs e)
        {
            if (AEC_ON)
            {
                AEC_ON = false;
                buttonAEC.BackColor = Color.Red;
                buttonAEC.Text = "AEC OFF";
                dif_aec = 1;
            }
            else
            {
                AEC_ON = true;
                buttonAEC.BackColor = Color.LightGreen;
                buttonAEC.Text = "AEC ON";
            }
        }

        private void button4_Click(object sender, EventArgs e) // KV +
        {
            if (serialPort3.IsOpen)
            {
                dataOUT3 = "K+" + dif_aec.ToString();
                serialPort3.WriteLine(dataOUT3);
                if (DEBUG) DisplayData(6, dataOUT3);
            }
        }

        private void button5_Click(object sender, EventArgs e) // KV -
        {
            dataOUT3 = "K-" + dif_aec.ToString();
            serialPort3.WriteLine(dataOUT3);
            if (DEBUG) DisplayData(6, dataOUT3);
        }

        // Colimator Control Buttons
        public void buttonIrisOpen_MouseDown(object sender, MouseEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                dataOUT2 = "IC5";
                serialPort2.WriteLine(dataOUT2);
            }
        }

        private void buttonIris_MouseUp(object sender, MouseEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                dataOUT2 = "IC0";
                serialPort2.WriteLine(dataOUT2);
            }

        }

        private void buttonIrisClose_MouseDown(object sender, MouseEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                dataOUT2 = "IC-5";
                serialPort2.WriteLine(dataOUT2);
            }
        }

        private void buttonVColsOpen_MouseDown(object sender, MouseEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                dataOUT2 = "VC5";
                serialPort2.WriteLine(dataOUT2);
            }
        }

        private void buttonVCol_MouseUp(object sender, MouseEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                dataOUT2 = "VC0";
                serialPort2.WriteLine(dataOUT2);
            }
        }

        private void buttonVColsClose_MouseDown(object sender, MouseEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                dataOUT2 = "VC-5";
                serialPort2.WriteLine(dataOUT2);
            }
        }

        private void buttonRotCW_MouseDown(object sender, MouseEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                dataOUT2 = "RO5";
                serialPort2.WriteLine(dataOUT2);
            }
        }

        private void buttonRot_MouseUp(object sender, MouseEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                dataOUT2 = "RO0";
                serialPort2.WriteLine(dataOUT2);
            }
        }

        private void buttonRotCCW_MouseDown(object sender, MouseEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                dataOUT2 = "RO-5";
                serialPort2.WriteLine(dataOUT2);
            }
        }

        // Make ReadUPD_Data async
        private async Task ReadUPD_Data(EventArgs e)
        {
            string s = u.Read();
            if (string.IsNullOrEmpty(s))
                return;

            textBoxUDP.Text = s;

            // Helper to send a sequence of commands with delay
            async Task SendCommands(params string[] commands)
            {
                foreach (var cmd in commands)
                {
                    serialPort3.WriteLine(cmd);
                    if (DEBUG) DisplayData(6, cmd);
                    await Task.Delay(300); // Non-blocking delay
                }
            }

            switch (s)
            {
                case "Head":
                    await SendCommands("KV" + VHKV, "MA" + VHMA, "TC" + VHMS);
                    Offset_KV_Cine = VHKVOF; // 15
                    break;
                case "Neck":
                    await SendCommands("KV" + VNKV, "MA" + VNMA, "TC" + VNMS);
                    Offset_KV_Cine = VNKVOF; // 15
                    break;
                case "Cine":
                    await SendCommands("KV" + CIKV, "MA" + CIMA, "TC" + CIMS);
                    Offset_KV_Cine = CIKVOF; // 4
                    break;
                case "Arms":
                    await SendCommands("KV" + VUEKV, "MA" + VUEMA, "TC" + VUEMS);
                    Offset_KV_Cine = VUEKVOF; // 6
                    break;
                case "Abdomen":
                    await SendCommands("KV" + VAKV, "MA" + VAMA, "TC" + VAMS);
                    Offset_KV_Cine = VAKVOF; // 12
                    break;
                case "Legs":
                    await SendCommands("KV" + VEKV, "MA" + VEMA, "TC" + VEMS);
                    Offset_KV_Cine = VEKVOF; // 6
                    break;
                case "Foot":
                    await SendCommands("KV" + VFKV, "MA" + VFMA, "TC" + VFMS);
                    Offset_KV_Cine = VFKVOF; // 6
                    break;
                case "Heart":
                    await SendCommands("KV" + CHKV, "MA" + CHMA, "TC" + CHMS);
                    Offset_KV_Cine = CHKVOF; // 4
                    break;
                case "FLUOROMAP":
                    await SendCommands("KZ" + FMKV, "MZ" + FMMA, "TX" + FMMS);
                    break;
                case "FLUORO1":
                    await SendCommands("KZ" + F1KV, "MZ" + F1MA, "TX" + F1MS);
                    break;
                case "FLUORO2":
                    await SendCommands("KZ" + F2KV, "MZ" + F2MA, "TX" + F2MS);
                    break;
                case "FLUORO3":
                    await SendCommands("KZ" + F3KV, "MZ" + F3MA, "TX" + F3MS);
                    break;
                case "FLUORO4":
                    await SendCommands("KZ" + F4KV, "MZ" + F4MA, "TX" + F4MS);
                    break;
                case "FluoroOff":
                    // No action needed
                    break;
                case "CineOff":
                    // No action needed
                    break;
                default:
                    // Optionally log unknown command
                    break;
            }
        }

        void SendCommands(string command1, string command2, string command3)
        {
            if (serialPort3.IsOpen)
            {
                dataOUT3 = command1;
                serialPort3.WriteLine(dataOUT3);
                Thread.Sleep(300);
                dataOUT3 = command2;
                serialPort3.WriteLine(dataOUT3);
                Thread.Sleep(300);
                dataOUT3 = command3;
                serialPort3.WriteLine(dataOUT3);
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

        private void LoggearError()
        {
            if (LastER != textBoxER.Text)
            {
                try
                {
                    logger.LogError(textBoxER.Text);

                    // Helper to safely trim trailing \r, \n, or whitespace
                    string SafeTrim(string s)
                    {
                        return string.IsNullOrEmpty(s) ? "?" : s.TrimEnd('\r', '\n', ' ');
                    }

                    logger.LogWarning(
                        "VCC:" + SafeTrim(textVCC) +
                        " ,Sense Ref:" + SafeTrim(textSRE) +
                        " ,Sense Fil:" + SafeTrim(textSFI) +
                        " ,Sense CC:" + SafeTrim(textSCC) +
                        " ,U Cap:" + SafeTrim(textSUC) +
                        " ,I Com:" + SafeTrim(textSIC) +
                        " ,U Power:" + SafeTrim(textUPW) +
                        " ,%HU:" + SafeTrim(textHU)
                    );

                    LastER = textBoxER.Text;
                }
                catch (Exception ex)
                {
                    // Optionally log to a fallback location or show a message in DEBUG mode
#if DEBUG
                    Debug.WriteLine("LoggearError exception: " + ex.Message);
#endif
                    // Do not show MessageBox to avoid UI blocking in production
                }
            }
        }

        private void CloseSerialOnExit()
        {
            try
            {
                // Cierre seguro de los puertos
                if (serialPort1 != null && serialPort1.IsOpen) { try { serialPort1.Close(); } catch { } }
                if (serialPort2 != null && serialPort2.IsOpen) { try { serialPort2.Close(); } catch { } }
                if (serialPort3 != null && serialPort3.IsOpen) { try { serialPort3.Close(); } catch { } }
            }
            catch { }
            finally
            {
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    if (this.InvokeRequired)
                        this.Invoke(new EventHandler(NowClose));
                    else
                        NowClose(this, EventArgs.Empty);
                }
            }
        }

        private void NowClose(object sender, EventArgs e)
        {
            this.Close(); //now close the form
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            t?.Stop();
            f?.Stop();

            if ((serialPort1?.IsOpen ?? false) || (serialPort2?.IsOpen ?? false) || (serialPort3?.IsOpen ?? false))
            {
                e.Cancel = true;
                Thread CloseDown = new Thread(new ThreadStart(CloseSerialOnExit));
                CloseDown.Start();
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
            File.AppendAllText(logFilePath, "=== Log Interface Aspor - Digirad ===" + Environment.NewLine);
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

    public class CustomButton : System.Windows.Forms.Button
    {
        const int WM_POINTERDOWN = 0x0246;
        const int WM_POINTERUP = 0x0247;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_POINTERDOWN)
            {
                // Differentiate between the two buttons
                if (this.Text == "Iris Open")
                {
                    // Handle the Iris Open button "buttonIrisOpen_MouseDown"
                    Form1.OpenIrisDW = true;

                }
                else if (this.Text == "Iris Close")
                {
                    // Handle the Iris Close button "buttonIrisClose_MouseDown"
                    Form1.CloseIrisDW = true;
                }
            }
            else if (m.Msg == WM_POINTERUP)
            {
                // Handle pointer up event
                Form1.IrisUp = true;
            }
        }
    }

}
