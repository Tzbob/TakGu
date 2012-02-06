using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TakGu
{
    public class MenuItem
    {
        public delegate void OnSelected();

        private string text;
        private OnSelected onSelected;

        public MenuItem(string text, OnSelected onSelected)
        {
            this.text = text;
            this.onSelected = onSelected;
        }

        public void ExecuteSelected()
        {
            this.onSelected();
        }

        public string Text {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }
    }
}
