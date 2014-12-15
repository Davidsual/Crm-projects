using System.Windows;
using Reply.Seat.DinamichePromozionali.Test.DinamichePromozionaliServicesWsdl;
using System.Threading;
using System;
using System.Net;
using System.Windows.Threading;

namespace Reply.Seat.DinamichePromozionali.Test
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TestServiceSeat : Window
    {
        public TestServiceSeat()
        {
            InitializeComponent();
        }
        private DinamichePromozionaliServices _service;

        public DinamichePromozionaliServices Service
        {
            get
            {
                if (_service == null)
                    _service = new DinamichePromozionaliServices();
                _service.Proxy = new WebProxy() { UseDefaultCredentials = true };
                _service.Url = this.txtUrlServer.Text;
                //"http://localhost:60908/DinamichePromozionaliServices.asmx";
                //_service.Url = "http://localhost/DinamichePromozionali/DinamichePromozionaliServices.asmx";
                _service.Timeout = Timeout.Infinite;
                return _service;
            }
        }
        /// <summary>
        /// Click Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.Dispatcher.BeginInvoke(DispatcherPriority.Background,new Action(() => this.txtResult.Text = string.Empty)); 
            GetStatusCampaignResult _result = Service.GetStatusCampaign(this.txtIdCall.Text.Trim().ToLower(),
                                    this.txtCallType.Text.Trim().ToLower(),
                                    this.txtCryptedCode.Text.Trim().ToLower(),
                                    this.txtPhoneNumber.Text.Trim().ToLower(),
                                    this.txtIdOperator.Text.Trim().ToLower());

            this.txtResult.Text = _result.ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ///managementPrize

            GetManagementPrizeResult _result = this.Service.GetManagementPrize(new Guid(this.txtIdChiamanteCampagna.Text), this.txtCryptedCode.Text.Trim().ToLower(), this.txtPhoneNumber.Text.Trim().ToLower());
            this.txtResult.Text = _result.ToString();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ///Set privacy
            SetPrivacyResult _result = this.Service.SetPrivacy(new Guid(this.txtIdChiamanteCampagna.Text), "SI", this.txtCryptedCode.Text.ToLower(), this.txtPhoneNumber.Text.Trim().ToLower());
            this.txtResult.Text = _result.ToString();
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.txtResult.Text = string.Empty)); 
            ///GetStatusCampaing
            GetChiamanteCampagnaResult _result = this.Service.GetChiamanteCampagna(new Guid(this.txtIdChiamanteCampagna.Text.Trim()));
            this.txtResult.Text = _result.ToString();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.txtUrlServer.Text = "http://localhost:60908/DinamichePromozionaliServices.asmx";
            this.txtUrlServer.Text = Reply.Seat.DinamichePromozionali.Test.Properties.Settings.Default.CrmUrl;
        }

        private void btnOnlyPrivacy_Click(object sender, RoutedEventArgs e)
        {
            ///Set privacy
            SetOnlyPrivacyResult _result = this.Service.SetOnlyPrivacy(new Guid(this.txtIdChiamanteCampagna.Text), "NO");
            this.txtResult.Text = _result.ToString();
        }
    }
}
