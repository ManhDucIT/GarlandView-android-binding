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
using GarlandView.Droid.Details;

namespace GarlandView.Droid.Profile
{
    public class ProfileItem : RecyclerView.ViewHolder
    {

        private TextView tvTitle;
        private TextView tvText;

        public ProfileItem(View itemView) : base(itemView)
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