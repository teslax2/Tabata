using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabata.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent ();
		}

        protected override bool OnBackButtonPressed()
        {
            Tabata.ViewModel.TabataViewModel.Instance.CloseApp();
            return base.OnBackButtonPressed();
        }
    }
}