using System;
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
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Client;
using System.Text;
using Microsoft.Azure.Devices.Shared;
using System.Threading.Tasks;



namespace SignalGeneratorWPF

{ 


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();


    }



    private void button_Click(object sender, RoutedEventArgs e)
    {

        string connectionString = txt_connstring.Text.ToString();
        string deviceID = txt_devid.Text.ToString();

        try
        {
            DeviceClient klient = DeviceClient.CreateFromConnectionString(connectionString, deviceID);
            klient.OpenAsync();
            rad_connected.IsChecked = true;
            rad_connissue.IsChecked = false;

            txt_connlog.Text = DateTime.Now.TimeOfDay.ToString() + " CONNECTED! \n" + txt_connlog.Text;
        }
        catch
        {
            txt_connlog.Text = DateTime.Now.TimeOfDay.ToString() + " CONN ERROR\n" + txt_connlog.Text;
            rad_connissue.IsChecked = true;
            rad_connected.IsChecked = false;


        }




    }

    public void genmsg()
    {
        Random random = new Random();
        random.NextDouble();
        Double max = Convert.ToDouble(txt_prmax.Text.ToString());
        Double min = Convert.ToDouble(txt_prmin.Text.ToString());

        try
        {
            string connectionString = txt_connstring.Text.ToString();
            string deviceID = txt_devid.Text.ToString();
            DeviceClient klient = DeviceClient.CreateFromConnectionString(connectionString, deviceID);
            var komunikat = new
            {
                timestamp = DateTime.Now,
                parr = random.NextDouble() * (max - min) + min,
                par1 = txt_p1val.Text.ToString(),
                par2 = txt_p2val.Text.ToString(),
                par3 = txt_p3val.Text.ToString(),
                par4 = txt_p4val.Text.ToString()
            };

            var komunikat_nodate = new
            {
                parr = random.NextDouble() * (max - min) + min,
                par1 = txt_p1val.Text.ToString(),
                par2 = txt_p2val.Text.ToString(),
                par3 = txt_p3val.Text.ToString(),
                par4 = txt_p4val.Text.ToString()
            };

            var messageString = JsonConvert.SerializeObject(komunikat);

            if (chk_timestampincl.IsChecked.Value == true)
            {

                messageString = JsonConvert.SerializeObject(komunikat);
            }
            else
            {
                messageString = JsonConvert.SerializeObject(komunikat_nodate);
            }

            var transmisja = new Message(Encoding.ASCII.GetBytes(messageString));
            klient.SendEventAsync(transmisja);
            txt_connlog.Text = DateTime.Now.TimeOfDay.ToString() + " MSG SENT\n" + txt_connlog.Text;

        }
        catch
        { txt_connlog.Text = DateTime.Now.TimeOfDay.ToString() + " FAILED\n" + txt_connlog.Text; }

    }

    async Task RUNGENERATOR()
    {
        while (rungen == true)
        {
            genmsg();
            int delay = Convert.ToInt32(txt_period.Text.ToString());
            await Task.Delay(delay);
        }
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
        genmsg();
    }

    public bool rungen = false;

    private void btn_startgen_Click(object sender, RoutedEventArgs e)
    {
        rungen = true;
        RUNGENERATOR();

    }

    private void btn_stopgen_Click(object sender, RoutedEventArgs e)
    {
        rungen = false;

    }
}
}
