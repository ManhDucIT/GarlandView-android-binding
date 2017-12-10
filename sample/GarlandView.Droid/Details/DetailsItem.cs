using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace GarlandView.Droid.Details
{
    public class DetailsItem : RecyclerView.ViewHolder
    {

        private TextView tvTitle;
        private TextView tvText;

        public DetailsItem(View itemView) : base(itemView)
        {
            tvTitle = ItemView.FindViewById<TextView>(Resource.Id.tv_title);
            tvText = ItemView.FindViewById<TextView>(Resource.Id.tv_text);
        }

        public void SetContent(DetailsData data)
        {
            tvTitle.Text = data.Title;
            tvText.Text = data.Text;
        }

    }
}