using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Threading;

using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

namespace pingServer
{
    public partial class Form1 : Form
    {

        public static string data = null;

        void StartServer()
        {
            this.txtStatus.Text += "Server init... \r\n";

            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.

            IPAddress ipAddress = (IPAddress)this.cmbIP.SelectedItem;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, int.Parse(this.txtServerPort.Text));

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                this.txtStatus.Text += "Server init... OK \r\n";

                // Start listening for connections.
                while (true)
                {
                    this.txtStatus.Text += ("Waiting for a connection... \r\n");
                    // Program is suspended while waiting for an incoming connection.
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.                    
                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);                                        

                    // Show the data on the console.
                    this.txtStatus.Text += "Response Client: " + data + "\r\n";

                    // Echo the data back to the client.
                    byte[] msg = Encoding.ASCII.GetBytes("PONG!");

                    handler.Send(msg);
                    this.txtStatus.Text += "Server Send: PONG! \r\n";

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    
                    if (data == "PING!")
                    {
                        break;
                    }
                                        
                }
            }
            catch (Exception e)
            {
                this.txtStatus.Text += "$ ERROR: \r\n";
                this.txtStatus.Text += e.Message;
            }        
        }


        void StartClient()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.                
                IPAddress ipAddress = IPAddress.Parse(this.txtIPServer.Text);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, int.Parse(this.txtPortConnect.Text));

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    this.txtStatus.Text += "Try connect... \r\n";
                    this.Refresh();
                    sender.Connect(remoteEP);
                    this.txtStatus.Text += "Server connected! \r\n";

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes("PING!");

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);
                    this.txtStatus.Text += "Client Send: PING! \r\n";

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    this.txtStatus.Text += "Server Response:" + Encoding.ASCII.GetString(bytes, 0, bytesRec) + "\r\n";

                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    this.txtStatus.Text += "$ ERROR: \r\n";
                    this.txtStatus.Text += ane.Message;
                }
                catch (SocketException se)
                {
                    this.txtStatus.Text += "$ ERROR: \r\n";
                    this.txtStatus.Text += se.Message;
                }
                catch (Exception e)
                {
                    this.txtStatus.Text += "$ ERROR: \r\n";
                    this.txtStatus.Text += e.Message;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        

        public Form1()
        {
            InitializeComponent();
            

            // Get host name
            String strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            this.txtStatus.Text = "";
            this.txtStatus.Text += "Your IP's: \r\n";
            // Enumerate IP addresses
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                this.txtStatus.Text += " > " + ipaddress + "\r\n";
                this.cmbIP.Items.Add(ipaddress);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //  Thread th = new Thread(this.StartServer);
            //  th.Start();
            this.StartServer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //            Thread th = new Thread(this.StartClient);
            //            th.Start();
            this.StartClient();
        }
    }
}
