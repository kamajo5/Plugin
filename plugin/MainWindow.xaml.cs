using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;

namespace plugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        string url_image;
        List<string> linia = new List<string>();
        List<string> fullname = new List<string>();
        Bitmap ba;
        private void Wczytaj_obraz(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = false;
            file.Filter = "Image file (*.jpg)|*.jpg";
            file.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (file.ShowDialog() == true)
            {
                url_image = file.FileName;
                ba = new Bitmap(url_image);
                mainPictureBox.Image = ba;
            }
        }

        private void Zapisz_obraz(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pliki jpg | *.jpg";
            if (saveFileDialog.ShowDialog(this) == true)
            {
                mainPictureBox.Image.Save(saveFileDialog.FileName);
            }
        }

        private void Koniec(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Check_plugin(object sender, RoutedEventArgs e)
        {
            opcje.Items.Clear();
            find_dll();
            for (int i = 0; i < linia.Count; i++)
            {
                Assembly dll = Assembly.LoadFile(fullname[i]);
                Type[] typeArr = dll.GetTypes();

                foreach (Type typ in typeArr)
                {
                    MethodInfo[] myMethods = typ.GetMethods(BindingFlags.Static | BindingFlags.Public);
                    foreach (MethodInfo metoda in myMethods)
                    {
                        MenuItem item = new MenuItem();
                        item.Header = metoda.Name;
                        item.Name = metoda.Name;
                        item.Click += opcje_Click;
                        item.Tag = fullname[i].ToString();
                        opcje.Items.Add(item);
                    }
                }
            }
            linia.Clear();
            fullname.Clear();
        }

        private void INFO(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Wczytywanie pluginów \n Kamil Jonaszko");
        }

        private void find_dll()
        { 
            string[] napis;
            String fullPath;
            fullPath = System.IO.Path.GetFullPath("plugin.exe");
            fullPath = fullPath.Remove(fullPath.Length-10,10);
            napis = File.ReadAllLines(@"memory.txt");
            for (int i = 0; i < napis.Length; i++)
            {
                linia.Add(napis[i]);
            }
            DirectoryInfo t;
            t = new DirectoryInfo(fullPath);
            foreach (var fi in t.GetFiles("*.dll*"))
            {
                if (match(fi.Name))
                {
                    linia.Add(fi.Name);
                }
                fullname.Add(fi.FullName);
            }
            File.WriteAllLines(@"memory.txt", linia);
        }
        private bool match( string pomnazwa)
        {
            string[] napis;

            napis = File.ReadAllLines(@"memory.txt");
            for (int i=0;i<napis.Length;i++)
            {
                if (linia[i] == pomnazwa)
                    return false;
            }
            return true;
        }

        private void opcje_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var t = (sender as MenuItem);
                Assembly myAssembly = Assembly.LoadFile(t.Tag.ToString());
                Type type = myAssembly.GetType(t.Name + ".Efekty");
                MethodInfo myMethod = type.GetMethod(t.Name);
                ba = (Bitmap)myMethod.Invoke(null, new object[] { ba });
                mainPictureBox.Image = ba;
            }
            catch
            {
                MessageBox.Show("Brak obrazka");
            }
        }
    }
}
