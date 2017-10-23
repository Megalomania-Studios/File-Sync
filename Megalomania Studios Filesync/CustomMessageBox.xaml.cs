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

        #region Constructors

        //standard Messagebox constructor with just an oK-Button
        public CustomMessageBox(string header, string message)
        {
            InitializeComponent();
            Header.Content = header;
            Content.Text = message;
            Bottomleft.Visibility = Visibility.Hidden;
            Bottommiddle.Visibility = Visibility.Hidden;
            Bottomright.Content = "OK";

        }

        public CustomMessageBox(string header, string message, string answerright)
        {
            InitializeComponent();
            Header.Content = header;
            Content.Text = message;
            Bottomleft.Visibility = Visibility.Hidden;
            Bottommiddle.Visibility = Visibility.Hidden;
            Bottomright.Content = answerright;
        }

        public CustomMessageBox(string header, string message, string answerright, string answermiddle)
        {
            InitializeComponent();
            Header.Content = header;
            Content.Text = message;
            Bottomleft.Visibility = Visibility.Hidden;
            Bottommiddle.Content = answermiddle;
            Bottomright.Content = answerright;
        }
       

        //Messagebox constructor with all arguments
        public CustomMessageBox(string header, string message, string answerright, string answermiddle, string answerleft)
        {
            InitializeComponent();          
            Header.Content = header;
            Content.Text = message;
            Bottomleft.Content = answerleft;
            Bottommiddle.Content = answermiddle;
            Bottomright.Content = answerright; 
        }

        #endregion

        #region clickhandlers

        private void Bottomleft_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            this.Close();
        }



        

        private void Bottommiddle_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
      

        private void Bottomright_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {

            }
        }

        
    }





}
