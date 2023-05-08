using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Windows.Threading;

namespace Socket_4I
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Socket socket = null;
        DispatcherTimer dTimer = null;
        public MainWindow()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress local_address = IPAddress.Any;
            IPEndPoint local_endpoint = new IPEndPoint(local_address, 10000);  //crea una variabile che conterrà l'ip di destinazione, cioè quello del dispositivo e la porta in uso

            socket.Bind(local_endpoint); //associa la socket alla variabile appena creata

            socket.Blocking = false;
            socket.EnableBroadcast = true; //fa si che la socket permetta di inviare pacchetti

            dTimer = new DispatcherTimer(); //crea un dispatcher timer

            dTimer.Tick += new EventHandler(aggiornamento_dTimer); //crea un evento nel corrente tick, i tick sono gli intervalli salvati in una label 
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250); //imposta l'intervallo per i tick su 250 millisecondi
            dTimer.Start(); //avvia il timer

        }

        private void aggiornamento_dTimer(object sender, EventArgs e)//metodo chiamato ogni volta che il timer si resetta
        {
            int nBytes = 0;

            if ((nBytes = socket.Available) > 0)
            {
                //ricezione dei caratteri in attesa
                byte[] buffer = new byte[nBytes];

                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);//crea una variabile che indica quale sarà il punto di destinazione del messaggio

                nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);

                string from = ((IPEndPoint)remoteEndPoint).Address.ToString();//salva in una stringa l'indirizzo/numero di porta del mittente

                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);//salva in una stringa il messaggio ricevuto


                lstMessaggi.Items.Add(from+" : "+messaggio);//aggiunge nella lista lstMessaggi il messaggio con il suo mittente come stringa

            }
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)//metodo che viene chiamato al premere del button
        {
            string thisip = Dns.GetHostName();//ottiene il nome del localhost

            IPAddress remote_address = Dns.GetHostByName(thisip).AddressList[0];//inserisce in una variabile ipaddress l'ip del localhost tramite il suo nome

            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 11000);//imposta l'indirizzo e la porta a cui inviare il messaggio

            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);//prende il text nella textbox e lo mette nella variabile che verrà trasmessa al remote endpoint

            socket.SendTo(messaggio, remote_endpoint);//invia il messaggio al remote endpoint
        }
    }
}
