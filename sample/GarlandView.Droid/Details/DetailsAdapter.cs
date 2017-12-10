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
    public class DetailsAdapter : RecyclerView.Adapter
    {

        private List<DetailsData> lstData;

        public DetailsAdapter(List<DetailsData> lstData)
        {
            this.lstData = lstData;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.details_item, parent, false);

            return new DetailsItem(view.RootView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as DetailsItem)?.SetContent(lstData[position]);
        }

        public override int ItemCount
        {
            get { return lstData.Count; }
        }

    }
}