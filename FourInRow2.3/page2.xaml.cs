using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238


namespace App1
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class page2 : Page
    {
        private SerialDevice serialDevice;
        public page2()
        {
            this.InitializeComponent();
            init_dict();
            connect1();
            // minimax_algo();
        }
        private async void connect1()
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

        private async void insert_arduino(int col)
        {
            DataWriter writer = new DataWriter(serialDevice.OutputStream);
            writer.WriteString((col+1).ToString());
            await writer.StoreAsync();
            //writer.DetachStream();
        }

        private async Task listen()
        {
            int counter = 0;
            string buffer_acc = "";
            try
            {
                DataReader reader = new DataReader(serialDevice.InputStream);
                while (true)
                {

                    await reader.LoadAsync(1);
                    string inputBuffer = reader.ReadString(1);
                    buffer_acc += inputBuffer;
                    textBlock.Text = buffer_acc;
                    //var msg = System.String.Format("counter={0}, input={1}", counter, inputBuffer);
                    //var dig = new MessageDialog(msg);
                    //                    await dig.ShowAsync();

                    //if (counter%2 == 0)
                    //{
                    int col = Int16.Parse(inputBuffer);
                        if (get_index(col) != FULL)
                        {
                            user_turn(col);
                        }
                    //}
                    reader.ReadString(reader.UnconsumedBufferLength);

                }
            }
            catch (Exception)
            {

            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        public enum Color
        {
            Empty = 0,
            Red,    // user
            Orange  // Computer
        };

        public const int FULL = 6;

        public static int turn = 0;

        public Color[,] Board = new Color[6, 7];
                static int maxmin = 0;

                int find_max(int[] a)
                {
                    int max = a[0];
                    for (int i=1; i<7; i++)
                    {
                        if (a[i] > max) { max = a[i]; }
                    }
                    return max;
                }
        /*
                        int find_min(int[] a)
                        {
                            int min = a[0];
                            for (int i = 1; i < 7; i++)
                            {
                                if (a[i] < min) { min = a[i]; }
                            }
                            return min;
                        }

                        public int comp_insert(int col, Color color)
                        {
                            var row = get_index(col);
                            if (row==FULL)
                            {
                                return -10;
                            }
                            Board[row, col] = color;
                            return 0;
                        }

                        public int min_algo(int turn_num, int min_turn)
                        {

                            if (turn_num >= min_turn) { return 1500; }

                            int max_turn = -9000;

                            for (int k = 0; k < 7; k++)         // return Minimum!
                            {

                                var illegalMove = comp_insert(k, Color.Red);
                                if (illegalMove < 0) // no more place in that column
                                {
                                    continue;
                                }

                                if (is_winner(k))   // that move leads to win!
                                {
                                    extract_coin(k);
                                    return (-1) * (turn_num);
                                }

                                var a = max_algo(--turn_num, max_turn);
                                turn_num++;
                                if (a > max_turn) { max_turn = a; }
                                extract_coin(k);
                            }

                            return max_turn;

                        }

                        public int max_algo(int turn_num, int max_turn)
                        {
                            if (max_turn >= turn_num) { return -1500; }
                            if (turn_num == 0) { return 0; }

                            int min_turn = 1000;

                            for (int i=0; i<7; i++)             // return Maximum!
                            {
                                var illegalMove = comp_insert(i, Color.Orange);
                                if (illegalMove<0) // no more place in that column
                                {
                                    continue;
                                }

                                if (is_winner(i))   // that move leads to win!
                                {
                                    extract_coin(i);
                                    return turn_num;
                                } 
                                var a = min_algo(--turn_num, min_turn);
                                turn_num++;

                                if (a < min_turn) { min_turn = a; }
                                extract_coin(i);
                            }

                            return min_turn;
                        }

                        private async void computer_turn(int left_turns)
                        {
                            int[] computer_max = new int[7];

                            int max_turn = -9000;
                            int best_col = 0;

                            for (int i = 0; i < 7; i++)
                            {
                                var illegalMove = comp_insert(i, Color.Orange);
                                {
                                    var dig = new MessageDialog("i=" + i);
                                    await dig.ShowAsync();
                                }
                                if (illegalMove < 0) // no more place in that column
                                {
                                    continue;
                                }

                                if (is_winner(i))   // that move leads to win!
                                {
                                    extract_coin(i);
                                    insert_coin(i, Color.Orange);
                                    return;
                                }

                                var a = min_algo(left_turns-1, left_turns);
                                if (a>max_turn) {
                                    a = max_turn;
                                    best_col = i;
                                }
                                extract_coin(i);
                            }

                            insert_coin(best_col, Color.Orange);
                        }
                */

        public int comp_insert(int col, Color color)
        {
            var row = get_index(col);
            if (row == FULL)
            {
                return -10;
            }
            
            Board[row, col] = color;
            return 0;
        }


        private int computer_turn()
        {
            var turns = new int[7];
            var turns_comp2 = new int[7];
 

            for (int i=0; i<7; i++)
            {
                var illegalMove = comp_insert(i, Color.Orange);
                var pref_main = 0;


                if (illegalMove<0)
                {
                    turns[i] = -3000;
                    continue;
                }

                if (is_winner(i))
                {
                    extract_coin(i);
                    return insert_coin(i, Color.Orange);
                }

                for (int j=0; j<7; j++)
                {
                    var illegalMove_user = comp_insert(j, Color.Red);

                    if (illegalMove_user < 0)
                    {
                        continue;
                    }

                    if (is_winner(j))
                    {
                        extract_coin(j);
                        pref_main = -900;
                        break;
                    }

                    for (int i2=0; i2<7; i2++)
                    {
                        var pref = 0;
                        var illegalMove2 = comp_insert(i2, Color.Orange);

                        if (illegalMove2 < 0)
                        {
                            continue;
                        }

                        if (is_winner(i2))
                        {
                            extract_coin(i2);
                            turns_comp2[i2] = 100;
                            break;
                        }

                        for (int j2 = 0; j2 < 7; j2++)
                        {
                            pref = 0;
                            var illegalMove22 = comp_insert(j2, Color.Red);

                            if (illegalMove22 < 0)
                            {
                                continue;
                            }

                            if (is_winner(j2))
                            {
                                extract_coin(j2);
                                pref = -100;
                                break;
                            }

                            extract_coin(j2);
                        }

                        extract_coin(i2);
                        turns_comp2[i2] = pref;
                    }

                    extract_coin(j);

                    int max = find_max(turns_comp2);
                    if (max<0)
                    {
                        pref_main = -700;
                    }

                }

                extract_coin(i);
                turns[i] = pref_main;
            }

            var most_pref = find_max(turns);

            //for (int k=0; k<7; k++)
            //{
            //    var dig = new MessageDialog("turns[" + k + "] = " + turns[k]);
            //    await dig.ShowAsync();
            //}

            if (turns[3]==most_pref)
            {
                return insert_coin(3, Color.Orange);
            }
            if (turns[4]== most_pref)
            {
                if (turns[2] == most_pref)
                {
                    var row2 = get_index(2);
                    var row4 = get_index(4);

                    if (row2 < row4)
                    {
                        return insert_coin(2, Color.Orange);
                    }
                    else
                    {
                        return insert_coin(4, Color.Orange);
                    }
                }
                else
                {
                    return insert_coin(4, Color.Orange);
                }
            }

            if (turns[2] == most_pref)
            {
                return insert_coin(2, Color.Orange);
            }

            if (turns[5] == most_pref)
            {
                if (turns[1] == most_pref)
                {
                    var row1 = get_index(1);
                    var row5 = get_index(5);

                    if (row1 < row5)
                    {
                        return insert_coin(1, Color.Orange);
                    }
                    else
                    {
                        return insert_coin(5, Color.Orange);
                    }
                }
                else
                {
                    return insert_coin(5, Color.Orange);
                }
            }

            if (turns[1] == most_pref)
            {
                return insert_coin(1, Color.Orange);
            }

            if (turns[6] == most_pref)
            {
                if (turns[0] == most_pref)
                {
                    var row0 = get_index(0);
                    var row6 = get_index(6);

                    if (row0 < row6)
                    {
                        return insert_coin(0, Color.Orange);
                    }
                    else
                    {
                        return insert_coin(6, Color.Orange);
                    }
                }
                else
                {
                    return insert_coin(6, Color.Orange);
                }
            }

            if (turns[0] == most_pref)
            {
                return insert_coin(0, Color.Orange);
            }

            return 0;


        }       


        //       public Dictionary<Tuple<int,int>, Ellipse> dict = new Dictionary<Tuple<int, int>, Ellipse>();
        public Dictionary<int, Ellipse> dict = new Dictionary<int, Ellipse>();

        public void init_dict()
        {
            /*  dict.Add(new Tuple<int,int> (0, 0), _00);
              dict.Add(new Tuple<int, int>(0, 1), _01);
              dict.Add(new Tuple<int, int>(0, 2), _02);
              dict.Add(new Tuple<int, int>(0, 3), _03);
              dict.Add(new Tuple<int, int>(0, 4), _04);
              dict.Add(new Tuple<int, int>(0, 5), _05);
              dict.Add(new Tuple<int, int>(0, 6), _06);
              dict.Add(new Tuple<int, int>(0, 0), _10);
              dict.Add(new Tuple<int, int>(1, 1), _11);
              dict.Add(new Tuple<int, int>(1, 2), _12);
              dict.Add(new Tuple<int, int>(1, 3), _13);
              dict.Add(new Tuple<int, int>(1, 4), _14);
              dict.Add(new Tuple<int, int>(1, 5), _15);
              dict.Add(new Tuple<int, int>(1, 6), _16);
              dict.Add(new Tuple<int, int>(2, 0), _20);
              dict.Add(new Tuple<int, int>(2, 1), _21);
              dict.Add(new Tuple<int, int>(2, 2), _22);
              dict.Add(new Tuple<int, int>(2, 3), _23);
              dict.Add(new Tuple<int, int>(2, 4), _24);
              dict.Add(new Tuple<int, int>(2, 5), _25);
              dict.Add(new Tuple<int, int>(2, 6), _26);
              dict.Add(new Tuple<int, int>(3, 0), _30);
              dict.Add(new Tuple<int, int>(3, 1), _31);
              dict.Add(new Tuple<int, int>(3, 2), _32);
              dict.Add(new Tuple<int, int>(3, 3), _33);
              dict.Add(new Tuple<int, int>(3, 4), _34);
              dict.Add(new Tuple<int, int>(3, 5), _35);
              dict.Add(new Tuple<int, int>(3, 6), _36);
              dict.Add(new Tuple<int, int>(4, 0), _40);
              dict.Add(new Tuple<int, int>(4, 1), _41);
              dict.Add(new Tuple<int, int>(4, 2), _42);
              dict.Add(new Tuple<int, int>(4, 3), _43);
              dict.Add(new Tuple<int, int>(4, 4), _44);
              dict.Add(new Tuple<int, int>(4, 5), _45);
              dict.Add(new Tuple<int, int>(4, 6), _46);
              dict.Add(new Tuple<int, int>(5, 0), _50);
              dict.Add(new Tuple<int, int>(5, 1), _51);
              dict.Add(new Tuple<int, int>(5, 2), _52);
              dict.Add(new Tuple<int, int>(5, 3), _53);
              dict.Add(new Tuple<int, int>(5, 4), _54);
              dict.Add(new Tuple<int, int>(5, 5), _55);
              dict.Add(new Tuple<int, int>(5, 6), _56);  */

            dict.Add(0, _00);
            dict.Add(1, _01);
            dict.Add(2, _02);
            dict.Add(3, _03);
            dict.Add(4, _04);
            dict.Add(5, _05);
            dict.Add(6, _06);
            dict.Add(10, _10);
            dict.Add(11, _11);
            dict.Add(12, _12);
            dict.Add(13, _13);
            dict.Add(14, _14);
            dict.Add(15, _15);
            dict.Add(16, _16);
            dict.Add(20, _20);
            dict.Add(21, _21);
            dict.Add(22, _22);
            dict.Add(23, _23);
            dict.Add(24, _24);
            dict.Add(25, _25);
            dict.Add(26, _26);
            dict.Add(30, _30);
            dict.Add(31, _31);
            dict.Add(32, _32);
            dict.Add(33, _33);
            dict.Add(34, _34);
            dict.Add(35, _35);
            dict.Add(36, _36);
            dict.Add(40, _40);
            dict.Add(41, _41);
            dict.Add(42, _42);
            dict.Add(43, _43);
            dict.Add(44, _44);
            dict.Add(45, _45);
            dict.Add(46, _46);
            dict.Add(50, _50);
            dict.Add(51, _51);
            dict.Add(52, _52);
            dict.Add(53, _53);
            dict.Add(54, _54);
            dict.Add(55, _55);
            dict.Add(56, _56);
        }
            
        public bool IsFull()
        {
            for (var i = 0; i < 7; i++)
            {
                if (get_index(i)!=6)
                {
                    return false;
                }
            }

            return true;
        }

        public int get_index(int col)
        {
            for (var i = 0; i < 6; i++)
            {
                if (Board[i,col] == Color.Empty)
                    return i;
            }

            return FULL;
        }

        #region finding winner
        public int is_winning_col(int col, int row)
        {
            Color color = Board[row, col];
            if (row>=3 && color == Board[row-1, col] && color == Board[row-2, col] && color == Board[row-3, col])
            {
                return 1;
            }

            return 0;
        }

        public int is_winning_row(int col, int row)
        {
            Color color = Board[row, col];
            if (col>0 && color == Board[row, col-1])
            {
                return is_winning_row(col-1, row);
            }

            if (col<=3 && color == Board[row, col+1] && color == Board[row, col+2] && color == Board[row, col+3])
            {
                return 1;
            }
            return 0;
        }

        public int is_winning_diagM(int col, int row)
        {
            Color color = Board[row, col];
            if (row < FULL-1 && col > 0 && color == Board[row+1, col - 1])
            {
                return is_winning_diagM(col - 1, row + 1);
            }

            if (row >= 3 && col <= 3 && color == Board[row-1, col + 1] && color == Board[row-2, col + 2] && color == Board[row-3, col + 3])
            {
                return 1;
            }
            return 0;
        }

        public int is_winning_diagS(int col, int row)
        {
            Color color = Board[row, col];
            if (row > 0 && col > 0  && color == Board[row - 1, col - 1])
            {
                return is_winning_diagS(col - 1, row - 1);
            }

            if (row <= 2 && col <= 3 && color == Board[row + 1, col + 1] && color == Board[row + 2, col + 2] && color == Board[row + 3, col + 3])
            {
                return 1;
            }
            return 0;
        }

        public bool is_winner(int col)
        {
            int is_winning = 0;

            int row = get_index(col) - 1;
            is_winning += is_winning_col(col, row);
            is_winning += is_winning_row(col, row);
            is_winning += is_winning_diagM(col, row);
            is_winning += is_winning_diagS(col, row);

            return is_winning>0;
        }

        #endregion


        public async void extract_coin(int col)
        {
            var row = get_index(col);
            if (row <= 0 || row > 6)
            {
                var dig = new MessageDialog("Impossible! column " + col + " was full or empty!");
                await dig.ShowAsync();
                return;
            }

            Board[row-1, col] = Color.Empty;            
        }

        public async void errorMsg(int col)
        {
            var dig = new MessageDialog("Column " + col + 1 + "full!\nPlease try again.");
            await dig.ShowAsync();
        }

        public async void compWins()
        {
            var dig = new MessageDialog("Computer wins! You Lose!! :P");
            await dig.ShowAsync();
        }

        public async void userWins()
        {
            var dig = new MessageDialog("Amazing!! You beat the computer!! =O");
            await dig.ShowAsync();
        }

        public async void itsAtie()
        {
            var dig = new MessageDialog("It's a tie! At least you didn't lose... ");
            await dig.ShowAsync();
        }

        public int insert_coin(int col, Color color)
        {
            var row = get_index(col);
            if (row == FULL) // Column is full.
            {
                errorMsg(col);
                return 0;              
            }
            if (color == Color.Orange)
            {
                insert_arduino(col);

            }
            Board[row, col] = color;
            if (color == Color.Red)
            {
                dict[10 * row + col].Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else       // color == Orange
            {
                dict[10 * row + col].Fill = new SolidColorBrush(Windows.UI.Colors.Orange);
            }

            if (is_winner(col))
            {
               
                if (color == Color.Orange)  // Orange = computer
                {
                    compWins();
                }
                else
                {
                    userWins();
                }

                
                return 2;
            }

            if (IsFull())
            {
                itsAtie();
                return 3;
            }

            return 0;
        }

        public async void user_turn(int col)
        {
            turn++;
            //if (turn % 2 == 1)
            //{
            if (insert_coin(col, Color.Red) > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(8));
                this.Frame.Navigate(typeof(MainPage));
                return;
            }
            //}
            //else
            //{
            //    insert_coin(col, Color.Orange);
            //}
            await Task.Delay(TimeSpan.FromSeconds(1)); //wait two seconds

            turn++;
            if (computer_turn() > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(8));
                this.Frame.Navigate(typeof(MainPage));
                return;
            }
           
        }

    
        private void button_Click0(object sender, RoutedEventArgs e)
        {
            user_turn(0);
        }

        private void button_Click1(object sender, RoutedEventArgs e)
        {
            user_turn(1);
        }

        private void button_Click2(object sender, RoutedEventArgs e)
        {
            user_turn(2);
        }

        private void button_Click3(object sender, RoutedEventArgs e)
        {
            user_turn(3);
        }

        private void button_Click4(object sender, RoutedEventArgs e)
        {
            user_turn(4);
        }

        private void button_Click5(object sender, RoutedEventArgs e)
        {
            user_turn(5);
        }

        private void button_Click6(object sender, RoutedEventArgs e)
        {
            user_turn(6);
        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
