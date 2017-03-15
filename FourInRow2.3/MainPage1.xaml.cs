using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments.DataProvider;
using Windows.Networking.Sockets;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage1 : Page
    {
        private SerialDevice serialDevice;

        public MainPage1()
        {
            this.InitializeComponent();
            connect();
            
        }

        public async void connect()
        {
            var selector = SerialDevice.GetDeviceSelector("COM8"); //Get the serial port on port '8'
            var devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Any()) //if the device is found
            {
                
                var deviceInfo = devices.First();
                serialDevice = await SerialDevice.FromIdAsync(deviceInfo.Id);
                //Set up serial device according to device specifications:
                //This might differ from device to device
                var dig = new MessageDialog("CONNECTED!");
                await dig.ShowAsync();

                //while (serialDevice == null)
                //{
                //    await Task.Delay(TimeSpan.FromSeconds(1));
                //} 
                serialDevice.BaudRate = 9600;
                serialDevice.DataBits = 8;
                serialDevice.Parity = SerialParity.None;
                serialDevice.StopBits = SerialStopBitCount.One;
                await listen();
            }

        }


        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            // DataReader reader = new DataReader(serialDevice.InputStream);

            // await reader.LoadAsync(reader.UnconsumedBufferLength);
            //IBuffer b = new Windows.Storage.Streams.Buffer(1);
            ////  reader.ReadBytes(b);
            //await serialDevice.InputStream.ReadAsync(b, 1, InputStreamOptions.None);
            //var dataReader = DataReader.FromBuffer(b);
            //var output = dataReader.ReadString(1);
            //DataReader reader = new DataReader(serialDevice.InputStream);
            //await reader.LoadAsync(100);
            //uint x = reader.UnconsumedBufferLength;
            //textBlock.Text = reader.ReadString(1);
            //reader.ReadString(reader.UnconsumedBufferLength);
        }

        private async void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            DataWriter writer = new DataWriter(serialDevice.OutputStream);
            writer.WriteString(textBox.Text);
            await writer.StoreAsync();
            writer.DetachStream();
        }



        // aExMaEzeO44tZKrQ5chMGNzvFWsogDA4ce9Pm1g/v4i9Pa7vhAGXZuaARsMIL8MP8lL4NJKtE3ixpo4HUUvczQ==

        private async Task listen()
        {
            try
            {
                DataReader reader = new DataReader(serialDevice.InputStream);
                var k = new page2();
                while (true)
                {
                    
                    await reader.LoadAsync(1);
                    uint x = reader.UnconsumedBufferLength;
                    string input_buffer= reader.ReadString(1);
                    textBlock.Text = input_buffer;
                    k.user_turn(Int16.Parse(input_buffer));
                    reader.ReadString(reader.UnconsumedBufferLength);

                }
            }
            catch (Exception)
            {
               
            }
        }
    }


}

//    private SerialDevice serialDevice;
    //    DataReader read;
    //    DataWriter write; 
    //    // private delegate 

    //    public MainPage()
    //    {
    //        this.InitializeComponent();

    //        connect();


    //    }

    //    private void button_Click(object sender, RoutedEventArgs e)
    //    {
    //        writeString();
    //        readString();
    //    }

    //    private async void connect()
    //    {
    //        var selector = SerialDevice.GetDeviceSelector("COM8"); //Get the serial port on port '8'
    //        var devices = await DeviceInformation.FindAllAsync(selector);

    //        if (devices.Any()) //if the device is found
    //        {
    //            var deviceInfo = devices.First();
    //            serialDevice = await SerialDevice.FromIdAsync(deviceInfo.Id);
    //            //Set up serial device according to device specifications:
    //            //This might differ from device to device
    //            var dig = new MessageDialog("CONNECTED!");
    //            await dig.ShowAsync();

    //            //while (serialDevice == null)
    //            //{
    //            //    await Task.Delay(TimeSpan.FromSeconds(1));
    //            //} 
    //            serialDevice.BaudRate = 9600;
    //            serialDevice.DataBits = 8;
    //            serialDevice.Parity = SerialParity.None;
    //            serialDevice.StopBits = SerialStopBitCount.One;

    //            read = new DataReader(serialDevice.InputStream);
    //            write = new DataWriter(serialDevice.OutputStream);
    //        }

    //    }

    //    private async void writeString()
    //    {
    //        write.WriteString(textBox.Text);
    //        await write.StoreAsync();
    //    }

    //    private async void readString()
    //    {


    //        try
    //        {


    //            await read.LoadAsync(1);
    //            textBlock.Text = 

    //        }
    //        catch (Exception e)
    //        {

    //        }

    //        Stream s = serialDevice.InputStream.AsStreamForRead();
    //        int ivLength = read.ReadByte();

    //        string str = System.Text.Encoding.ASCII.GetString(b);
    //        this.textBlock.Text = Convert.ToChar(ivLength).ToString();

    //        /*            if (serialDevice != null)
    //                    {

    //                        await ReadAsync();
    //                    }*/
    //        //int d = s.ReadByte();//Async(b, 0, 1);
    //        //string t = read.ReadString(1);

    //        //             t;//Convert.ToChar(t).ToString();

    //        //read.DetachStream();

    //    }

    //    private async Task ReadAsync()
    //    {
    //        Task<UInt32> loadAsyncTask;

    //        uint ReadBufferLength = 1024;

    //        read.InputStreamOptions = InputStreamOptions.Partial;

    //        // Create a task object to wait for data on the serialPort.InputStream
    //        loadAsyncTask = read.LoadAsync(ReadBufferLength).AsTask();

    //        // Launch the task and wait
    //        UInt32 bytesRead = await loadAsyncTask;
    //        if (bytesRead > 0)
    //        {
    //            textBlock.Text = read.ReadString(bytesRead);
    //        }
    //    }
    //}

