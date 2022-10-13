using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;

namespace PPT_Controller_Lite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // COM reference tracker instance for counting
        private COMReferenceTracker comRef = new COMReferenceTracker();
        private dynamic presentation;

        private string load_path;

        //System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        ClientWebSocket clientSocket = new ClientWebSocket();
        private string ip_address;
        private Regex ip_regex = new Regex("\\b(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9]))\\b:[0-9]+", RegexOptions.IgnoreCase);

        // Data bind for server status
        private DataBinds DataBind = new DataBinds() { Server_Status = "Enter IP to Connect"};


        public MainWindow()
        {
            InitializeComponent();

            // Data bind
            DataContext = this.DataBind;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please contact author for help", "Help");
        }
        private void File_Exit_Click(object sender, RoutedEventArgs e)
        {
            comRef.Dispose();
            Application.Current.Shutdown();
        }


        /// <PPT Logic>
        /// All methods related to ppt file operation
        /// </PPT Logic>
        dynamic T(dynamic obj)
        {
            return comRef.T(obj);
        }

        private void File_Open_Click(object sender, RoutedEventArgs e)
        {
            // Clear Previous one ppt memory
            comRef.Dispose();

            // Choose PPT file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PowerPoint files (*.pptx;*.ppt;*.pptm;*.ppsx;*.pps;*.ppsm)|*.pptx;*.ppt;*.pptm;*.ppsx;*.pps;*.ppsm|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                load_path = openFileDialog.FileName;

            // Open PPT
            dynamic ppt = T(PowerPointHelper.CreatePowerPointApplication());
            ppt.Visible = true;
            dynamic presentations = T(ppt.Presentations);
            this.presentation = T(presentations.Open(load_path));
            //T(presentation.SlideShowSettings).Run();
        }

        private void Window_Left_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                T(T(this.presentation.SlideShowWindow).View).Previous();
            }
            catch (Exception ex)
            {
                if (ex is System.Runtime.InteropServices.COMException)
                {
                    MessageBox.Show("Please make sure ppt is in reader or presentation mode", "Warning");
                }
                else if (ex is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    MessageBox.Show("Please make sure select ppt file by File -> Open routine", "Warning");
                }
            }
        }

        private void Socket_Left_cmd()
        {
            try
            {
                T(T(this.presentation.SlideShowWindow).View).Previous();
            }
            catch (Exception ex)
            {
                if (ex is System.Runtime.InteropServices.COMException)
                {
                    MessageBox.Show("Please make sure ppt is in reader or presentation mode", "Warning");
                }
                else if (ex is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    MessageBox.Show("Please make sure select ppt file by File -> Open routine", "Warning");
                }
            }
        }

        private void Window_Right_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                T(T(this.presentation.SlideShowWindow).View).Next();
            }
            catch (Exception ex)
            {
                if (ex is System.Runtime.InteropServices.COMException)
                {
                    MessageBox.Show("Please make sure ppt is in reader or presentation mode", "Warning");
                }
                else if (ex is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    MessageBox.Show("Please make sure select ppt file by File -> Open routine", "Warning");
                }
            }
        }

        private void Socket_Right_cmd()
        {
            try
            {
                T(T(this.presentation.SlideShowWindow).View).Next();
            }
            catch (Exception ex)
            {
                if (ex is System.Runtime.InteropServices.COMException)
                {
                    MessageBox.Show("Please make sure ppt is in reader or presentation mode", "Warning");
                }
                else if (ex is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    MessageBox.Show("Please make sure select ppt file by File -> Open routine", "Warning");
                }
            }

        }

        protected override void OnClosed(EventArgs e)
        {
            comRef.Dispose();
            //this.clientSocket.Close();
            try
            {
                this.clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            catch (System.InvalidOperationException)
            {

            }
            Application.Current.Shutdown();
        }

        private void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DataBind.Server_Status = "Connecting...";
            this.ip_address = IP_input.Text;

            if (ip_regex.IsMatch(this.ip_address))
            {
                try
                {
                    //string[] ip_port = this.ip_address.Split(':');
                    StartClient(this.ip_address);
                    //StartClient(ip_port);
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("Connection is on, please disconnect first", "Warning");
                }
            }
            else
            {
                MessageBox.Show("Incorrect IP address", "Warning");
                this.DataBind.Server_Status = "Incorrect IP";
            }
        }

        private async void StartClient(string ip_address)
        {
            //var client = new TcpClient();
            try
            {
                await this.clientSocket.ConnectAsync(new Uri("ws://" + ip_address), CancellationToken.None);
                //await this.clientSocket.ConnectAsync(ip_port[0], Int32.Parse(ip_port[1]));

                if (true)//(clientSocket.Connected)
                {
                    //var networkStream = this.clientSocket.GetStream();
                    this.DataBind.Server_Status = "Connected";
                    

                    //byte[] msg = Encoding.ASCII.GetBytes("This is a test");
                    //networkStream.Write(msg, 0, msg.Length);

                    while (true)//(clientSocket.Connected)
                    {
                        //byte[] buffer = new byte[clientSocket.ReceiveBufferSize];
                        //int read = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        byte[] incomingData = new byte[1024];
                        WebSocketReceiveResult result = await this.clientSocket.ReceiveAsync(new ArraySegment<byte>(incomingData), CancellationToken.None);
                        string _msg = Encoding.UTF8.GetString(incomingData, 0, result.Count);
                        Console.WriteLine(_msg);

                        if (_msg == "Next")
                        {
                            Socket_Right_cmd();
                        }
                        if (_msg == "Previous")
                        {
                            Socket_Left_cmd();
                        }
                        /*
                        if (read > 0)
                        {
                            byte[] after_cut = new byte[read];
                            Buffer.BlockCopy(buffer, 0, after_cut, 0, read);
                            string _msg = Encoding.UTF8.GetString(after_cut);

                            Console.WriteLine(_msg);
                            
                            if (_msg == "Next")
                            {
                                Socket_Right_cmd();
                            }
                            if (_msg == "Previous")
                            {
                                Socket_Left_cmd();
                            }
                        }*/
                    }
                    this.DataBind.Server_Status = "Disconnected";
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Net.Sockets.SocketException)
                {
                    MessageBox.Show("Already connected to server", "Note");
                }
                else if (ex is System.ObjectDisposedException)
                {

                }
            }
                
        }

        private void Disconnect_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DataBind.Server_Status = "Disconnected";
            try
            {
                this.clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            catch (System.InvalidOperationException)
            {

            }
            this.clientSocket = new ClientWebSocket();//new System.Net.Sockets.TcpClient();
        }
    }

    public class DataBinds : INotifyPropertyChanged
    {
        private string server_status;
        public string Server_Status
        {
            get { return this.server_status; }
            set
            {
                if (this.server_status != value)
                {
                    this.server_status = value;
                    this.NotifyPropertyChanged("Server_Status");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
    
}
