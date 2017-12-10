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
using Com.Bumptech.Glide;
using GarlandView.Droid.Details;
using Plugin.CurrentActivity;

namespace GarlandView.Droid.Main.Inner
{
    public class MainInnerItem : Com.Ramotion.Garlandview.Inner.InnerItem
    {

        private View itemView;

        private View mInnerLayout;

        public TextView mHeader;
        public TextView mName;
        public TextView mAddress;
        public ImageView mAvatar;
        public View mAvatarBorder;
        public View mLine;

        private InnerData mInnerData;

        public MainInnerItem(View p0) : base(p0)
        {
            this.itemView = p0;

            mInnerLayout = (p0 as ViewGroup)?.GetChildAt(0);

            mHeader = p0.FindViewById<TextView>(Resource.Id.tv_header);
            mName = p0.FindViewById<TextView>(Resource.Id.tv_name);
            mAddress = p0.FindViewById<TextView>(Resource.Id.tv_address);
            mAvatar = p0.FindViewById<ImageView>(Resource.Id.avatar);
            mAvatarBorder = p0.FindViewById<View>(Resource.Id.avatar_border);
            mLine = p0.FindViewById<View>(Resource.Id.line);

            mInnerLayout.Click += (sender, args) =>
            {
                InnerData itemData = GetItemData();

                if (itemData != null  && CrossCurrentActivity.Current.Activity is MainActivity mainActivity)
                {
                    DetailsActivity.Start(mainActivity,
                        GetItemData().Name, mAddress.Text, GetItemData().AvatarUrl, 
                        itemView, mAvatarBorder);
                }
            };
        }

        protected override View InnerLayout
        {
            get
            {
                return mInnerLayout;
            }
        }

        public InnerData GetItemData()
        {
            return mInnerData;
        }

        public void ClearContent()
        {
            //Glide.Clear(mAvatar);
            mInnerData = null;
        }

        public void SetContent(InnerData data)
        {
            mInnerData = data;

            mHeader.Text = data.Title;
            mName.Text = String.Format("%s %s",
                data.Name,
                itemView.Context.GetString(Resource.String.answer_low));

            mAddress.Text = String.Format("%s %s · %s",
                data.Age,
                mAddress.Context.GetString(Resource.String.years),
                data.Address);

            Glide.With(itemView.Context)
                .Load(data.AvatarUrl)
                .Placeholder(Resource.Drawable.avatar_placeholder)
                .Transform(new ImageCircleTransformation(itemView.Context))
                .Into(mAvatar);
        }

    }
}