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
using System.Windows.Shapes;

namespace Megalomania_Studios_Filesync
{
    /// <summary>
    /// Interaktionslogik für CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string header, string message, string answerleft, string answermiddle, string answerright)
        {
            InitializeComponent();          
            Header.Content = header;
            Content.Text = message;
            /*switch (answer)
            {
                case Answer.OK:
                    Bottomright.Visibility = Visibility.Visible;
                    Bottommiddle.Visibility = Visibility.Hidden;
                    Bottomleft.Visibility = Visibility.Hidden;
                    break;
                case Answer.OKNO:
                    Bottomright.Visibility = Visibility.Visible;
                    Bottommiddle.Visibility = Visibility.Visible;
                    Bottomleft.Visibility = Visibility.Hidden;
                    break;
                case Answer.OKNOABORT:
                    Bottomright.Visibility = Visibility.Visible;
                    Bottommiddle.Visibility = Visibility.Visible;
                    Bottomleft.Visibility = Visibility.Visible;
                    break;

            }*/

            Bottomleft.Content = answerleft;
            Bottommiddle.Content = answermiddle;
            Bottomright.Content = answerright;
            
        }

        public void show(string header, string message, Answer answer)
        {
            


            
            
        }

        #region clickhandlers
        private void Bottomleft_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            this.Close();
        }



        #endregion

        private void Bottommiddle_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
      
        public Answer answer;

        private void Bottomright_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
    public enum Answer
    {
        OK = 0,
        OKNO = 1,
        OKNOABORT = 2
          
    }

   


}
