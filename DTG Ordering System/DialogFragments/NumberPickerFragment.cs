using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;

namespace DTG_Ordering_System
{
    public class OnNumberPickEventArgs : EventArgs
    {
        private int quantity;
        private int position;

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public OnNumberPickEventArgs(int quantity, int position)
        {
            this.quantity = quantity;
            this.position = position;
        }
    }

    class NumberPickerFragment : DialogFragment
    {
        private NumberPicker numPicker;
        private Button okButton;
        private Button cancelButton;
        Context thisContext;

        public event EventHandler<OnNumberPickEventArgs> onNumberPickComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int quantity = Arguments.GetInt("quantity");
            int position = Arguments.GetInt("position");

            var view = inflater.Inflate(Resource.Layout.numberPickerFragment, container, false);
            numPicker = view.FindViewById<NumberPicker>(Resource.Id.quantityPicker);
            okButton = view.FindViewById<Button>(Resource.Id.pickerOK);
            cancelButton = view.FindViewById<Button>(Resource.Id.pickerCancel);

            okButton.Click += (object sender, EventArgs e) =>
            {
                numPicker.ClearFocus();
                onNumberPickComplete.Invoke(this, new OnNumberPickEventArgs(numPicker.Value, position));
                HideKeyboard(view, thisContext);
                this.Dismiss();
            };

            cancelButton.Click += (object sender, EventArgs e) =>
            {
                HideKeyboard(view, thisContext);
                this.Dismiss();
            };

            numPicker.MaxValue = 100;
            numPicker.MinValue = 0;
            numPicker.Value = quantity;
            
            numPicker.WrapSelectorWheel = true;
            thisContext = this.Activity.BaseContext;
            
            ShowKeyboard(view, thisContext);

            return view;
        }

        public static void ShowKeyboard(View pView, Context context)
        {     
            pView.RequestFocus();
            InputMethodManager inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.ShowSoftInput(pView, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        public static void HideKeyboard(View pView, Context context)
        {
            InputMethodManager inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.HideSoftInputFromWindow(pView.WindowToken, HideSoftInputFlags.None);
        }
    }
}