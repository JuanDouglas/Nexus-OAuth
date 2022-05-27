using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Nexus.OAuth.Android.Assets.Models;
using System;

namespace Nexus.OAuth.Android.Assets.Adapters
{
    internal class InfosAdapter : RecyclerView.Adapter
    {
        public Info[] Infos { get; set; }
        public override int ItemCount => Infos.Length;

        public InfosAdapter(Info[] infos)
        {
            Infos = infos ?? throw new ArgumentNullException(nameof(infos));
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            InfoViewHolder viewHolder = (InfoViewHolder)holder;
            Info info = Infos[position];

            viewHolder.Bind(info);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.FromContext(parent.Context);
            View view = (viewType == 0) ? inflater.Inflate(Resource.Layout.item_account_info, parent, false) :
                                          inflater.Inflate(Resource.Layout.item_account_info_disabled, parent, false);

            InfoViewHolder holder = new InfoViewHolder(view);
            return holder;
        }

        public override int GetItemViewType(int position)
            => Infos[position].Disabled ? 1 : 0;
    }

    internal class InfoViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtKey { get; set; }
        public TextView txtValue { get; set; }
        public InfoViewHolder(View view) : base(view)
        {
            txtKey = view.FindViewById<TextView>(Resource.Id.txtItemKey);
            txtValue = view.FindViewById<TextView>(Resource.Id.txtItemValue);
        }

        internal void Bind(Info info)
        {
            txtKey.Text = info.Name;
            txtValue.Text = info.Value;
        }
    }
}