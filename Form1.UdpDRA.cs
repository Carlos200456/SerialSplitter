using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SerialSplitter
{
    // Canal con el DRA.XA por UDP en lugar de serialPort1 (puerto serie virtual).
    //
    // Se activa desde el XML con name="UDP" en <SerialPort1>, por ejemplo:
    //   <SerialPort1> name="UDP" localport="45011" remoteport="45010" </SerialPort1>
    // localport  = donde escucha SerialSplitter (default 45011)
    // remoteport = donde escucha el DRA.XA       (default 45010)
    // baudrate/stopbits/etc. se ignoran en modo UDP.
    //
    // Formato: el mismo de SimpleUDP (UTF-32LE), solo loopback. Cada mensaje lleva '\n' al
    // final como el WriteLine del serie, y del lado de recepcion se acumula hasta '\n' (un
    // datagrama no es necesariamente una linea). Los puertos 45000/45001 ya estan usados
    // (nombres de tecnica y MonitorConsola), no reutilizarlos.
    public partial class Form1
    {
        private const int DefaultUdpDRALocalPort = 45011;
        private const int DefaultUdpDRARemotePort = 45010;
        private const int SIO_UDP_CONNRESET = -1744830452;

        string Serial1LocalPort, Serial1RemotePort;

        private UdpClient _udpDRA;
        private IPEndPoint _udpDRARemote;
        private Thread _udpDRAThread;
        private volatile bool _udpDRARunning = false;
        private readonly object _udpDRASendLock = new object();

        private bool UdpDRAMode => string.Equals(Serial1PortName, "UDP", StringComparison.OrdinalIgnoreCase);
        private bool UdpDRAOpen => _udpDRA != null;

        private static int ParseUdpPort(string s, int def)
        {
            if (int.TryParse(s, out int p) && p > 0 && p < 65536) return p;
            return def;
        }

        private bool OpenUdpDRA()
        {
            int local = ParseUdpPort(Serial1LocalPort, DefaultUdpDRALocalPort);
            int remote = ParseUdpPort(Serial1RemotePort, DefaultUdpDRARemotePort);
            try
            {
                UdpClient c = new UdpClient(new IPEndPoint(IPAddress.Loopback, local));
                // Windows: mandar a un puerto donde nadie escucha (DRA.XA todavia cerrado) hace
                // que el Receive siguiente tire WSAECONNRESET. Se desactiva ese comportamiento.
                try { c.Client.IOControl(SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null); } catch { }
                _udpDRARemote = new IPEndPoint(IPAddress.Loopback, remote);
                _udpDRA = c;
            }
            catch (Exception err)
            {
                MessageBox.Show("No se pudo abrir UDP " + local + " para el DRA.XA: " + err.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _udpDRARunning = true;
            _udpDRAThread = new Thread(UdpDRAReceiveLoop) { IsBackground = true, Name = "UdpDRA" };
            _udpDRAThread.Start();
            return true;
        }

        private void CloseUdpDRA()
        {
            _udpDRARunning = false;
            UdpClient c = _udpDRA;
            _udpDRA = null;
            try { c?.Close(); } catch { }      // desbloquea el Receive del hilo
            _udpDRAThread?.Join(300);
        }

        // Envio crudo. Lo usa el hilo escritor (WriterLoop) y, para el ACK, el hilo receptor.
        private void UdpDRASend(string data)
        {
            UdpClient c = _udpDRA;
            if (c == null) return;
            byte[] b = Encoding.UTF32.GetBytes(data + "\n");
            lock (_udpDRASendLock)
            {
                try { c.Send(b, b.Length, _udpDRARemote); } catch { }
            }
        }

        private void UdpDRAReceiveLoop()
        {
            StringBuilder pending = new StringBuilder();
            IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);

            while (_udpDRARunning)
            {
                UdpClient c = _udpDRA;
                if (c == null) break;

                byte[] b;
                try
                {
                    b = c.Receive(ref from);
                }
                catch (ObjectDisposedException) { break; }
                catch (SocketException)
                {
                    if (!_udpDRARunning) break;
                    continue;
                }

                string s = Encoding.UTF32.GetString(b);

                // El DRA confirma cada linea recibida con una 'R' suelta, sin fin de linea.
                // En el serie se pegaba a la linea siguiente ("RAx120"); aca se descarta.
                if (pending.Length == 0 && s == "R") continue;

                pending.Append(s);
                int nl;
                while ((nl = pending.ToString().IndexOf('\n')) >= 0)
                {
                    string line = pending.ToString(0, nl).TrimEnd('\r');
                    pending.Remove(0, nl + 1);
                    if (line.Length > 0) HandleLineFromDRA(line);
                }
                if (pending.Length > 1024) pending.Clear();   // basura sin '\n'
            }
        }

        // Equivalente a ShowData1, pero corre en el hilo receptor: el ACK del brillo sale de
        // inmediato sin pasar por la UI (el DRA espera el ACK 50 ms y la UI puede estar en el
        // busy-wait de WaitForACK hasta 2 s). Solo los textos de debug van a la UI.
        private void HandleLineFromDRA(string line)
        {
            int ax = line.IndexOf("Ax", StringComparison.Ordinal);
            if (ax >= 0)
            {
                if (int.TryParse(line.Substring(ax + 2).Trim(), out int v)) AnalogData = v;
                UdpDRASend("ACK");
                if (DEBUG) PostToUI(() => { DisplayData(1, line); DisplayData(4, "ACK"); });
            }
            else
            {
                SafeWriteLine(serialPort2, line);
                if (DEBUG) PostToUI(() => DisplayData(5, line));
            }
        }

        private void PostToUI(Action a)
        {
            try
            {
                if (IsHandleCreated && !IsDisposed) BeginInvoke(a);
            }
            catch { }
        }
    }
}
