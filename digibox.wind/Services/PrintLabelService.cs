using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace digibox.wind.Services
{
    public class PrintLabelService
    {
        private readonly IMaterialRepository _material;
        private readonly IReplenishDetailRepository _replenishDetail;
        private readonly IDistributorRepository _distributor;

        public PrintLabelService(IMaterialRepository material, IReplenishDetailRepository replenishDetail, IDistributorRepository distributor)
        {
            _material = material;
            _replenishDetail = replenishDetail;
            _distributor = distributor;
        }

        private Bitmap qrcode(string rfidCode)
        {
            var barcode = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 150,
                    Width = 150,
                    Margin = 1
                }
            };
            var result = barcode.Write(rfidCode);
            return result;
        }

        private Bitmap barcode(string rfidCode)
        {
            var barcode = new BarcodeWriter()
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 150,
                    Margin = 1
                }
            };
            var result = barcode.Write(rfidCode);
            return result;
        }

        public void PrintLabel(Guid id)
        {
            //Data

            var rdetail = _replenishDetail.FindById(id); 
            var material = _material.FindById(rdetail.materialid);
            var distributor = _distributor.FindById(material.distributor);
            //qrcode data
            var qrcode = this.qrcode(rdetail.rfidcode);
            System.Windows.Controls.Image qrimage = new System.Windows.Controls.Image();
            using (var stream = new MemoryStream())
            {
                qrcode.Save(stream, ImageFormat.Png);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                stream.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = stream;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                //QRCode.Source = bi; //A WPF Image control
                qrimage.Source = bi;
                qrimage.Height = 100.0;
                qrimage.Width= 100.0;
            }

            //barcode
            var barcode = this.barcode(rdetail.rfidcode);
            System.Windows.Controls.Image barImage = new System.Windows.Controls.Image();
            using (var stream = new MemoryStream())
            {
                barcode.Save(stream, ImageFormat.Png);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                stream.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = stream;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                //QRCode.Source = bi; //A WPF Image control
                barImage.Source = bi;
                barImage.Height = 100.0;
                barImage.Width = 200.0;
            }


            DockPanel dp = new DockPanel();
            dp.LastChildFill = true;
            dp.Margin = new Thickness(10);

            //Header
            TextBlock tb = new TextBlock();
            tb.Text = "UNILEVER INDONESIA";
            tb.FontSize = 15;
            tb.FontWeight = FontWeights.Bold;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.Margin = new Thickness(0, 0, 0, 5);

            //header border
            Border brd = new Border();
            brd.BorderThickness = new Thickness(1, 1, 1, 1);
            brd.BorderBrush = new SolidColorBrush(Colors.Black);
            brd.Child = tb;
            dp.Children.Add(brd);
            DockPanel.SetDock(brd, Dock.Top);

            //Content panel
            DockPanel spc = new DockPanel();
            spc.LastChildFill = true;
            brd = new Border();
            brd.BorderThickness = new Thickness(1, 0, 1, 1);
            brd.BorderBrush = new SolidColorBrush(Colors.Black);
            brd.Child = spc;
            dp.Children.Add(brd);

            //left panel
            StackPanel lspc = new StackPanel();
            lspc.Orientation = Orientation.Vertical;
            brd = new Border();
            brd.BorderThickness = new Thickness(0, 0, 1, 0);
            brd.BorderBrush = new SolidColorBrush(Colors.Black);
            brd.Child = lspc;
            brd.Width = 250;
            spc.Children.Add(brd);

            //logo panel
            DockPanel logodoc = new DockPanel();
            logodoc.LastChildFill = true;
            logodoc.Height = 229;
            logodoc.HorizontalAlignment = HorizontalAlignment.Center;
            logodoc.VerticalAlignment = VerticalAlignment.Center;
            tb = new TextBlock();
            System.Windows.Controls.Image logoimage = new System.Windows.Controls.Image();
            var path = AppDomain.CurrentDomain.BaseDirectory;
            Uri resourceUri = new Uri(path+"/Images/U11.png", UriKind.Absolute);
            logoimage.Source = new BitmapImage(resourceUri);
            tb.Text = "LOGO";
            logoimage.VerticalAlignment = VerticalAlignment.Center;
            logoimage.HorizontalAlignment = HorizontalAlignment.Center;
            logoimage.Width = 100;
            logoimage.Height = 100;
            logodoc.Children.Add(logoimage);
            brd = new Border();
            brd.BorderThickness = new Thickness(0, 0, 0, 0);
            brd.BorderBrush = new SolidColorBrush(Colors.Black);
            brd.Child = logodoc;
            DockPanel.SetDock(brd, Dock.Top);
            lspc.Children.Add(brd);

            //qr code panel
            logodoc = new DockPanel();
            logodoc.LastChildFill = true;
            qrimage.HorizontalAlignment = HorizontalAlignment.Center;
            qrimage.VerticalAlignment = VerticalAlignment.Center;
            qrimage.Height = 100;
            qrimage.Width = 200;
            qrimage.Margin = new Thickness(5);
            logodoc.Children.Add(qrimage);
            brd = new Border();
            brd.BorderThickness = new Thickness(0, 1, 0, 0);
            brd.BorderBrush = new SolidColorBrush(Colors.Black);
            brd.Child = logodoc;
            lspc.Children.Add(brd);

            //right panel
            DockPanel rspc = new DockPanel();
            rspc.LastChildFill = true;
            brd = new Border();
            brd.BorderThickness = new Thickness(0, 0, 0, 0);
            brd.BorderBrush = new SolidColorBrush(Colors.Black);
            brd.Child = rspc;
            spc.Children.Add(brd);

            //material panel
            StackPanel materialpanel = new StackPanel();
            materialpanel.Height = 200;
            materialpanel.Orientation = Orientation.Vertical;
            materialpanel.Margin = new Thickness(10);

            StackPanel infopanel = new StackPanel();
            infopanel.Orientation = Orientation.Horizontal;
            tb = new TextBlock();
            tb.Text = "Material Code";
            tb.Width = 100;
            infopanel.Children.Add(tb);
            tb = new TextBlock();
            tb.Text = material.partno;
            infopanel.Children.Add(tb);
            materialpanel.Children.Add(infopanel);

            infopanel = new StackPanel();
            infopanel.Orientation = Orientation.Horizontal;
            tb = new TextBlock();
            tb.Text = "Material Name";
            tb.Width = 100;
            infopanel.Children.Add(tb);
            tb = new TextBlock();
            tb.Text = material.name;
            infopanel.Children.Add(tb);
            materialpanel.Children.Add(infopanel);

            infopanel = new StackPanel();
            infopanel.Orientation = Orientation.Horizontal;
            tb = new TextBlock();
            tb.Text = "Distributed by";
            tb.Width = 100;
            infopanel.Children.Add(tb);
            tb = new TextBlock();
            tb.Text = distributor.name;
            infopanel.Children.Add(tb);
            materialpanel.Children.Add(infopanel);

            infopanel = new StackPanel();
            infopanel.Orientation = Orientation.Horizontal;
            tb = new TextBlock();
            tb.Text = "Description";
            tb.Width = 100;
            infopanel.Children.Add(tb);
            tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Text = material.description;
            infopanel.Children.Add(tb);
            materialpanel.Children.Add(infopanel);

            brd = new Border();
            brd.BorderThickness = new Thickness(0, 0, 0, 0);
            brd.BorderBrush = new SolidColorBrush(Colors.Black);
            brd.Child = materialpanel;
            rspc.Children.Add(brd);
            DockPanel.SetDock(brd, Dock.Top);

            //barcode
            logodoc = new DockPanel();
            logodoc.LastChildFill = true;
            tb = new TextBlock();
            tb.Text = "BARCODE";

            barImage.HorizontalAlignment = HorizontalAlignment.Center;
            barImage.VerticalAlignment = VerticalAlignment.Center;
            barImage.Margin = new Thickness(5);
            barImage.Height = 100;
            barImage.Width = 200;
            logodoc.Children.Add(barImage);
            brd = new Border();
            brd.BorderThickness = new Thickness(0, 1, 0, 0);
            brd.BorderBrush = new SolidColorBrush(Colors.Black);
            brd.Child = logodoc;
            brd.VerticalAlignment = VerticalAlignment.Center;
            rspc.Children.Add(brd);

            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintTicket.PageOrientation = PageOrientation.Landscape;
            printDlg.PrintTicket.PageBorderless = PageBorderless.Borderless;
            printDlg.PrintTicket.PageMediaSize = new System.Printing.PageMediaSize(System.Printing.PageMediaSizeName.ISOA6);

            printDlg.PrintVisual(dp, "Label");

        }


        public static void Print(string rfidcode)
        {
            PrintDialog printDlg = new PrintDialog();
            StackPanel sp = new StackPanel();
            sp.HorizontalAlignment = HorizontalAlignment.Left;
            sp.Margin = new Thickness(10);

            TextBlock tb = new TextBlock();
            tb.Text = rfidcode;
            tb.FontSize = 10;
            tb.FontWeight = FontWeights.Bold;
            tb.Margin = new Thickness(0, 0, 0, 5);
            sp.Children.Add(tb);

            printDlg.PrintVisual(sp, "RFIDCODE");
        }
    }
}
