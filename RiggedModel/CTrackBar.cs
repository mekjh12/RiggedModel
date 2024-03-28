using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSystem
{
    class CTrackBar : TrackBar
    {
        int margin = 5;

        Label _lblMin;
        Label _lblMax;
        Label _lblValue;

        public string ValueText
        {
            set
            {
                _lblValue.Text = value;
            }
        }

        public CTrackBar(string name, int min = 0, int max = 100, int delta = 1)
        {
            this.Name = name;
            _lblMin = new Label() { AutoSize = true };
            _lblMax = new Label() { AutoSize = true };
            _lblValue = new Label() { AutoSize = true };

            this.Minimum = min; 
            this.Maximum = max;
            this.SmallChange = delta;

            this.Width = 300;
            this.Height += _lblMax.Height + 2 * margin;

            this.Controls.Add( _lblMin );
            this.Controls.Add( _lblMax );
            this.Controls.Add(_lblValue);

            this.Scroll += (o, e) => Draw();
        }

        public void Draw()
        {
            int h = this.Height - _lblMin.Height - margin;

            _lblMin.Location = new System.Drawing.Point(margin, h);
            _lblMax.Location = new System.Drawing.Point(this.Width - 20, h);
            _lblValue.Location = new System.Drawing.Point((int)((float)this.Width * 0.5f), h);

            _lblMin.Text = "min:" + Minimum;
            _lblMax.Text = "max:" + Maximum;
            _lblValue.Text = "" + Value;

            _lblValue.Location = new System.Drawing.Point((int)(((float)this.Width - (float)_lblValue.Width) * 0.5f), h);
            _lblValue.Text = Name + "=" + Value;
            _lblMax.Left = this.Width - _lblMax.Width - margin;
        }
    }
}
