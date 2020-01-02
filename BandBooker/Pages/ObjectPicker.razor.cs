using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace BandBooker.Pages
{
    public class ObjectPickerCode<TItem> : ComponentBase
    {

        [Parameter]
        public string ItemType { get; set; }

        [Parameter]
        public string ItemTypePlural { get; set; }

        [Parameter]
        public string TextPropertyName { get; set; }

        [Parameter]
        public string ValuePropertyName { get; set; }

        [Parameter]
        public List<TItem> AllItems { get; set; }

        [Parameter]
        public List<TItem> SelectedItems { get; set; }

        protected TItem SelectedItem { get; set; }

        protected bool addSelectedItemButtonDisabled = true;
        protected bool removeSelectedItemButtonDisabled = true;

        protected string ItemValue(TItem item)
        {
            return item.GetType()
                .GetProperty(ValuePropertyName)
                .GetValue(item, null)
                .ToString();
        }

        protected string ItemText(TItem item)
        {
            return item.GetType()
                .GetProperty(TextPropertyName)
                .GetValue(item, null)
                .ToString();
        }

        protected override void OnParametersSet()
        {
            if (AllItems.Count > 0)
            {
                // remove the items that exist in SelectedItems
                foreach (var item in SelectedItems)
                {
                    var id = item.GetType()
                    .GetProperty(ValuePropertyName)
                    .GetValue(item, null)
                    .ToString();

                    var ItemFromAllItems =
                    (from x in AllItems
                     where x.GetType()
                         .GetProperty(ValuePropertyName)
                         .GetValue(x, null)
                         .ToString() == id
                     select x).FirstOrDefault();

                    if (ItemFromAllItems != null)
                    {
                        AllItems.Remove(ItemFromAllItems);
                    }
                }
            }

            if (AllItems.Count > 0)
            {
                SelectedItem = AllItems.First();
            }
            else if (SelectedItems.Count > 0)
            {
                SelectedItem = SelectedItems.First();
            }

            UpdateButtonEnabledStates();
        }

        protected void ItemSelectedFromAllItems(ChangeEventArgs args)
        {
            SelectedItem =
                (from x in AllItems
                 where x.GetType()
                     .GetProperty(ValuePropertyName)
                     .GetValue(x, null)
                     .ToString() == args.Value.ToString()
                 select x).FirstOrDefault();

            UpdateButtonEnabledStates();
        }

        protected void UpdateButtonEnabledStates()
        {
            addSelectedItemButtonDisabled = !AllItems.Contains(SelectedItem);
            removeSelectedItemButtonDisabled = !SelectedItems.Contains(SelectedItem);
        }

        protected void AddAllItems()
        {
            foreach (var Item in AllItems.ToArray())
            {
                SelectedItems.Add(Item);
            }
            if (SelectedItems.Count > 0)
            {
                SelectedItem = SelectedItems.First();
            }
            AllItems.Clear();
            UpdateButtonEnabledStates();
        }

        protected void RemoveAllItems()
        {
            foreach (var Item in SelectedItems.ToArray())
            {
                AllItems.Add(Item);
            }
            if (AllItems.Count > 0)
            {
                SelectedItem = AllItems.First();
            }
            SelectedItems.Clear();
            UpdateButtonEnabledStates();
        }

        protected void AddSelectedItem()
        {
            if ((from x in SelectedItems
                 where ItemValue(x) == ItemValue(SelectedItem)
                 select x).FirstOrDefault() == null)
            {
                SelectedItems.Add(SelectedItem);
                AllItems.Remove(SelectedItem);
                UpdateButtonEnabledStates();
            }
        }

        protected void RemoveSelectedItem()
        {
            if ((from x in AllItems
                 where ItemValue(x) == ItemValue(SelectedItem)
                 select x).FirstOrDefault() == null)
            {
                AllItems.Add(SelectedItem);
                SelectedItems.Remove(SelectedItem);
                UpdateButtonEnabledStates();
            }
        }

        protected void ItemSelectedFromSelectedItems(ChangeEventArgs args)
        {
            SelectedItem =
                (from x in SelectedItems
                 where x.GetType()
                     .GetProperty(ValuePropertyName)
                     .GetValue(x, null)
                     .ToString() == args.Value.ToString()
                 select x
                ).FirstOrDefault();
            UpdateButtonEnabledStates();
        }
    }
}
