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
using Object = Java.Lang.Object;

namespace GarlandView.Droid.Main.Inner
{
    public class MainInnerAdapter : Com.Ramotion.Garlandview.Inner.InnerAdapter
    {

        private readonly List<InnerData> mData = new List<InnerData>();

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(viewType, parent, false);

            return new MainInnerItem(view.RootView);
        }

        public override int GetItemViewType(int position)
        {
            return Resource.Layout.inner_item;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as MainInnerItem)?.SetContent(mData[position]);
        }

        public override int ItemCount
        {
            get
            {
                return mData.Count;
            }
        }

        public override void OnViewRecycled(Object holder)
        {
            (holder as MainInnerItem)?.ClearContent();
        }

        public void AddData(List<InnerData> innerDataList)
        {
            int size = mData.Count;

            mData.AddRange(innerDataList);

            NotifyItemRangeInserted(size, innerDataList.Count);
        }

        public void ClearData()
        {
            mData.Clear();
            NotifyDataSetChanged();
        }

    }
}